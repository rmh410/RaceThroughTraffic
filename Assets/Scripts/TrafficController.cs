using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficController : MonoBehaviour
{
	private ArrayList carSpawnPositions;
	private int numSpawnPoints = 4;

	void Start()
	{
		CollectSpawnPoints();
	}

	void Update()
	{

	}

	private void CollectSpawnPoints()
	{
		for (int i = 0; i < numSpawnPoints; i++) {
			Vector3 ithSpawnPos = GameObject.Find("Car Spawn " + (i + 1)).transform.position;
			carSpawnPositions.Add(ithSpawnPos);
		}
	}

	private void SpawnVehicle()
	{

	}
}
