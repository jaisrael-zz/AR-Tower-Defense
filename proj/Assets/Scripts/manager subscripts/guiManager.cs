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

	//turret images
	public Texture basicTurretImage;
	public Texture bashTurretImage;
	public Texture slowTurretImage;
	public Texture burnTurretImage;
	public Texture snipeTurretImage;

	//creep images
	public Texture basicCreepImage;
	public Texture quickCreepImage;
	public Texture quickStatusCreepImage;
	public Texture strongCreepImage;
	public Texture strongStatusCreepImage;

	//GUISkins
	public GUISkin buttonSkin;

	//GUIStyles
	public GUIStyle mainstyle;

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
			case 1: return bashTurretImage;
			case 2: return slowTurretImage;
			case 3: return burnTurretImage;
			case 4: return snipeTurretImage;
			default: break;
		}
		return basicTurretImage;
	}
	int typeToTurretCost(turretType type)
	{
		switch ((int)type)
		{
			case 0: return 1;
			case 1: return 10;
			case 2: return 3;
			case 3: return 3;
			case 4: return 10;
			default: break;
		}
		return 1;
	}

	Texture typeToCreepImage(creepType type)
	{
		switch ((int)type)
		{
			case 0: return basicCreepImage;
			case 1: return quickCreepImage;
			case 2: return quickStatusCreepImage;
			case 3: return strongCreepImage;
			case 4: return strongStatusCreepImage;
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
		waveContentOffset = 50;
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
		//GUI.skin = null;

		//Top-Center (Wave Information)
		//Very Ugly ><
		if(gm.state != gameState.gameWon)
		{
			Vector2 waveViewVector = new Vector2(0,0);
			Rect waveViewRect = new Rect((Screen.width-waveViewWidth)/2,waveViewVertOffset,waveViewWidth,waveViewHeight);
			waveViewVector = GUI.BeginScrollView(waveViewRect,waveViewVector,new Rect(0,0,waveViewWidth,waveViewHeight));
			
			GUI.Label(new Rect(0,buttonOffset,100,50),("WAVE " + (sm.currentWave+1).ToString() + "\n" + " UP NEXT: "));
			for(int i = 0; i <= 2; i++)
				if(sm.currentWaveIndex+i < sm.allWaves[sm.currentWave].waveSize) 
					GUI.Label(new Rect(waveContentInitial + (i*waveContentOffset),0,40,40),typeToCreepImage((creepType)sm.allWaves[sm.currentWave].creepIDs[sm.currentWaveIndex+i]));
			
			GUI.EndScrollView();
		}
		else
		{
			GUI.Label(new Rect(0,buttonOffset,100,50),("ALL WAVES CLEARED!"));
		}

		//Top-Right Button
		//pulls up game exit window
		//GUI.skin = buttonSkin;
		if(GUI.Button(new Rect(Screen.width-buttonSize-buttonOffset,buttonOffset,buttonSize,buttonSize),exitButtonTexture))
		{
			if(gm.state == gameState.gameWon)
			{
				Application.LoadLevel("VictoryScreen");
			}
			else
			{
				renderExitWindow = true;
				prevState = gm.state;
				gm.state = gameState.paused;
			}
		}
		//GUI.skin = null;
		if(renderExitWindow)
		{
			Rect exitWindowRect = new Rect((Screen.width-exitWindowWidth)/2,(Screen.height-exitWindowHeight)/2,exitWindowWidth,exitWindowHeight);
			exitWindowRect = GUI.Window(0, exitWindowRect,exitWindow,"RETURN TO MAIN MENU?");
		}

		//Available Units
		GUI.Label(new Rect(buttonOffset,buttonOffset+buttonSize+buttonOffset,100,50),"UNITS: "+gm.availableUnits.ToString());

		//turret build menu
		GUI.skin = null;
		if(tm.selected == selectedState.tile && gm.state == gameState.buildPhase)
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
						 if (typeIndex >= 4) typeIndex -= 1; //account for center square
						 Rect buttonRect = new Rect(tm.selectedPos.x + (selectionOffset*i) - (buttonSize/2) ,Screen.height-tm.selectedPos.y + (selectionOffset*j) - (buttonSize/2),selectionButtonSize,selectionButtonSize);
						 //need to change
						 if(typeIndex < System.Enum.GetNames(typeof(turretType)).Length && gm.availableUnits >= typeToTurretCost((turretType)typeIndex))
						 {
						 	if (GUI.Button(buttonRect,typeToTurretImage((turretType)typeIndex)))
						 	{
						 		//set turret
						 		Debug.Log((turretType)typeIndex);
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
		if(tm.selected == selectedState.turret) {
			tm.clickable = true;
			tm.selected = selectedState.none;
		}
	}

	void exitWindow(int windowID)
	{
		GUI.skin = buttonSkin;
		if (GUI.Button(new Rect(80,25,40,20),"YES"))
		{
			Application.LoadLevel("StartMenu");
		}
		if (GUI.Button(new Rect(80,65,40,20),"NO"))
		{
			renderExitWindow = false;
			gm.state = prevState;
		}
	}
}
