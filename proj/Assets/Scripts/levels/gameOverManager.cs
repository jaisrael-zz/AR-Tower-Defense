using UnityEngine;
using System.Collections;

public class gameOverManager : GameLevel {

	public GUIStyle titleStyle;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI () {

		GUI.Label(new Rect(0,0,Screen.width,100),"GAME OVER.",titleStyle);
		if (GUI.Button( new Rect((Screen.width/2)-50,(Screen.height/2)+30,100,60),"quit",titleStyle))
		{
			Application.LoadLevel("StartMenu");
		}
	}
}
