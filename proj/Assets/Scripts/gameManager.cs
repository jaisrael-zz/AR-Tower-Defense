using UnityEngine;
using System.Collections;

//default enum initialization 0,1,2,...
//possible game states
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
	//object types created here:
	
	//turrets
	public GameObject basicTurret;

	//creeps
	public GameObject basicCreep;

	//goal
	//public GameObject goal;

	//spawn point
	//public GameObject spawn;

	/////////////////////////////////////////////////////

	//makes game grid
	public gridCreator gc;
	private GameObject[,] grid;
	private bool[,] traversible;

	//used for object creation
	private int complexity;

	//public ArrayList turrets;
	//public ArrayList creeps;

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
	void createTurret(Vector2 gridPos, turretType type)
	{
		GameObject newTurretType = typeToTurret(type);
		GameObject newTurret = (GameObject)Instantiate(newTurretType,new Vector3(gridPos.x,0.4f,gridPos.y),Quaternion.identity);
		newTurret.tag = "Turret";
		traversible[(int)gridPos.x,(int)gridPos.y] = false;
		//newTurret.GetComponent("Creep").gm = this.GetComponent("Game Manager");
		//turrets.Add(newTurret);
	}

	void createCreep(Vector2 gridPos, creepType type)
	{
		GameObject newCreepType = typeToCreep(type);
		GameObject newCreep = (GameObject)Instantiate(newCreepType,new Vector3(gridPos.x,0.4f,gridPos.y),Quaternion.identity);
		//newCreep.GetComponent("Turret").gm = this.GetComponent("Game Manager");
		newCreep.tag = "Creep";
		//creeps.Add(newCreep);
	}

	void Start () {

		state = gameState.battlePhase;

		this.gameObject.tag = "Main";
		//turrets = new ArrayList();
		//creeps = new ArrayList();

		grid = gc.createGameGrid();
		traversible = new bool[gc.gridWidth,gc.gridHeight];
		for(int i = 0; i < gc.gridWidth; i++)
			for(int j = 0; j < gc.gridHeight; j++)
				traversible[i,j] = true;

		complexity = 10;	

		createTurret(new Vector2(5,5),turretType.basic);
		createTurret(new Vector2(7,5),turretType.basic);
		createCreep(new Vector2(3,3),creepType.basic);
		createCreep(new Vector2(6,4),creepType.basic);
	}
	
	// Update is called once per frame
	void Update () {

		if((int)state == (int)gameState.battlePhase)
		{
			//execute turret AI
			foreach(GameObject currentTurret in GameObject.FindGameObjectsWithTag("Turret"))
			{
				turret t = (turret)currentTurret.GetComponent("turret");
				t.Fire(new Vector2(9,9));
			}

			//execute creep AI
			foreach(GameObject currentCreep in GameObject.FindGameObjectsWithTag("Creep"))
			{
				creep c = (creep)currentCreep.GetComponent("creep");
				c.Seek(new Vector2(9,9),traversible,10);
			}
		}
		else if((int)state == (int)gameState.battlePhase)
		{
			Debug.Log("Let's Build Yo");
		}
	}
}
