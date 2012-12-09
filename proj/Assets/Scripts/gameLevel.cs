using UnityEngine;
using System.Collections;

public class GameLevel : MonoBehaviour {

	Texture2D fader;
	int alpha;

	void OnLevelWasLoaded(int level)
	{
		fader = Resources.Load("empty") as Texture2D;
	}

	// Use this for initialization
	void Start () {
		
	}

	// Update is called once per frame
	void Update () {
		
	}

	void OnGUI () {
		//if(fader != null)
		//	GUI.DrawTexture(new Rect(0,0,Screen.width,Screen.height),fader,ScaleMode.ScaleToFit,false,0);
	}
}
