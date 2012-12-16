using UnityEngine;
using System.Collections;

public enum gameState {
	buildPhase,
	battlePhase,
	paused,
	gameOver,
	gameWon
};
//possible turrets
public enum turretType {
	basic,
	bash,
	slow,
	burn,
	snipe
}
//possible creeps
public enum creepType {
	basic,
	quick,
	quickStatus,
	strong,
	strongStatus
}


public class gameManager : GameLevel {

	public gameState state;

	/////////////////////////////////////////////////////
	//Object Management
	
	//turrets
	public GameObject basicTurret;
	public GameObject bashTurret;
	public GameObject slowTurret;
	public GameObject burnTurret;
	public GameObject snipeTurret;

	//creeps
	public GameObject basicCreep;
	public GameObject quickCreep;
	public GameObject quickStatusCreep;
	public GameObject strongCreep;
	public GameObject strongStatusCreep;

	//other objects
	public GameObject goal;
	public GameObject spawn;
	public GameObject pathSystem; //highlighted creep path



	/////////////////////////////////////////////////////
	//Grid Management
	//makes game grid
	public gridCreator gc;
	private GameObject[,] grid;

	public bool[,] traversible;		//map of occupied spaces
	public float[,] influence; 		//influence map
	public float totalInfluence;	//used for heuristic
	public ArrayList currentPath;	

	public Vector2 goalPos;

	public GameObject selectedTile;	

	public int availableUnits;		//purchasing power

	/////////////////////////////////////////////////////
	//Spawn Management
	public spawnManager sm;

	//GUI Management
	public guiManager gm;

	//Touch Management
	public touchManager tm;

	//Path Management
	public pathManager pm;

	/////////////////////////////////////////////////////
	//Initialization

	// enum converters
	GameObject typeToTurret(turretType type)
	{
		switch ((int)type)
		{
			case 0: return basicTurret;
			case 1: return bashTurret;
			case 2: return slowTurret;
			case 3: return burnTurret;
			case 4: return snipeTurret;

			default: break;
		}
		return basicTurret;
	}

	GameObject typeToCreep(creepType type)
	{
		switch ((int)type)
		{
			case 0: return basicCreep;
			case 1: return quickCreep;
			case 2: return quickStatusCreep;
			case 3: return strongCreep;
			case 4: return strongStatusCreep;

			default: break;
		}
		return basicCreep;
	} 

	float typeToInfluence(turretType type)
	{
		switch ((int)type)
		{
			case 0: return 1; 
			case 1: return 10;
			case 2: return 3;
			case 3: return 3;
			case 4: return 10;

			default: break;
		}
		return 1;
	}

	//updates influence map when turret is added/removed
	void applyToInfluenceMap(GameObject turret,bool add)
	{
		Vector2 turretPos = new Vector2(turret.transform.position.x,turret.transform.position.z);
		turret t = ((turret)turret.GetComponent("turret"));
		for(int i = (int)(turretPos.x-t.range); i <= (int)(turretPos.x+t.range); i++)
		{
			for(int j = (int)(turretPos.y-t.range); j <= (int)(turretPos.y+t.range); j++)
			{
				if(i>=0 && j>=0 && i < gc.gridWidth && j < gc.gridHeight)
				{
					if(Mathf.Abs(turretPos.x-i)+Mathf.Abs(turretPos.y-j) <= t.range)
					{
						float inf = typeToInfluence((turretType)t.identifier);
						if(add) { 	influence[i,j] += inf; totalInfluence += inf; }
						else { 		influence[i,j] -= inf; totalInfluence -= inf; }
					}
				}	
			}			
		}
	}

	//destroys turret, sets grid traversability to true, updates influence map
	public void destroyTurret(GameObject turret)
	{
		traversible[(int)turret.transform.position.x,(int)turret.transform.position.z] = true;
		availableUnits += ((turret)turret.GetComponent("turret")).cost;
		applyToInfluenceMap(turret,false);
		Destroy(turret);
	}

	//initializers for objects
	//createss turret, and sets grid tile traversibility to false, updates influence map
	public void createTurret(Vector2 gridPos, turretType type)
	{
		GameObject newTurretType = typeToTurret(type);
		GameObject newTurret = (GameObject)Instantiate(newTurretType,new Vector3(gridPos.x,0.4f,gridPos.y),Quaternion.identity);
		newTurret.tag = "Turret";
		traversible[(int)gridPos.x,(int)gridPos.y] = false;
		availableUnits -= ((turret)newTurret.GetComponent("turret")).cost;
		applyToInfluenceMap(newTurret,true);
		ArrayList proposedPath = pm.updatePath(sm.spawnPos,goalPos);
		if(proposedPath != null) currentPath = proposedPath;
		else destroyTurret(newTurret);
		updatePathParticles();
		//newTurret.GetComponent("Creep").gm = this.GetComponent("Game Manager");
		//turrets.Add(newTurret);
	}

