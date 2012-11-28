using UnityEngine;
using System.Collections;

public class turret : MonoBehaviour {

	//instantiated through prefab
	public int identifier; 			//turret type
	public GameObject missile;		//missile
	public int rate;				//fireRate
	public int range;

	//instantiated upon creation (gameManager.cs)
	public gameManager gm;			//gets other object and game data

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
