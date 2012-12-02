using UnityEngine;
using System.Collections;

public class guiManager : MonoBehaviour {

	//scripts (to access certain variables)
	public gameManager gm;		//need current game state
	public spawnManager sm;		//need current wave information
	public touchManager tm;		//need current selected state

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
	private int waveContentInitial;

	private int exitWindowWidth;
	private int exitWindowHeight;
	private bool renderExitWindow;
	private gameState prevState;

	private int selectionOffset;
	private int selectionButtonSize;

/*
	private int buildViewWidth;
	private int buildViewHeight;
	private int buildContentOffset;
	private int buildContentWidth;
	*/

	//type conversion methods
	Texture typeToTurretImage(turretType type)
	{
		switch ((int)type)
		{
			case 0: return basicTurretImage;

			default: break;
		}
		return basicTurretImage;
	}

	Texture typeToCreepImage(creepType type)
	{
		switch ((int)type)
		{
			case 0: return basicCreepImage;

			default: break;
		}
		return basicCreepImage;
	} 

	// Use this for initialization
	void Start () {
		buttonSize = 64;
		buttonOffset = 10;

		waveViewWidth = 400;
		waveViewHeight = 150;
		waveViewVertOffset = 10;
		waveContentOffset = 60;
		waveContentInitial = 120;

		exitWindowWidth = 200;
		exitWindowHeight = 100;
		renderExitWindow = false;
		prevState = (gameState)0;
/*
		buildViewWidth = 500;
		buildViewHeight = 150;
		buildContentOffset = 10;
		buildContentWidth = 80;
		*/

		selectionOffset = 60;
		selectionButtonSize = 50;

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI()
	{
		//Debugging Label
		/*if(gm.state == gameState.buildPhase) GUI.Label(new Rect(100,100,100,100),"Let's Build Yo!");
		else if(gm.state == gameState.battlePhase) GUI.Label(new Rect(100,100,100,100),"Let's Fight Yo!");
		else if(gm.state == gameState.paused) GUI.Label(new Rect(100,100,100,100),"Paused!");
		else if(gm.state == gameState.gameOver) GUI.Label(new Rect(100,100,100,100),"You Lose!");
		else if(gm.state == gameState.gameWon) GUI.Label(new Rect(100,100,100,100),"You Win!");
*/
		//Top-Left Button
		//Pauses/Resumes the game
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
			
			GUI.Label(new Rect(0,0,100,50),("Wave " + (sm.currentWave+1).ToString() + " Up Next: "));
			for(int i = 0; i < 2; i++)
				if(sm.currentWaveIndex+i < sm.allWaves[sm.currentWave].waveSize) 
					GUI.Label(new Rect(waveContentInitial + (i*waveContentOffset),0,50,50),typeToCreepImage((creepType)sm.allWaves[sm.currentWave].creepIDs[sm.currentWaveIndex]));
			
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
			exitWindowRect = GUI.Window(0, exitWindowRect,exitWindow,"Return to Main Menu?");
		}


		//turret build menu
		if(tm.selected == selectedState.tile)
		{
			/*Debug.Log(Screen.height);
			Vector2 buildViewVector = new Vector2(0,0);
			Rect buildViewRect = new Rect((Screen.width-buildViewWidth)/2,(Screen.height-buildViewHeight),buildViewWidth,buildViewHeight); 
			buildViewVector = GUI.BeginScrollView(buildViewRect,buildViewVector, new Rect(0,0,buildViewWidth, buildViewHeight));

			GUI.Label(new Rect(0,0,100,50),"Build!");

			GUI.EndScrollView();
			*/
			for(int i = -1; i <= 1; i++)
			{
				for(int j = -1; j <= 1; j++)
				{
					if(i != 0 || j != 0)
					{
						 int typeIndex = (i+1)+((j+1)*3);
						 Rect buttonRect = new Rect(tm.selectedPos.x + (selectionOffset*i) - (buttonSize/2),Screen.height-tm.selectedPos.y + (selectionOffset*j) - (buttonSize/2),selectionButtonSize,selectionButtonSize);
						 if(typeIndex < System.Enum.GetNames(typeof(turretType)).Length)
						 {
						 	if (GUI.Button(buttonRect,typeToTurretImage((turretType)typeIndex)))
						 	{
						 		//set turret
						 		gm.createTurret(new Vector2(tm.selectedObject.transform.position.x,tm.selectedObject.transform.position.z),(turretType)typeIndex);
						 		tm.clickable = true;
						 		tm.selected = selectedState.none;
						 	}
						 }
						 else if(GUI.Button(buttonRect,"")) {tm.clickable = true;  tm.selected = selectedState.none;}
					}
				}
			}
		}
		if(tm.selected == selectedState.turret) tm.clickable = true;
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
