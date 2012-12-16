using UnityEngine;
using System.Collections;

public class mainMenuManager : GameLevel {

	public GUIStyle titleStyle;

	

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI () {

		GUI.Label(new Rect(0,0,Screen.width,100),"AR TOWER DEFENSE",titleStyle);

		if (GUI.Button( new Rect((Screen.width/2)-50,(Screen.height/2)-50,100,60),"play",titleStyle))
		{
			Application.LoadLevel("main");
		}
		if (GUI.Button( new Rect((Screen.width/2)-50,(Screen.height/2)+30,100,60),"quit",titleStyle))
		{
			Application.Quit();
		}
	}
}
