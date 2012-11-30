using UnityEngine;
using System.Collections;

public class mainMenuManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI () {
		if (GUI.Button( new Rect((Screen.width/2)-70,(Screen.height/2)-70,140,140),"Play"))
		{
			Application.LoadLevel("main");
		}
		if (GUI.Button( new Rect((Screen.width/2)-70,(Screen.height/2)-280,140,140),"Quit"))
		{
			Application.Quit();
		}
	}
}
