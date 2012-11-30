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
	public GameObject selectedObject;

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
				selectedObject = hitInfo.collider.gameObject;
				//print out the GameObject's name
				//Debug.Log(selectedObject.transform.parent.gameObject.tag);

				if(selectedObject.transform.parent.gameObject.tag == "Tile") selected = selectedState.tile;
				if(selectedObject.transform.parent.gameObject.tag == "Turret") selected = selectedState.turret;
				if(selectedObject.transform.parent.gameObject.tag == "Creep") selected = selectedState.creep;
			}
			else 
			{
				selected = selectedState.none;
			}
			
		}
		//Debug.Log(selected);
	}
}
