using UnityEngine;
using System.Collections;

public class guiManager : MonoBehaviour {

	public gameManager gm;

	//textures
	public Texture playButtonTexture;
	public Texture pauseButtonTexture;

	//GUISkins
	public GUISkin buttonSkin;

	//size variables
	private int buttonSize;

	// Use this for initialization
	void Start () {
		buttonSize = 64;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI()
	{
		//Debugging Label
		if(gm.state == gameState.buildPhase) GUI.Label(new Rect(100,100,100,100),"Let's Build Yo!");
		else if(gm.state == gameState.battlePhase) GUI.Label(new Rect(100,100,100,100),"Let's Fight Yo!");
		else if(gm.state == gameState.paused) GUI.Label(new Rect(100,100,100,100),"Paused!");
		else if(gm.state == gameState.gameOver) GUI.Label(new Rect(100,100,100,100),"You Lose!");
		else if(gm.state == gameState.gameWon) GUI.Label(new Rect(100,100,100,100),"You Win!");

		//Top-Left Button
		GUI.skin = buttonSkin;
		if(gm.state == gameState.buildPhase || gm.state == gameState.paused)
		{		
			if(GUI.Button(new Rect(10,10,buttonSize,buttonSize),playButtonTexture))
			{
				gm.state = gameState.battlePhase;
			}
		}
		else if(gm.state == gameState.battlePhase)
		{
			if(GUI.Button(new Rect(10,10,buttonSize,buttonSize),pauseButtonTexture))
			{
				gm.state = gameState.paused;
			}
		}
		GUI.skin = null;
	}
}
