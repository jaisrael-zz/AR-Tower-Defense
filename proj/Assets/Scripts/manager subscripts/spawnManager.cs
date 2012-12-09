using UnityEngine;
using System.Collections;

//accessing the same index in both arrays 
//gives a time to spawn a type of creep
public struct wave {
	public int waveSize;
	public int[] creepIDs;
	public float[] spawnTimes;
}

public class spawnManager : MonoBehaviour {

	public Vector2 spawnPos;
	public int totalWaves;
	public gameManager gm;

	public int currentWave;
	public int currentWaveIndex;
	public float totaldt;
	private bool waveFinishedSpawning;

	public wave[] allWaves;

	//somewhat arbitrary; will be balanced if needed
	public int creepToInfluenct(int type)
	{
		//loosely: creep health*creep speeds
		switch(type)
		{
			case 0: return 5;
			case 1: return 9;
			case 2: return 20;
			case 3: return 8;
			case 4: return 15;
			default: break;
		}
		return 1;
	}

	public bool isWaveDefeated()
	{
		if(waveFinishedSpawning && GameObject.FindWithTag("Creep") == null)
		{
			//prepare for next wave
			currentWave++;
			currentWaveIndex = 0;
			totaldt = 0;
			waveFinishedSpawning = false;

			return true;
		}

		return false;
	}

	//starts of close to 2 sec/creep, tends toward .5 sec/creep
	float waveSpawnFunction(float waveIndex)
	{
		return (1/((waveIndex+5.0f)/8.0f))+0.5f;
	}

	//increases wave length by 2 each wave
	int waveLengthFunction(int waveIndex)
	{
		return 1+(waveIndex*2);
	}

	//returns a creep id randomly based off of a probability function
	//for each group of waves
	int idSpawnFunction(float waveIndex)
	{
		float rand = UnityEngine.Random.value;
		if(waveIndex < 5) {
			return (int)creepType.basic;
		} else if(waveIndex < 10) {
			if 		(rand < .33f)	return (int)creepType.basic;
			else if (rand < .67f) 	return (int)creepType.quick;
			else 					return (int)creepType.strong;
		} else if(waveIndex < 15) {
			if 		(rand < .20f)	return (int)creepType.basic;
			else if (rand < .40f) 	return (int)creepType.quick;
			else if (rand < .60f) 	return (int)creepType.quickStatus;
			else if (rand < .80f) 	return (int)creepType.strong;
			else 					return (int)creepType.strongStatus;
		} else if(waveIndex < 20) {
			if 		(rand < .10f)	return (int)creepType.basic;
			else if (rand < .40f)	return (int)creepType.quick;
			else if (rand < .60f) 	return (int)creepType.quickStatus;
			else if (rand < .75f)	return (int)creepType.strong;
			else 					return (int)creepType.strongStatus;	
		} else {
			if 		(rand < .02f)	return (int)creepType.basic;
			else if (rand < .30f)	return (int)creepType.quick;
			else if (rand < .60f) 	return (int)creepType.quickStatus;
			else if (rand < .75f)	return (int)creepType.strong;
			else 					return (int)creepType.strongStatus;
		}
	}

	void Start () {
		
		allWaves = new wave[totalWaves];

		for(int i = 0; i < totalWaves; i++)
		{
			//UnityEngine.Random.seed = (int)Time.time;
			allWaves[i].waveSize = waveLengthFunction(i);
			allWaves[i].creepIDs = new int[allWaves[i].waveSize];
			allWaves[i].spawnTimes = new float[allWaves[i].waveSize];
			float offset = waveSpawnFunction((float)i);
			for(int j = 0; j < allWaves[i].waveSize; j++)
			{
				allWaves[i].creepIDs[j] = idSpawnFunction((float)i);
				allWaves[i].spawnTimes[j] = ((float)j)*offset;
			}
		}

		currentWave = 0;
		currentWaveIndex = 0;
		totaldt = 0;
		waveFinishedSpawning = false;

	}
	
	// Update is called once per frame
	// normally we would use this function, but we only sometimes want 
	// the spawn manager to update (when it is battlephase)
	void Update () {
	
	}

	//called once per frame during battle phase
	public void UpdateSpawn () {
		totaldt += Time.deltaTime;
		if(!waveFinishedSpawning)
		{
			if(totaldt >= allWaves[currentWave].spawnTimes[currentWaveIndex])
			{
				gm.createCreep(spawnPos,(creepType)allWaves[currentWave].creepIDs[currentWaveIndex]);
				currentWaveIndex++;
				if (currentWaveIndex == allWaves[currentWave].waveSize)
				{
					waveFinishedSpawning = true;
				}
			}
		}
	}
}
