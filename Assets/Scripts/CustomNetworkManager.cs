using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CustomNetworkManager : NetworkManager {

	private RaceNetworkDiscovery networkDiscovery;
	private GameState gameState;

	public override void OnStartServer () {
		networkDiscovery = GameObject.Find("Network Discovery").GetComponent<RaceNetworkDiscovery>();
	}

	// server action when a client connects
	public override void OnServerConnect(NetworkConnection conn) {
		/* we need to make sure the gamestatemanager exists, so we'll just assign its value
		when the first client (host client) joins the server */
		if(conn.connectionId == 0) {
			gameState = GameObject.Find("GameState Container").GetComponent<GameState>();
		}
    }

    // server action when a client disconnects
	public override void OnServerDisconnect(NetworkConnection conn) {
		/***** DONT TOUCH THIS BUILTIN SHIT ************/
		NetworkServer.DestroyPlayersForConnection(conn);

		if (conn.lastError != NetworkError.Ok) {
			if (LogFilter.logError) { Debug.LogError("Server disconnected due to error: " + conn.lastError); }
		}
		/***********************************************/

		// MY CODE

    }

    // client action when client connects
    public override void OnClientConnect(NetworkConnection conn) {
    	base.OnClientConnect(conn);

/*
    	// attach gamestate
    	gameState = GameObject.Find("GameStateManager").GetComponent<GameState>();
    	GameObject.Find("LocalGameStateContainer").GetComponent<LocalGameState>().gameState = gameState;
    	*/
    }

    // client action when a client disconnects
    public override void OnClientDisconnect(NetworkConnection conn) {
  		StopClient();
	    if (conn.lastError != NetworkError.Ok)
	    {
	    	if (LogFilter.logError) { Debug.LogError("ClientDisconnected due to error: " + conn.lastError); }
	    }

	    // make the host available for another game
	    networkDiscovery.ResetClient();
	}


/*
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
*/

}
