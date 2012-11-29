using UnityEngine;
using System.Collections;

public class missile : MonoBehaviour {

	//instantiated in prefab
	public float speed;  //particle speed
	public float damage; //damage at center
	public float radius; //damage radius
	public float att;	 //damage attenuation across area of effect

	//instantiated upon creation
	public Transform target;
	//public gameManager gm;

	// Use this for initialization
	void Start () {
	
	}
	
	void onHit () {
		GameObject[] creeps = GameObject.FindGameObjectsWithTag("Creep");
		foreach(GameObject creep in creeps)
		{
			float dFromCenter = Vector3.Distance(creep.transform.position,this.transform.position);
			Debug.Log(dFromCenter);
		}
			

		Destroy(this.gameObject);
	}

	// Update is called once per frame
	void Update () {
		float dt = Time.deltaTime;

		Vector3 distance = target.position - this.transform.position;
		Vector3 direction = distance/distance.magnitude;
		Vector3 newPos = (direction * dt * speed);

		if(newPos.magnitude > distance.magnitude) 	onHit();
		else 	this.transform.position = this.transform.position + newPos;
	}
}
