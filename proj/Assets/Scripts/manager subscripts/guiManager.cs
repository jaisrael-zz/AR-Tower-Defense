using UnityEngine;
using System.Collections;

public class guiManager : MonoBehaviour {

	//scripts (to access certain variables)
	public gameManager gm;		//need current game state
	public spawnManager sm;		//need current wave information

	//textures
	public Texture playButtonTexture;
	public Texture pauseButtonTexture;
	public Texture exitButtonTexture;

	//creep images
	public Texture basicTurretImage;

	//turret images
	public Texture basicCreepImage;

	//GUISkins
	public GUISkin buttonSkin;

	//size variables
	private int buttonSize;
	private int buttonOffset;

	private int waveViewWidth;
	private int waveViewHeight;
	private int waveViewVertOffset;
	private int waveContentOffset;

	private int exitWindowWidth;
	private int exitWindowHeight;
	private bool renderExitWindow;
	private gameState prevState;

	// Use this for initialization
	void Start () {
		buttonSize = 64;
		buttonOffset = 10;

		waveViewWidth = 400;
		waveViewHeight = 150;
		waveViewVertOffset = 10;
		waveContentOffset = 100;

		exitWindowWidth = 200;
		exitWindowHeight = 100;
		renderExitWindow = false;
		prevState = (gameState)0;
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
		//Pauses/Resumes the game
		Debug.Log(GUI.skin);
		GUI.skin = buttonSkin;
		if(gm.state == gameState.buildPhase || gm.state == gameState.paused)
		{		
			if(GUI.Button(new Rect(buttonOffset,buttonOffset,buttonSize,buttonSize),playButtonTexture))
			{
				gm.state = gameState.battlePhase;
			}
		}
		else if(gm.state == gameState.battlePhase)
		{
			if(GUI.Button(new Rect(buttonOffset,buttonOffset,buttonSize,buttonSize),pauseButtonTexture))
			{
				gm.state = gameState.paused;
			}
		}
		GUI.skin = null;

		//Top-Center (Wave Information)
		//Very Ugly ><
		if(gm.state != gameState.gameWon)
		{
			Vector2 waveViewVector = new Vector2(0,0);
			Rect waveViewRect = new Rect((Screen.width-waveViewWidth)/2,waveViewVertOffset,waveViewWidth,waveViewHeight);
			waveViewVector = GUI.BeginScrollView(waveViewRect,waveViewVector,new Rect(0,0,waveViewWidth,waveViewHeight));
			
			GUI.Label(new Rect(0,0,100,50),("Wave " + sm.currentWave.ToString() + " Up Next: "));
			if(sm.currentWaveIndex < sm.allWaves[sm.currentWave].waveSize) 
				GUI.Label(new Rect(120,0,50,50),basicCreepImage);
			if(sm.currentWaveIndex+1 < sm.allWaves[sm.currentWave].waveSize) 
				GUI.Label(new Rect(180,0,50,50),basicCreepImage);
			if(sm.currentWaveIndex+2 < sm.allWaves[sm.currentWave].waveSize) 
				GUI.Label(new Rect(240,0,50,50),basicCreepImage);
			
			GUI.EndScrollView();
		}

		//Top-Right Button
		//pulls up game exit window
		GUI.skin = buttonSkin;
		if(GUI.Button(new Rect(Screen.width-buttonSize-buttonOffset,buttonOffset,buttonSize,buttonSize),exitButtonTexture))
		{
			renderExitWindow = true;
			prevState = gm.state;
			gm.state = gameState.paused;
		}
		GUI.skin = null;
		if(renderExitWindow)
		{
			Rect exitWindowRect = new Rect((Screen.width-exitWindowWidth)/2,(Screen.height-exitWindowHeight)/2,exitWindowWidth,exitWindowHeight);
			exitWindowRect = GUI.Window(0, exitWindowRect,exitWindow,"Are You Sure?");
		}
	}

	void exitWindow(int windowID)
	{
		if (GUI.Button(new Rect(80,25,40,20),"Yes"))
		{
			Application.LoadLevel("StartMenu");
		}
		if (GUI.Button(new Rect(80,65,40,20),"No"))
		{
			renderExitWindow = false;
			gm.state = prevState;
		}
	}
}
