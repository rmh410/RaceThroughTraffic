using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSpawnController : MonoBehaviour {

	public GameObject carPrefab;
	public float speed;
	public float minSpacing;
	public float maxSpacing;
	public GameObject stateContainer;

//	private LocalGameState stateContainer;
	private float lastSpawnTime;
	private float nextSpawnInterval;
	private bool first = true;

	// Use this for initialization
	void Start () {
		

//		stateContainer = GameObject.Find("LocalGameStateContainer").GetComponent<LocalGameState>();
	}
	
	// Update is called once per frame
	void Update () {
		// if gamestate hasnt been initialized do nothing
		if(stateContainer.GetComponent<GameState>().curState == "") {
			return;
		}

		if(stateContainer.GetComponent<GameState>().curState == "active") {
			if(first) {
				// pretend we spawned a car 3.1s ago so we might get one at outset
				lastSpawnTime = Time.fixedTime - 3.1f;
				nextSpawnInterval = Random.Range(minSpacing*3.1f/speed, maxSpacing*3.1f/speed);
				first = false;
			}
			// should we spawn a car?
			if(Time.fixedTime > lastSpawnTime + nextSpawnInterval) {
				// make one and set its speed and color
				GameObject newCar = Instantiate(carPrefab, this.transform.position, this.transform.rotation);
				newCar.GetComponent<CarController>().speed = speed;
				

				// reset our timing variables
				lastSpawnTime = Time.fixedTime;
				nextSpawnInterval = Random.Range(minSpacing*3.1f/speed, maxSpacing*3.1f/speed);
			}
		}
	}
}
