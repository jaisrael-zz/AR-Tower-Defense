using UnityEngine;
using System.Collections;

public enum creepStatus
{
	none,
	stun,
	slow,
	burn,
	count
}

public class creep : MonoBehaviour {

	//instantiated in prefab
	public int speed;
	public float health;
	public int weight;

	int currentIndex;

	public int[] currentStatuses;
	public float[] effectMultipliers;
	public int[] durationMultipliers;

	//instantiated upon creation
	public GameObject target;

	// Use this for initialization
	void Start () {
		currentIndex = 0;
		currentStatuses = new int[(int)creepStatus.count];
		durationMultipliers = new int[(int)creepStatus.count];
		for(int i = 0; i < (int)creepStatus.count; i++)
		{
			currentStatuses[i] = 0;
			durationMultipliers[i] = 1;
		}
	}

	// Update is called once per frame
	void Update () {
		
	}

	//called on hit
	public void hit (float damage) 
	{
		health -= damage;
		Debug.Log(health);
		//Debug.Log(health);
		if (health <= 0)
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

	public void applyStatus(creepStatus status,int frames)
	{

		if(currentStatuses[(int)status] < frames*durationMultipliers[(int)status])
		{
			currentStatuses[(int)status] = frames*durationMultipliers[(int)status];
		}
	}

	Vector2 accountForStatus(Vector2 raw)
	{
		for(int i = 0; i < currentStatuses.Length; i++)
		{
			if(currentStatuses[i] > 0)
			{
				switch(i)
				{
					case (int)creepStatus.stun: 
						raw = new Vector2(0,0);
						break;
					case (int)creepStatus.slow:
						raw = new Vector2(raw.x*0.5f*effectMultipliers[i],raw.y*0.5f*effectMultipliers[i]);
						break;
					case (int)creepStatus.burn:
						hit(0.01f*effectMultipliers[i]);
						break;
					default:
						break;
				}
			}
		}
		return raw;
	}

	public void updateStatuses()
	{
		for(int i = 0; i < currentStatuses.Length; i++)
			if(currentStatuses[i] > 0)
				currentStatuses[i]--;
	}

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
		Vector2 movement = accountForStatus(direction);
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
