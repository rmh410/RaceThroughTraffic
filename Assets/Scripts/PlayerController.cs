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
	
    private bool serverClickFlag = false;
    private bool localPlayerClickFlag = false;
    private string waitForOpponent = "waiting for next round";
	private float speed;
	private string state;
	private float buttonTime;
	private float lastSpeed;
    private Vector3 initialPlayerPos;
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
            DisplayEndGameMessage();
		}

        if (isServer) {
            if (gameState != null) {
                if (gameState.curState == "active")
                    ServerCheckForWin();
                if (gameState.curState == waitForOpponent && gameState.playAgainCount == gameState.maxNumPlayers)
                    Invoke("ServerResetIfNextRound", 8.0f); // Start next round after n seconds
            }
        }
	}

    // -------------------- SETUP --------------------
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
    }

    private void SetupPlayAgainButton()
    {
        UnityEngine.UI.Button playAgainButton = GameObject.Find("Play Again Button").GetComponent<Button>();
        if (playAgainButton != null) {
            if (isServer) playAgainButton.onClick.AddListener(ServerOptIn); // Host change state
            if (isLocalPlayer) playAgainButton.onClick.AddListener(ResetGame); // Host and client reset position
            if (!isServer && isLocalPlayer) playAgainButton.onClick.AddListener(LocalPlayerOptIn); // Client opt in using [Command], prevent Host with !isServer
        } else {
            Debug.Log("Error: \'Play Again\' button not found.");
        }
    }

    // -------------------- GAMEPLAY --------------------
	private void ServerCheckForWin()
	{
		if (transform.position.z > winThreshold) {
			gameState.curState = "ended";
			gameState.winner = connectionToClient.connectionId;
		}
	}

    private void ServerResetIfNextRound()
    {
        serverClickFlag = false;
        localPlayerClickFlag = false;
        gameState.ServerReset();
    }

	private void DisplayEndGameMessage()
	{
		if (gameState == null) {
			return;
		}
		if (gameState.curState == "ended") {
			if (gameState.winner == connectionToServer.connectionId) {
				scoreCanvas.transform.GetChild(0).gameObject.SetActive(true); // Winner message
			} else {
                scoreCanvas.transform.GetChild(1).gameObject.SetActive(true); // Loser message
            }
            scoreCanvas.transform.GetChild(2).gameObject.SetActive(true); // Play Again button presented to both players
            SetupPlayAgainButton();
		}
	}

    // -------------------- PLAYER OPT IN --------------------
    private void ServerOptIn()
    {
        if (!serverClickFlag) {
            gameState.playAgainCount += 1;
            gameState.UpdateSpawn();
        }
        serverClickFlag = true; // Prevent multiple server opt in
    }

    // IMPORTANT: CmdLocalPlayerOptIn must be called from wrapper function LocalPlayerOptIn
    [Command]
    private void CmdLocalPlayerOptIn()
    {
        if (!localPlayerClickFlag) {
            gameState.playAgainCount += 1;
            gameState.UpdateSpawn();
        }
        localPlayerClickFlag = true; // Prevent multiple client opt in
    }

    private void LocalPlayerOptIn()
    {
        CmdLocalPlayerOptIn();
    }

	public void ResetGame()
    {
        gameState.curState = waitForOpponent;
        HideScoreCanvas();
        Debug.Log("spawning player at next spawn"+gameState.nextSpawn.ToString());
        transform.position = playerSpawns.transform.GetChild(gameState.nextSpawn).position; // Confirm with Reese
	}

    private void HideScoreCanvas()
    {
        GameObject scoreCanvas = GameObject.Find("Score Canvas");
        for (int i = 0; i < scoreCanvas.transform.childCount; i++) {
            scoreCanvas.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

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