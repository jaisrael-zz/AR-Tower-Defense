using UnityEngine;
using System.Collections;

public class gameManager : MonoBehaviour {

	public gridCreator gc;
	private GameObject[,] grid;

	private int frame;
	private int fps;

	// Use this for initialization
	void Start () {
		grid = gc.createGameGrid();

		frame = 0;
		fps = 60;
	}
	
	// Update is called once per frame
	void Update () {
	
		frame++;

	}
}
