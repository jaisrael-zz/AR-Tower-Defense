using UnityEngine;
using System.Collections;

public enum creepStatus
{
	stun,
	slow,
	burn
}

public class creep : MonoBehaviour {

	//instantiated in prefab
	public int speed;
	public float health;
	public int weight;
	public Texture image;

	public float stunDurationMultiplier;
	public float slowDurationMultiplier;
	public float burnDurationMultiplier;
	int currentIndex;

	

	//instantiated upon creation
	public GameObject target;
	//public gameManager gm;

	private bool[] currentStatuses;

	// Use this for initialization
	void Start () {
		currentIndex = 0;
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
			Destroy(this.gameObject);
		}
	}

	public void kamikaze()
	{
		GameObject g = GameObject.FindWithTag("Goal");
		if(g != null) 
		{
			goal gscript = (goal)g.GetComponent("goal");
			gscript.Hit(weight);
		}
		Destroy(this.gameObject);
	}

	Vector2 applyStatus(Vector2 raw)
	{
		return raw;
	}

	//public void accountForStatus(Vector3 movement,)

	//called on game update to determine orientation change
	//dim assumes the grid is a sqaure, I think that is safe to say right now
	public void Seek (Vector2 goal, ArrayList path, int dim)
	{
		float dt = Time.deltaTime;
		Vector2 pos = new Vector2(this.transform.position.x,this.transform.position.z);

		if(pos.x == goal.x && pos.y == goal.y)
		{
			kamikaze();
		}

		Vector2 currentTarget = (Vector2)path[currentIndex];
		Vector2 rawMovement = (new Vector2(currentTarget.x-pos.x,currentTarget.y-pos.y));
		Vector2 direction = rawMovement.normalized;
		Vector2 movement = applyStatus(direction);
		movement = new Vector2(movement.x*speed*dt,movement.y*speed*dt);

		if(rawMovement.magnitude <= movement.magnitude)
		{
			this.transform.position = new Vector3(currentTarget.x,this.transform.position.y,currentTarget.y);
			
			currentIndex++;
			if(currentIndex == path.Count) kamikaze();
		}
		this.transform.position += new Vector3(movement.x,0,movement.y);

	}
}
