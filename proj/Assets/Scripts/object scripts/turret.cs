using UnityEngine;
using System.Collections;

public class turret : MonoBehaviour {

	//instantiated through prefab
	public int identifier; 			//turret type
	public GameObject missileType;	//missile type created on fire
	public float rate;				//fire rate
	public float range;				//fire range

	//instantiated upon creation (gameManager.cs)
	//public gameManager gm;			//gets other object and game data

	//for updating
	float timeSinceLastFire;

	// Use this for initialization
	void Start () {
		timeSinceLastFire = rate + 1.0f;
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
		m.target = targetCreep;
	}

	// Fire is called through the gameManager's update, to easily supply the creep list and goal position
	// current AI: fire at creep closest to goalPosition and within range
	public void Fire (ArrayList creeps, Vector2 goalPos) {

		GameObject target = null;
		float minDist = 0;

		if(timeSinceLastFire > rate)
		{
			foreach(GameObject creep in creeps)
			{
				float creepDist = Mathf.Pow((creep.transform.position.x-this.transform.position.x),2.0f)+Mathf.Pow((creep.transform.position.z-creep.transform.position.z),2.0f);
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
			if(minDist != 0)
			{
				//Debug.Log(target);
				createMissile(target);
			}
			timeSinceLastFire = 0;
		}
	}
}
