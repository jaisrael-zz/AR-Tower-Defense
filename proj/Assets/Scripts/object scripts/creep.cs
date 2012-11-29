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
	public void hit (float damage) 
	{
		health -= damage;
		//Debug.Log(health);
		if (health < 0)
		{
			//GameObject maincam = GameObject.FindWithTag("Main");
			//gameManager gm = (gameManager)(maincam.GetComponent("gameManager"));
			//gm.creeps.Remove(this.gameObject);
			Destroy(this.gameObject);
		}
	}

	public void kamikaze()
	{
		GameObject g = GameObject.FindWithTag("Goal");
		goal gscript = (goal)g.GetComponent("goal");
		gscript.Hit(weight);
		Destroy(this.gameObject);
	}

	Vector2 convertToGridPos(Transform t)
	{
		return new Vector2(Mathf.Floor(t.position.x),Mathf.Floor(t.position.z));
	}

	//called on game update to determine orientation change
	//dim assumes the grid is a sqaure, I think that is safe to say right now
	public void Seek (Vector2 goal, bool[,] traversible, int dim)
	{
		float dt = Time.deltaTime;

		Vector2 pos = convertToGridPos(this.transform);

		if(pos.x == goal.x && pos.y == goal.y)
		{
			kamikaze();
		}

		//fail AI: move preference: right, up, left, down
		//note that creeps can totally get trapped by this even if there is an obvious path
		//don't switch the order of the boolean statements within each if, will cause
		//array out of bounds exception
		if((int)pos.x+1 < dim && traversible[(int)pos.x+1,(int)pos.y])
		{
			this.transform.position += new Vector3(speed*dt,0,0);
		}
		else if((int)pos.y+1<dim && traversible[(int)pos.x,(int)pos.y+1])
		{
			this.transform.position += new Vector3(0,0,speed*dt);
		}
		else if((int)pos.x-1>=0 && traversible[(int)pos.x-1,(int)pos.y])
		{
			this.transform.position += new Vector3(-speed*dt,0,0);
		}
		else if((int)pos.y-1 >= 0 && traversible[(int)pos.x,(int)pos.y-1])
		{
			this.transform.position += new Vector3(0,0,-speed*dt);
		}

	}
}
