using UnityEngine;
using System.Collections;

public class CameraMove : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{
	
	}
	float moveSpeed = 10.0f;
	private float z;
	private float x;
	// Update is called once per frame
	void Update () 
	{
		z = Input.GetAxis("Vertical") * Time.deltaTime * moveSpeed;
		x = Input.GetAxis("Horizontal") * Time.deltaTime * moveSpeed;

		this.transform.Translate(x, 0, z);
	}
}
