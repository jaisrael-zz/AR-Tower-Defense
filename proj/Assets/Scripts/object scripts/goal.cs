using UnityEngine;
using System.Collections;

public class goal : MonoBehaviour {

	public int health;

	public void Hit (int damage) {
		health -= damage;
		if (health <= 0)
		{
			GameObject main = GameObject.FindWithTag("MainCamera");
			if(main != null)
			{
				gameManager gm = (gameManager)main.GetComponent("gameManager");
				gm.lose();
			}
			else Debug.Log("very bad news, friend");
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
