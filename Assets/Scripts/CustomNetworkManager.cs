using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CustomNetworkManager : NetworkManager {
    private GameState gameState;
	private RaceNetworkDiscovery networkDiscovery;

	public override void OnStartServer ()
    {
		networkDiscovery = GameObject.Find("Network Discovery").GetComponent<RaceNetworkDiscovery>();
	}

	// Server action when a client connects
	public override void OnServerConnect(NetworkConnection conn)
    {
        // Ensure GameState Container exists by assigning
        // it at the time first (host) client joins server
		if (conn.connectionId == 0) {
			gameState = GameObject.Find("GameState Container").GetComponent<GameState>();
		}
    }

    // Server action when a client disconnects
	public override void OnServerDisconnect(NetworkConnection conn)
    {
		// Do not touch below -------------------------
		NetworkServer.DestroyPlayersForConnection(conn);
		if (conn.lastError != NetworkError.Ok) {
			if (LogFilter.logError) { Debug.LogError("Server disconnected due to error: " + conn.lastError); }
		}
		// Do not touch above -------------------------

		// MY CODE
        // Debug.Log("Client" + conn.connectionId + " disconnected");
    }

    // Client action when client connects
    public override void OnClientConnect(NetworkConnection conn)
    {
    	base.OnClientConnect(conn);
        /*
    	// attach gamestate
    	gameState = GameObject.Find("GameStateManager").GetComponent<GameState>();
    	GameObject.Find("LocalGameStateContainer").GetComponent<LocalGameState>().gameState = gameState;
    	*/
    }

    // Client action when a client disconnects
    public override void OnClientDisconnect(NetworkConnection conn)
    {
  		// StopClient();
	    if (conn.lastError != NetworkError.Ok)
	    {
	    	if (LogFilter.logError) { Debug.LogError("ClientDisconnected due to error: " + conn.lastError); }
	    }

	    // Make host available for another game
	    networkDiscovery.ResetClient();
	}
}
