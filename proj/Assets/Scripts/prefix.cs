//Jason Israel (jaisrael)
//11/25/12

//contains variables intended for global use

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

//public int currentState;

/////////////////////////////////////////////////////////////////////

//int gridWidth = 10;
//int gridHeight = 10;

//GameObject[][] grid = new GameObject[gridWidth][gridHeight];

/////////////////////////////////////////////////////////////////////

enum objectType {
	turret,
	creep,
	missile
};

enum turretType {
	basic,
	splash
};

enum missileType {
	basic,
	splash
};

/////////////////////////////////////////////////////////////////////

/*public class prefix : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}*/
