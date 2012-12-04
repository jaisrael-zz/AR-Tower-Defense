using UnityEngine;
using System.Collections;

public class floating : MonoBehaviour {

	public float amplitude;
	//public float period; 
	private float theta;

	// Use this for initialization
	void Start () {
		theta = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {
		float dt = Time.deltaTime;
		theta += dt;
		this.transform.Translate(0,amplitude*dt*Mathf.Cos(theta),0); 
	}
}
