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
	public Vector2 selectedPos;
	public bool clickable;

	// Use this for initialization
	void Start () {
		clickable = true;
	} 
	
	// Update is called once per frame
	void Update () {
		if (clickable) {
			if (Input.GetMouseButtonDown(0))
			{
				//take the pixel clicked in the screen image and convert it to a ray in world space
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				selectedPos = Input.mousePosition;
				RaycastHit hitInfo = new RaycastHit();
				//cast the ray and return true if it collided with some GameObject's collider component
				if (Physics.Raycast(ray, out hitInfo))
				{
					//we know that if the ray collides with something we want, it is the component
					//of an overall object (ie the ray hits turretBody, part of a Turret gameObject)
					if(selected == selectedState.none) selectedObject = hitInfo.collider.gameObject.transform.parent.gameObject;

					if(selectedObject.tag == "Tile" && selected == selectedState.none) {
						selected = selectedState.tile;
						clickable = false;
					}
					if(selectedObject.tag == "Turret" && selected == selectedState.none) {
						selected = selectedState.turret;
						//clickable = false;
					}
					if(selectedObject.tag == "Creep" && selected == selectedState.none) selected = selectedState.creep;
				}
				else 
				{
					selected = selectedState.none;
				}
				
			}
		}
		//Debug.Log(selected);
	}
}
