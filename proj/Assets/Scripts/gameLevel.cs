using UnityEngine;
using System.Collections;

public class GameLevel : MonoBehaviour {

	void OnLevelWasLoaded(int level)
	{
		Texture fader = Resources.Load("empty") as Texture;
		Debug.Log(fader);
	}

/*	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}*/
}
