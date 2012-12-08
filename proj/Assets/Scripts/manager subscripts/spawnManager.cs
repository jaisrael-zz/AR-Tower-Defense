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

	// Use this for initialization
	// Current wave generation is temporary
	void Start () {
		
		allWaves = new wave[totalWaves];

		allWaves[0].waveSize = 1;
		allWaves[0].creepIDs = new int[] {4};
		allWaves[0].spawnTimes = new float[] {0};

		allWaves[1].waveSize = 5;
		allWaves[1].creepIDs = new int[] {0,0,0,0,0};
		allWaves[1].spawnTimes = new float[] {0,2,4,6,8};

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
