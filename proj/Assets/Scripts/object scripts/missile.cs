using UnityEngine;
using System.Collections;

public class missile : MonoBehaviour {

	//instantiated in prefab
	public float speed;  //particle speed
	public float damage; //damage at center
	public float radius; //damage radius
	public float att;	 //damage attenuation across area of effect
	public int statusEffect;
	public int potency;

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
			if (creep != null)
			{
				float dFromCenter = Vector3.Distance(creep.transform.position,this.transform.position);
				//Debug.Log(dFromCenter);
				if (dFromCenter < radius)
				{
					//splash removed
					//float splashDamage = ((radius - dFromCenter)/radius)*damage;
					creep c = (creep)creep.GetComponent("creep");
					c.hit(damage);
					c.applyStatus((creepStatus)statusEffect,potency);
				}
			}
		}
			

		Destroy(this.gameObject);
	}

	// Update is called once per frame
	void Update () {

	}

	//only done this way to make sure that missiles only fly during battle phase
	public void Fly() {
		float dt = Time.deltaTime;
		if(target == null) Destroy(this.gameObject);
		else {
			Vector3 distance = target.position - this.transform.position;
			Vector3 direction = distance/distance.magnitude;
			Vector3 newPos = (direction * dt * speed);
			this.transform.LookAt(target.position);
			this.transform.Rotate(new Vector3(0,180,0),Space.Self);

			if(newPos.magnitude > distance.magnitude) 	onHit();
			else 	this.transform.position = this.transform.position + newPos;
		}
	}
}
