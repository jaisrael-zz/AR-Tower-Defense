using UnityEngine;
using System.Collections;

public enum turretAI
{
	first,
	strongest,
	weakest,
	spread
}

public class turret : MonoBehaviour {

	//instantiated through prefab
	public int identifier; 			//turret type
	public GameObject missileType;	//missile type created on fire
	public float rate;				//fire rate
	public float range;				//fire range
	public int cost;				//turret cost
	public int ai;					//turret ai

	//for updating
	float timeSinceLastFire;

	// Use this for initialization
	void Start () {
		timeSinceLastFire = rate + 1.0f;
		ai = 0;
	}
	
	// Update is called once per frame
	void Update () {
		float dt = Time.deltaTime;
		timeSinceLastFire += dt;
	}

	void createMissile(GameObject targetCreep)
	{
		GameObject newMissile = (GameObject)Instantiate(missileType,this.transform.position,this.transform.rotation);
		missile m = (missile)newMissile.GetComponent("missile");
		newMissile.tag = "Missile";
		m.target = targetCreep.transform;
	}

	public GameObject fireFirst(Vector2 goalPos)
	{
		GameObject target = null;
		float minDist = 0;
		foreach(GameObject creep in GameObject.FindGameObjectsWithTag("Creep"))
		{
			float creepDist = Mathf.Pow((creep.transform.position.x-this.transform.position.x),2.0f)+Mathf.Pow((creep.transform.position.z-this.transform.position.z),2.0f);
			if(creepDist < (Mathf.Pow(range,2)))
			{
				float targetDist = Mathf.Pow((creep.transform.position.x-goalPos.x),2.0f)+Mathf.Pow((creep.transform.position.z-goalPos.y),2.0f);
				//we assign a new target if it is closer to the goal or we don't have a target yet
				if((targetDist < minDist) || minDist == 0)
				{
					target = creep;
					minDist = targetDist;
				}
			}
		}
		return target;
	}

	public GameObject fireWeak()
	{
		GameObject target = null;
		float minHealth = 0;
		foreach(GameObject creep in GameObject.FindGameObjectsWithTag("Creep"))
		{
			float creepDist = Mathf.Pow((creep.transform.position.x-this.transform.position.x),2.0f)+Mathf.Pow((creep.transform.position.z-this.transform.position.z),2.0f);
			if(creepDist < (Mathf.Pow(range,2)))
			{
				creep c = (creep)creep.GetComponent("creep");
				float targetHealth = c.health;
				//we assign a new target if it is closer to the goal or we don't have a target yet
				if((targetHealth < minHealth) || minHealth == 0)
				{
					target = creep;
					minHealth = targetHealth;
				}
			}
		}
		return target;
	}

	public GameObject fireStrong()
	{
		GameObject target = null;
		float maxHealth = 0;
		foreach(GameObject creep in GameObject.FindGameObjectsWithTag("Creep"))
		{
			float creepDist = Mathf.Pow((creep.transform.position.x-this.transform.position.x),2.0f)+Mathf.Pow((creep.transform.position.z-this.transform.position.z),2.0f);
			if(creepDist < (Mathf.Pow(range,2)))
			{
				creep c = (creep)creep.GetComponent("creep");
				float targetHealth = c.health;
				//we assign a new target if it is closer to the goal or we don't have a target yet
				if((targetHealth > maxHealth) || maxHealth == 0)
				{
					target = creep;
					maxHealth = targetHealth;
				}
			}
		}
		return target;
	}

	public GameObject fireSpread(Vector2 goalPos)
	{
		GameObject target = null;
		float minDist = 0;
		bool minStatus = true;
		foreach(GameObject creep in GameObject.FindGameObjectsWithTag("Creep"))
		{
			float creepDist = Mathf.Pow((creep.transform.position.x-this.transform.position.x),2.0f)+Mathf.Pow((creep.transform.position.z-this.transform.position.z),2.0f);
			if(creepDist < (Mathf.Pow(range,2)))
			{
				float targetDist = Mathf.Pow((creep.transform.position.x-goalPos.x),2.0f)+Mathf.Pow((creep.transform.position.z-goalPos.y),2.0f);
				creep c = (creep)creep.GetComponent("creep");
				bool hasStatus = false;
				for(int i = 1; i < c.currentStatuses.Length; i++)
				{
					if(c[i] > 0)
					{
						hasStatus = true;
						break;
					}
				}
				//we assign a new target if it is closer to the goal or we don't have a target yet
				if((minStatus && !hasStatus) || ((minStatus || (!minStatus && !hasStatus)) && (targetDist < minDist)) || minDist == 0)
				{
					target = creep;
					minDist = targetDist;
					minStatus == hasStatus;
				}
			}
		}
		return target;
	}

	// Fire is called through the gameManager's update, to easily supply the creep list and goal position
	// current AI: fire at creep closest to goalPosition and within range
	public void Fire (Vector2 goalPos) {

		GameObject target = null;

		if(timeSinceLastFire > rate)
		{
			switch((turretAI)ai)
			{
				case turretAI.first: 		target = fireFirst(goalPos); break;
				case turretAI.strongest: 	target = fireStrong(); break;
				case turretAI.weakest: 		target = fireWeak(); break;
				default:					target = fireSpread(goalPos); break;
			}

			if(target != null)
			{
				//Debug.Log(target);
				this.transform.LookAt(target.transform.position);
				this.transform.Rotate(0,180,0);
				createMissile(target);
			}
			timeSinceLastFire = 0;
		}
	}
}
