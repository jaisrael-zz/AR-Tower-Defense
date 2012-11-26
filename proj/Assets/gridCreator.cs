using UnityEngine;
using System.Collections;

public class gridCreator : MonoBehaviour {

	public GameObject gridTile;

	int gridWidth = 10;
	int gridHeight = 10;

	GameObject[,] grid;

	// Use this for initialization
	void Start () {

		grid = new GameObject[gridWidth,gridHeight];

		for(int i = 0; i < gridWidth; i++)
		{
			for(int j = 0; j < gridHeight; j++)
			{
				GameObject tile = (GameObject)Instantiate(gridTile);
				tile.transform.position = new Vector3(i,0,j);

				grid[i,j] = tile;
			}
		}


	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
