using UnityEngine;
using System.Collections;

public class creep : MonoBehaviour {

	//instantiated in prefab
	public int speed;
	public int health;
	public int weight;

	//instantiated upon creation
	public GameObject target;
	public gameManager gm;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	//called on hit
	void hit () {

	}
}
