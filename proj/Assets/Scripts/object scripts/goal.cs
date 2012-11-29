using UnityEngine;
using System.Collections;

public class goal : MonoBehaviour {

	public int health;

	public void Hit (int damage) {
		health -= damage;
		if (health <= 0)
		{
			Destroy(this.gameObject);
		}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
