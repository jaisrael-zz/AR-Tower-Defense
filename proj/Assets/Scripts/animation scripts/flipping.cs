using UnityEngine;
using System.Collections;

public class flipping : MonoBehaviour {

	public float pitch;
	public float roll;
	public float yaw;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		float dt = Time.deltaTime;
		this.transform.Rotate(new Vector3(pitch,roll,yaw)*dt);
	}
}
