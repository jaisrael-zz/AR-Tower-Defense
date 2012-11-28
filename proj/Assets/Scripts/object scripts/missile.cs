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
	
	// Update is called once per frame
	void Update () {
		float dt = Time.deltaTime;

		
	}
}
