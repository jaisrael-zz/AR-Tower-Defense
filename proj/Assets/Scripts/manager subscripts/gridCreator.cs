using UnityEngine;
using System.Collections;



public class gridCreator : MonoBehaviour {

	public GameObject gridTile;
	public GameObject edgeLight;
	public GameObject GlobalLight;

	public int gridWidth;
	public int gridHeight;

	GameObject[,] grid;

	// Use this for initialization
	public GameObject[,] createGameGrid()
	{
		grid = new GameObject[gridWidth,gridHeight];

		for(int i = 0; i < gridWidth; i++)
		{
			for(int j = 0; j < gridHeight; j++)
			{
				GameObject tile = (GameObject)Instantiate(gridTile);
				tile.transform.position = new Vector3(i,0,j);
				tile.tag = "Tile";

				grid[i,j] = tile;
			}
		}


/*		for(float i = 0.0f; i <= gridWidth*1.1f; i+=2.2f)
		{
			for(float j = 0.0f; j <= gridHeight*1.1f; j+=2.2f)
			{
				GameObject light = (GameObject)Instantiate(edgeLight);
				light.transform.position = new Vector3(i-0.5f,0,j-2f);
			}
		}
*/
		GameObject light = (GameObject)Instantiate(GlobalLight);

		return grid;
	}

	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
