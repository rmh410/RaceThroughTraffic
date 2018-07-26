using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficController : MonoBehaviour
{
	// Spawning
	private int numSpawnPoints = 4;
	private Vector3[] carSpawnPositions;
	private Quaternion[] carSpawnRotations;
	public GameObject carPrefab;

	void Start()
	{
		carSpawnPositions = new Vector3[numSpawnPoints];
		carSpawnRotations = new Quaternion[numSpawnPoints];
		CollectSpawnPoints();
		SpawnVehicles();
	}

	void Update()
	{

	}

	private void CollectSpawnPoints()
	{
		for (int i = 0; i < numSpawnPoints; i++) {
			Vector3 ithSpawnPos = GameObject.Find("Car Spawn " + (i + 1)).transform.position;
			Quaternion ithSpawnRot = GameObject.Find("Car Spawn " + (i + 1)).transform.rotation;
			carSpawnPositions[i] = ithSpawnPos;
			carSpawnRotations[i] = ithSpawnRot;
		}
	}

	private void SpawnVehicles()
	{
		for (int i = 0; i < numSpawnPoints; i++) {
			Instantiate(carPrefab, carSpawnPositions[i], carSpawnRotations[i]);
		}
	}
}
