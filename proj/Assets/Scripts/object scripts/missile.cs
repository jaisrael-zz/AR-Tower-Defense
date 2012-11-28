using UnityEngine;
using System.Collections;

public class missile : MonoBehaviour {

	//instantiated in prefab
	public float speed;
	public float damage;

	//instantiated upon creation
	public GameObject target;
	//public gameManager gm;

	// Use this for initialization
	void Start () {
	
	}
	
	void onHit () {
		//send message to turret
		Destroy(this.gameObject);
	}

	// Update is called once per frame
	void Update () {
		float dt = Time.deltaTime;

		Vector3 distance = target.transform.position - this.transform.position;
		Vector3 direction = distance/distance.magnitude;
		Vector3 newPos = (direction * dt * speed);

		if(newPos.magnitude > distance.magnitude) 	onHit();
		else 	this.transform.position = this.transform.position + newPos;
	}
}
