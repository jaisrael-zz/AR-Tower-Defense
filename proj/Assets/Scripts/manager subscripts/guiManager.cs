using UnityEngine;
using System.Collections;

public class guiManager : MonoBehaviour {

	public gameManager gm;

	//button textures
	public Texture playButtonTexture;
	public Texture pauseButtonTexture;

	//GUISkins
	public GUISkin buttonSkin;

	void OnGUI()
	{
		//Top-Left Button
		GUI.skin = buttonSkin;
		if(gm.state == gameState.buildPhase)
		{
			GUI.Label(new Rect(100,100,100,100),"Let's Build Yo!");
			if(GUI.Button(new Rect(10,10,64,64),playButtonTexture))
			{
				Debug.Log("Play Pressed!");
			}
		}
		else if(gm.state == gameState.battlePhase)
		{
			GUI.Label(new Rect(100,100,100,100),"Let's Fight Yo!");
			if(GUI.Button(new Rect(10,10,64,64),pauseButtonTexture))
			{
				Debug.Log("Play Pressed!");
				gm.state = gameState.paused;
			}
		}
		else if(gm.state == gameState.paused)
		{
			GUI.Label(new Rect(100,100,100,100),"Paused!");
			if(GUI.Button(new Rect(10,10,64,64),playButtonTexture))
			{
				Debug.Log("Play Pressed!");
				gm.state = gameState.battlePhase;
			}
		}
		GUI.skin = null;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
