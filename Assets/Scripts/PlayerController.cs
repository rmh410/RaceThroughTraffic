using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour
{
	public float maxSpeed;
	public float accelTime;
    public float winThreshold;
    public GameObject viewer;
	public GameObject stateContainer;
	public GameObject playerSpawns;
	public GameObject scoreCanvas;
	public GameObject matchmakingCanvas;
	public GameState gameState;
	
	private float speed;
	private string state;
	private float buttonTime;
	private float lastSpeed;
    private bool isResetButtonSetup = false;
    private RaceNetworkDiscovery networkDiscovery;

	public override void OnStartLocalPlayer()
	{
        SetupPlayer();
        SetupCamera();
        SetupPlayerSpawns();
	}

	void Update()
	{
		// Get GameState
		if (stateContainer == null) {
			stateContainer = GameObject.Find("GameState Container");
		} else if (gameState == null) {
			gameState = stateContainer.GetComponent<GameState>();
		}

		if (isLocalPlayer) {
			MovePlayer();
			ScoreUpdate();
		}

		if (gameState != null && gameState.curState == "active") {
			if (isServer) {
				CheckForWin();
			}
		}
	}

    // -------------------- Initialize --------------------
    private void SetupPlayer()
    {
        speed = 0.0f;
        state = "still";
        lastSpeed = 0.0f;
        buttonTime = -5.0f;
    }

    private void SetupCamera()
    {
        Vector3 viewerDest = this.transform.position;
        viewer = Camera.main.transform.parent.gameObject;
        viewer.transform.position = viewerDest;
        viewerDest.y = viewerDest.y + 0.4f;
    }

    private void SetupPlayerSpawns()
    {
        scoreCanvas = GameObject.Find("Score Canvas");
        playerSpawns = GameObject.Find("Player Spawns");
        matchmakingCanvas = GameObject.Find("Matchmaking Canvas");
        networkDiscovery = GameObject.Find("Network Discovery").GetComponent<RaceNetworkDiscovery>();
        Debug.Log("Network Discovery is " + networkDiscovery);
    }

    // -------------------- Gameplay --------------------
    // Server checks position and determines if there is winner
	void CheckForWin()
	{
		if (transform.position.z > winThreshold) {
			gameState.curState = "ended";
			gameState.winner = connectionToClient.connectionId;
			// Debug.Log("Winner's connection ID is: " + gameState.winner);
		}
	}

	void ScoreUpdate()
	{
		if (gameState == null) {
			return;
		}
		if (gameState.curState == "ended") {
			if (gameState.winner == connectionToServer.connectionId) {
				scoreCanvas.transform.GetChild(0).gameObject.SetActive(true); // Winner
				scoreCanvas.transform.GetChild(2).gameObject.SetActive(true); // Reset game option is presented to winner
                if (!isResetButtonSetup) {
                    SetupResetButton();
                }
			} else {
				scoreCanvas.transform.GetChild(1).gameObject.SetActive(true); // Loser
			}
		}
	}

    private void SetupResetButton()
    {
        UnityEngine.UI.Button resetButton = GameObject.Find("Reset Button").GetComponent<Button>();
        if (resetButton != null) {
            resetButton.onClick.AddListener(ResetGame);
            isResetButtonSetup = true;
        } else {
            Debug.Log("Error: reset button not found");
        }
    }

	public void ResetGame()
    {
        TearDownScoreCanvas();
        networkDiscovery.ResetClient(); // Does run
        Camera.main.transform.parent.position = new Vector3(0, 20, -70);
        // resest matchmaking canvas
        matchmakingCanvas.SetActive(true);
        matchmakingCanvas.transform.GetChild(0).gameObject.SetActive(true);
        matchmakingCanvas.transform.GetChild(1).gameObject.SetActive(true);

        if (isServer) {
            Debug.Log("Hello from StopHost");
            NetworkManager.singleton.StopHost(); // Does run
            gameState.curState = "waiting";
            gameState.nextSpawn = 0;
            gameState.winner = 0;
        } else {
            Debug.Log("Hello from StopClient");
            NetworkManager.singleton.StopClient(); // Not sure but probably runs
        }   
	}

    private void TearDownScoreCanvas()
    {
        GameObject scoreCanvas = GameObject.Find("Score Canvas");
        for (int i = 0; i < scoreCanvas.transform.childCount; i++) {
            scoreCanvas.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    // Do we need Cmd prefix here?
	void MovePlayer()
	{
		Camera cam = Camera.main;
		transform.eulerAngles = new Vector3(0, Camera.main.transform.eulerAngles.y, 0); // Set rotation

		// On click
		if (Input.GetButtonDown("Fire1")) {
			buttonTime = Time.fixedTime;
			state = "accelerating";
		}

		// If accelerating
		if (state == "accelerating") {
            // Move forward at speed depending on time of button press
			speed = maxSpeed * (Time.fixedTime - buttonTime) / accelTime;
			Vector3 dest = this.transform.position + speed * cam.transform.forward * Time.deltaTime;
			dest.y = this.transform.position.y; // Slice out y component
			this.transform.position = dest; // Update position

			// If its been three seconds go to gliding state
			if (Time.fixedTime - buttonTime >= accelTime) {
				state = "gliding";
			}
		} else if (state == "gliding") {
			// Move forward, slice out y component
			Vector3 dest = this.transform.position + maxSpeed * cam.transform.forward * Time.deltaTime;
			dest.y = this.transform.position.y;
			this.transform.position = dest;
		}

		// On button release
		if ((state == "accelerating" || state == "gliding") && Input.GetButtonUp("Fire1")) {
			state = "decelerating";
			buttonTime = Time.fixedTime;
			lastSpeed = speed;
		}

		if (state == "decelerating") {
			// Move forward at speed depending on time of button release
			speed = (accelTime - (Time.fixedTime - buttonTime)) / accelTime * lastSpeed;
			// Slice out y component
			Vector3 dest = this.transform.position + speed * cam.transform.forward * Time.deltaTime;
			dest.y = this.transform.position.y;
			this.transform.position = dest;
			// If its been three seconds stop
			if (Time.fixedTime - buttonTime >= accelTime) {
				state = "still";
				speed = 0.0f;
			}
		}
		// Now move the camera
		Vector3 viewerDest = this.transform.position;
		viewerDest.y = viewerDest.y + 0.4f;
		viewer.transform.position = viewerDest;
	}

	void OnCollisionEnter(Collision collision)
	{
		// Client should move to the next spawn point
		if (isLocalPlayer) {
			transform.position = playerSpawns.transform.GetChild(gameState.nextSpawn).position;
		}

		// Server should update the next spawn point for round robin
		if (isServer) {
			gameState.UpdateSpawn();
			return;
		}
	}
}
