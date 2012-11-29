using UnityEngine;
using System.Collections;

public class creep : MonoBehaviour {

	//instantiated in prefab
	public int speed;
	public float health;
	public int weight;

	//instantiated upon creation
	public GameObject target;
	//public gameManager gm;


	// Use this for initialization
	void Start () {
		
	}

	// Update is called once per frame
	void Update () {
		
	}

	//called on hit
	public void hit (float damage) {
		health -= damage;
		Debug.Log(health);
		if (health < 0)
		{
			Destroy(this.gameObject);
		}
	}
}
