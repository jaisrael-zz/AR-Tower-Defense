using UnityEngine;
using System.Collections;

public class victoryManager : MonoBehaviour {

	public GUIStyle titleStyle;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI () {
		GUI.Label(new Rect(0,0,Screen.width,100),"Victory!",titleStyle);
		if (GUI.Button( new Rect((Screen.width/2)-20,(Screen.height/2)-10,40,20),"Quit"))
		{
			Application.LoadLevel("StartMenu");
		}
	}
}
