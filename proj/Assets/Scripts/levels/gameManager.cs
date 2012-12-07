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
	basic
}
//possible creeps
public enum creepType {
	basic
}


public class gameManager : GameLevel {

	public gameState state;

	/////////////////////////////////////////////////////
	//Object Management
	
	//turrets
	public GameObject basicTurret;

	//creeps
	public GameObject basicCreep;

	public GameObject goal;
	public GameObject spawn;
	public GameObject pathSystem;



	/////////////////////////////////////////////////////
	//Grid Management
	//makes game grid
	public gridCreator gc;
	private GameObject[,] grid;
	public bool[,] traversible;
	public float[,] influence;
	public float totalInfluence;
	public ArrayList currentPath;

	//public ArrayList turrets;
	//public ArrayList creeps;

	public Vector2 goalPos;

	public GameObject selectedTile;

	public int availableUnits;

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

	// enum to GameObject converters
	GameObject typeToTurret(turretType type)
	{
		switch ((int)type)
		{
			case 0: return basicTurret;

			default: break;
		}
		return basicTurret;
	}

	GameObject typeToCreep(creepType type)
	{
		switch ((int)type)
		{
			case 0: return basicCreep;

			default: break;
		}
		return basicCreep;
	} 

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
						if(add) { 	influence[i,j] += 1; totalInfluence += 1; }
						else { 		influence[i,j] -= 1; totalInfluence -= 1; }
					}
				}
				
			}
			
		}
		/*for(int y = gc.gridHeight-1; y >= 0; y--)
		{
			string debugstr = "";
			for(int x = 0; x < gc.gridWidth; x++)
			{
				debugstr += influence[x,y] + " ";
			}
			Debug.Log(debugstr);
		}*/
		
		//Debug.Log(totalInfluence);

	}

	public void destroyTurret(GameObject turret)
	{
		traversible[(int)turret.transform.position.x,(int)turret.transform.position.z] = true;
		availableUnits += ((turret)turret.GetComponent("turret")).cost;
		applyToInfluenceMap(turret,false);
		Destroy(turret);
	}

	//initializers for objects
	//createss turret, and sets grid tile traversibility to false
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
		GameObject newCreep = (GameObject)Instantiate(newCreepType,new Vector3(gridPos.x,0.4f,gridPos.y),Quaternion.identity);
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
					if(sm.currentWave == sm.totalWaves) totalWin();
					else win();
				}
			}
		}
		else if((int)state == (int)gameState.buildPhase)
		{
			//Debug.Log("Let's Build Yo");
			if(tm.selected == selectedState.turret)
			{

			}
			else if(tm.selected == selectedState.tile)
			{

			}
			else if(tm.selected == selectedState.creep)
			{
				//apply stun 
			}

		}
		else if((int)state == (int)gameState.gameOver)
		{
			//Debug.Log("YOU LOSE");
		}
	}


}
