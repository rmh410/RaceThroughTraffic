﻿using System.Collections;
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

	private void CollectSpawnPoints()
	{
		for (int i = 0; i < numSpawnPoints; i++) {
			Transform currentSpawnPoint = transform.GetChild(transform.childCount - 1).GetChild(i); // Get ith spawn point
			Vector3 ithSpawnPos = currentSpawnPoint.position;
			Quaternion ithSpawnRot = currentSpawnPoint.rotation;
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
