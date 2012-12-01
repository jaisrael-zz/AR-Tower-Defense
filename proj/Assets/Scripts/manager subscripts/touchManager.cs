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
				//we know that if the ray collides with something we want, it is the component
				//of an overall object (ie the ray hits turretBody, part of a Turret gameObject)
				selectedObject = hitInfo.collider.gameObject.transform.parent.gameObject;

				if(selectedObject.tag == "Tile") selected = selectedState.tile;
				if(selectedObject.tag == "Turret") selected = selectedState.turret;
				if(selectedObject.tag == "Creep") selected = selectedState.creep;
			}
			else 
			{
				selected = selectedState.none;
			}
			
		}
		//Debug.Log(selected);
	}
}
