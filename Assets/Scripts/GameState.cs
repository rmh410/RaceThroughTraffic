using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameState : NetworkBehaviour {

	[SyncVar]
	public string curState;

	[SyncVar]
	public int nextSpawn = 0;

	[SyncVar]
	public int winner;

    [SyncVar]
    public int playAgainCount = 0;

	public int maxNumPlayers;
	private CustomNetworkManager networkManager;
	
	void Start ()
    {
		if (isServer) {
			curState = "waiting";			
		}
		networkManager = GameObject.Find("Custom Network Manager").GetComponent<CustomNetworkManager>();
	}
	
	void Update ()
    {
		// If correct number of players is met while waiting
		if (isServer && (networkManager.numPlayers == maxNumPlayers) && (curState == "waiting")) {
			curState = "active"; // Start match
		}
	}

	public void UpdateSpawn()
    {
		nextSpawn = (nextSpawn + 1) % maxNumPlayers;
	}

    public void ServerReset()
    {
        winner = 0;
        nextSpawn = 0;
        curState = "active";
        playAgainCount = 0;
        // isWinnerDetermined = false;
    }
}
