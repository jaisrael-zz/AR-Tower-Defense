using UnityEngine;
using System.Collections;

public enum selectedState
{
	none,
	tile,
	turret,
	creep
}



public class touchManager : MonoBehaviour {

	public selectedState selected;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0))
		{
			//take the pixel clicked in the screen image and convert it to a ray in world space
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hitInfo = new RaycastHit();
			//cast the ray and return true if it collided with some GameObject's collider component
			if (Physics.Raycast(ray, out hitInfo))
			{
				//when Physics.Raycast succeeds, it fill out the hitInfo object
				//get the GameObject that the collider belongs to
				GameObject g = hitInfo.collider.gameObject;
				//print out the GameObject's name

				if(g.name.Contains("tile")) selected = selectedState.tile;
				if(g.name.Contains("turret")) selected = selectedState.turret;
				if(g.name.Contains("creep")) selected = selectedState.creep;
			}
			else 
			{
				selected = selectedState.none;
			}
			Debug.Log(selected);
		}

	}
}
