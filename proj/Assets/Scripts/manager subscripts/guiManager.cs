using UnityEngine;
using System.Collections;

public class guiManager : MonoBehaviour {

	public gameManager gm;

	void OnGUI()
	{
		//GUI.Label(new Rect(100,100,100,100),"Hello World!");

		if(gm.state == gameState.buildPhase)
		{
			GUI.Label(new Rect(100,100,100,100),"Let's Build Yo!");
		}
		else if(gm.state == gameState.battlePhase)
		{
			GUI.Label(new Rect(100,100,100,100),"Let's Fight Yo!");
		}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