	//creates creep
	public void createCreep(Vector2 gridPos, creepType type)
	{
		GameObject newCreepType = typeToCreep(type);
		GameObject newCreep = (GameObject)Instantiate(newCreepType,new Vector3(gridPos.x,0.6f,gridPos.y),Quaternion.identity);
		//newCreep.GetComponent("Turret").gm = this.GetComponent("Game Manager");
		newCreep.tag = "Creep";
		//creeps.Add(newCreep);
	}

	//creates goal, may not meed to exist
	void createGoal(Vector2 gridPos)
	{
		GameObject newGoal = (GameObject)Instantiate(goal,new Vector3(gridPos.x,0.4f,gridPos.y),Quaternion.identity);
		newGoal.tag = "Goal";
	}

	//creates spawn
	void createSpawn(Vector2 gridPos)
	{
		GameObject newSpawn = (GameObject)Instantiate(spawn,new Vector3(gridPos.x,0.4f,gridPos.y),Quaternion.identity);
		newSpawn.tag = "Spawn";
	}

	void Start () {

		state = gameState.buildPhase;

		//Debug.Log(this.gameObject.tag);

		//this.gameObject.tag = "Main";
		//turrets = new ArrayList();
		//creeps = new ArrayList();

		//base environment
		grid = gc.createGameGrid();
		traversible = new bool[gc.gridWidth,gc.gridHeight];
		influence = new float[gc.gridWidth,gc.gridHeight];
		for(int i = 0; i < gc.gridWidth; i++) {
			for(int j = 0; j < gc.gridHeight; j++) {
				traversible[i,j] = true;
				influence[i,j] = 0;
			}
		}
		totalInfluence = 0;

		goalPos = new Vector2(gc.gridWidth-1,gc.gridHeight-1);

		createGoal(goalPos);
		createSpawn(sm.spawnPos);

		currentPath = new ArrayList();
		currentPath = pm.updatePath(sm.spawnPos,goalPos);
		if(currentPath == null) Debug.Log("It's null yo");
		updatePathParticles();
	}
	
	/////////////////////////////////////////////////////

	// game State Managers
	// currently, it would make sense to instead just change
	// the state variable where these function calls happen,
	// other things could happen though
	public void lose () {
		state = gameState.gameOver;
		Application.LoadLevel("GameOver");
	}

	public void win () {
		state = gameState.buildPhase;
	}

	public void totalWin () {
		state = gameState.gameWon;
	}

	public void beginWave () {
		state = gameState.battlePhase;
	}
	/////////////////////////////////////////////////////

	public void updateAvailableUnits(int gold)
	{
		availableUnits += gold;
	}

	// this is called every time the path is changed
	void updatePathParticles () {
		if(GameObject.FindWithTag("Particle") != null)
		{
			foreach(GameObject particle in GameObject.FindGameObjectsWithTag("Particle"))
			{
				Destroy(particle);
			}
		}
		if(currentPath != null)
		{
			foreach(Vector2 pos in currentPath)
			{
				GameObject pathParticle = (GameObject)Instantiate(pathSystem,new Vector3(pos.x,0.0f,pos.y),Quaternion.identity);
				pathParticle.tag = "Particle";
			}
		}
	}

	// Update is called once per frame
	void Update () {

		if((int)state == (int)gameState.battlePhase)
		{

			//continue spawning current wave
			sm.UpdateSpawn();

			//execute turret AI
			foreach(GameObject currentTurret in GameObject.FindGameObjectsWithTag("Turret"))
			{
				turret t = (turret)currentTurret.GetComponent("turret");
				t.Fire(new Vector2(9,9));
			}

			//execute creep AI
			GameObject[] creeps = GameObject.FindGameObjectsWithTag("Creep");
			foreach(GameObject currentCreep in creeps)
			{
				creep c = (creep)currentCreep.GetComponent("creep");
				c.Seek(new Vector2(9,9),currentPath,10);
				c.updateStatuses();
			}

			//update missiles 
			foreach(GameObject currentMissile in GameObject.FindGameObjectsWithTag("Missile"))
			{
				missile m = (missile)currentMissile.GetComponent("missile");
				m.Fly();
			}

			//check if wave is defeated
			if(creeps == null || creeps.Length == 0)
			{
				Debug.Log("here");
				if(sm.isWaveDefeated())
				{
					updateAvailableUnits(sm.currentWave);
					if(sm.currentWave == sm.totalWaves) totalWin();
					else win();
				}
			}
			if(tm.selected == selectedState.creep && tm.selectedObject != null)
			{
				//apply stun 
				creep c = (creep)tm.selectedObject.GetComponent("creep");
				c.applyStatus(creepStatus.stun,3*c.durationMultipliers[1]);
				//tm.selected = selectedState.none;
			}
			tm.selected = selectedState.none;
			tm.clickable = true;
		}
	}
}
