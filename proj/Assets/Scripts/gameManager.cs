using UnityEngine;
using System.Collections;

public enum gameState {
	titleScreen,
	loadScreen,
	buildPhase,
	battlePhase,
	gameOver
};
//possible turrets
public enum turretType {
	basic
}
//possible creeps
public enum creepType {
	basic
}


public class gameManager : MonoBehaviour {

	public gameState state;

	/////////////////////////////////////////////////////
	//Object Management
	
	//turrets
	public GameObject basicTurret;

	//creeps
	public GameObject basicCreep;

	//goal
	public GameObject goal;

	//spawn point
	public GameObject spawn;

	/////////////////////////////////////////////////////
	//Grid Management
	//makes game grid
	public gridCreator gc;
	private GameObject[,] grid;
	private bool[,] traversible;

	//public ArrayList turrets;
	//public ArrayList creeps;

	public Vector2 goalPos;

	/////////////////////////////////////////////////////
	//Spawn Management
	public spawnManager sm;


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

	//initializers for objects
	//createss turret, and sets grid tile traversibility to false
	void createTurret(Vector2 gridPos, turretType type)
	{
		GameObject newTurretType = typeToTurret(type);
		GameObject newTurret = (GameObject)Instantiate(newTurretType,new Vector3(gridPos.x,0.4f,gridPos.y),Quaternion.identity);
		newTurret.tag = "Turret";
		traversible[(int)gridPos.x,(int)gridPos.y] = false;
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

		state = gameState.battlePhase;

		this.gameObject.tag = "Main";
		//turrets = new ArrayList();
		//creeps = new ArrayList();

		//base environment
		grid = gc.createGameGrid();
		traversible = new bool[gc.gridWidth,gc.gridHeight];
		for(int i = 0; i < gc.gridWidth; i++)
			for(int j = 0; j < gc.gridHeight; j++)
				traversible[i,j] = true;

		goalPos = new Vector2(gc.gridWidth-1,gc.gridHeight-1);

		createGoal(goalPos);
		createSpawn(sm.spawnPos);
	

		//test environment
		createTurret(new Vector2(5,5),turretType.basic);
		createTurret(new Vector2(7,5),turretType.basic);
		createTurret(new Vector2(8,8),turretType.basic);
		createTurret(new Vector2(1,1),turretType.basic);
		createTurret(new Vector2(4,0),turretType.basic);
		createTurret(new Vector2(4,1),turretType.basic);
		createTurret(new Vector2(4,2),turretType.basic);

		/*createCreep(new Vector2(3,3),creepType.basic);
		createCreep(new Vector2(6,4),creepType.basic);
		createCreep(new Vector2(1,5),creepType.basic);
		createCreep(new Vector2(2,5),creepType.basic);
*/

	}
	
	/////////////////////////////////////////////////////

	// game State Managers
	// currently, it would make sense to instead just change
	// the state variable where these function calls happen,
	// other things could happen though
	public void lose () {
		state = gameState.gameOver;
	}

	public void win () {
		state = gameState.buildPhase;
	}

	public void beginWave () {
		state = gameState.battlePhase;
	}
	/////////////////////////////////////////////////////

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
				c.Seek(new Vector2(9,9),traversible,10);
			}
			//Debug.Log(creeps.Length);
			//check if wave is defeated
			if(creeps == null || creeps.Length == 0)
			{
				Debug.Log("here");
				if(sm.isWaveDefeated())
				{
					win();
				}
			}
		}
		else if((int)state == (int)gameState.buildPhase)
		{
			Debug.Log("Let's Build Yo");
		}
		else if((int)state == (int)gameState.gameOver)
		{
			Debug.Log("YOU LOSE");
		}
	}
}
