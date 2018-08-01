using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSpawnController : MonoBehaviour
{
	public float speed;
	public float minSpacing;
	public float maxSpacing;

    public GameObject carPrefab;
	public GameObject stateContainer;

    private bool first = true;
	private float lastSpawnTime;
	private float nextSpawnInterval;
	
	void Update () {
		// Do nothing if gamestate hasn't been initialized
		if (stateContainer.GetComponent<GameState>().curState == "") {
			return;
		}

		if (stateContainer.GetComponent<GameState>().curState == "active") {
			if (first) {
				// Pretend we spawned a car 3.1s ago so we might get one at outset
				lastSpawnTime = Time.fixedTime - 3.1f;
				nextSpawnInterval = Random.Range(minSpacing * 3.1f / speed, maxSpacing * 3.1f / speed);
				first = false;
			}
			
            // Check if should spawn car
			if (Time.fixedTime > lastSpawnTime + nextSpawnInterval) {
				// Create car
				GameObject newCar = Instantiate(carPrefab, this.transform.position, this.transform.rotation);
				newCar.GetComponent<CarController>().speed = speed;
				
				// Reset timing variables
				lastSpawnTime = Time.fixedTime;
				nextSpawnInterval = Random.Range(minSpacing*3.1f/speed, maxSpacing*3.1f/speed);
			}
		}
	}
}
