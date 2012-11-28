using UnityEngine;
using System.Collections;

//default enum initialization 0,1,2,...
//possible game states
enum gameState {
	titleScreen,
	loadScreen,
	buildPhase,
	battlePhase
};
//possible turrets
enum turretType {
	basic
}
//possible creeps
enum creepType {
	basic
}

public class gameManager : MonoBehaviour {

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

	//used for update
	private int frame;
	private int fps;

	//used for object creation
	private int complexity;

	private ArrayList turrets;
	private ArrayList creeps;

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
		//newTurret.GetComponent("Creep").gm = this.GetComponent("Game Manager");
		turrets.Add(newTurret);
	}

	void createCreep(Vector2 gridPos, creepType type)
	{
		GameObject newCreepType = typeToCreep(type);
		GameObject newCreep = (GameObject)Instantiate(newCreepType,new Vector3(gridPos.x,0.4f,gridPos.y),Quaternion.identity);
		//newCreep.GetComponent("Turret").gm = this.GetComponent("Game Manager");
		creeps.Add(newCreep);
	}

	void Start () {
		turrets = new ArrayList();
		creeps = new ArrayList();

		grid = gc.createGameGrid();

		frame = 0;
		fps = 60;

		complexity = 10;	

		createTurret(new Vector2(5,5),turretType.basic);
		createCreep(new Vector2(3,3),creepType.basic);
	}
	
	// Update is called once per frame
	void Update () {
	
		frame++;

	}
}
