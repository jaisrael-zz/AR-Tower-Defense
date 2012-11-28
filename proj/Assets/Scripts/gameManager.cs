using UnityEngine;
using System.Collections;

public class gameManager : MonoBehaviour {

	public gridCreator gc;
	private GameObject[,] grid;

	// Use this for initialization
	void Start () {
		grid = gc.createGameGrid();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
