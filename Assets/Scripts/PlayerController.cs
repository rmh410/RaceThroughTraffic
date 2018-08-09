using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour {

	public GameObject viewer;
	public float maxSpeed;
	public float accelTime;
	public GameObject stateContainer;
	public GameObject playerSpawns;
	public GameObject scoreCanvas;
	public GameState gameState;
	public float winThreshold;

	private float speed;
	private string state;
	private float buttonTime;
	private float lastSpeed;

	// Use this for initialization
	public override void OnStartLocalPlayer () {
		// player setup
		state = "still";
		buttonTime = -5.0f;
		speed = 0.0f;
		lastSpeed = 0.0f;

		// set up camera and move to us
		viewer = Camera.main.transform.parent.gameObject;
		Vector3 viewerDest = this.transform.position;
		viewerDest.y = viewerDest.y+0.4f;
		viewer.transform.position = viewerDest;

		// set player spawns
		playerSpawns = GameObject.Find("Player Spawns");
		scoreCanvas = GameObject.Find("Score Canvas");
		
	}
	
	// Update is called once per frame
	void Update () {

		// get the game state
		if(stateContainer == null) {
			stateContainer = GameObject.Find("GameState Container");
		}
		else if (gameState == null) {
			gameState = stateContainer.GetComponent<GameState>();
		}

		if(isLocalPlayer) {
			MovePlayer();
			ScoreUpdate();
		}

		if (gameState != null && gameState.curState == "active") {
			if(isServer) {
			CheckForWin();
			}
		}
		
	}

	// server should be checking position and updating accordingly if there is one
	void CheckForWin() {
		// is the player past the win point?
		if(transform.position.z > winThreshold) {
			gameState.curState = "ended";
			gameState.winner = connectionToClient.connectionId;
			Debug.Log(gameState.winner);
		}
	}

	void ScoreUpdate() {
		if(gameState == null) {
			return;
		}
		if(gameState.curState == "ended") {
			if(gameState.winner == connectionToServer.connectionId) {
				scoreCanvas.transform.GetChild(0).gameObject.SetActive(true);
			}
			else {
				scoreCanvas.transform.GetChild(1).gameObject.SetActive(true);
			}
		}
	}

	void MovePlayer() {
		Camera cam = Camera.main;

		// set rotation
		transform.eulerAngles = new Vector3(0, Camera.main.transform.eulerAngles.y, 0);

		// when we click
		if(Input.GetButtonDown("Fire1")) {
			buttonTime = Time.fixedTime;
			state = "accelerating";
		}

		// if accelerating
		if(state == "accelerating") {
			// move forward at speed depending on time of button press
			speed = maxSpeed * (Time.fixedTime - buttonTime)/accelTime;
			// slice out y component
			Vector3 dest = this.transform.position + speed * cam.transform.forward * Time.deltaTime;
			dest.y = this.transform.position.y;
			// update position
			this.transform.position = dest;
			// if its been three seconds go to gliding state
			if(Time.fixedTime - buttonTime >= accelTime) {
				state = "gliding";
			}
			
		}

		else if (state == "gliding") {
			// move forward
			// slice out y component
			Vector3 dest = this.transform.position + maxSpeed * cam.transform.forward * Time.deltaTime;
			dest.y = this.transform.position.y;
			this.transform.position = dest;
		}

		// when we release button
		if((state == "accelerating" || state == "gliding") && Input.GetButtonUp("Fire1")) {

			state =	 "decelerating";
			buttonTime = Time.fixedTime;
			lastSpeed = speed;
		}

		if(state == "decelerating") {
			// move forward at speed depending on time of button release
			speed = (accelTime - (Time.fixedTime - buttonTime))/accelTime * lastSpeed;
			// slice out y component
			Vector3 dest = this.transform.position + speed * cam.transform.forward * Time.deltaTime;
			dest.y = this.transform.position.y;
			this.transform.position = dest;
			// if its been three seconds stop
			if(Time.fixedTime - buttonTime >= accelTime) {
				state = "still";
				speed = 0.0f;
			}
		}
		// now move the camera
		Vector3 viewerDest = this.transform.position;
		viewerDest.y = viewerDest.y + 0.4f;
		viewer.transform.position = viewerDest;

		// constrain motion to 
		if (gameState.curState == "waiting") {
			if(transform.position.z > -30.0f) {
				transform.position.z = -30.0f;
			}
		}
	}

	void OnCollisionEnter(Collision collision) {
		// client should move to the next spawn point
		if(isLocalPlayer) {
			transform.position = playerSpawns.transform.GetChild(gameState.nextSpawn).position;

		}

		// server should update the next spawn point for round robin
		if(isServer) {
			gameState.UpdateSpawn();
			return;
		}
	}
}
