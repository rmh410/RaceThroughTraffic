using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameState : MonoBehaviour {

	public string curState;
	public int maxNumPlayers;
	public int nextSpawn = 0;

// custom network manager
	private CustomNetworkManager netMan;

	// default network manager
//	private NetworkManager netMan;
	
	// Use this for initialization
	void Start () {
		curState = "waiting";

		// custom network manager
		netMan = GameObject.Find("Custom Network Manager").GetComponent<CustomNetworkManager>();

		// default network manager
//		netMan = GameObject.Find("Network Manager").GetComponent<NetworkManager>();
	}
	
	// Update is called once per frame
	void Update () {
		// if we're waiting and now have right number of players
		if(netMan.numPlayers == maxNumPlayers && curState == "waiting") {
			curState = "active";
		}
	}

	public void UpdateSpawn() {
		nextSpawn = (nextSpawn + 1)%maxNumPlayers;
	}

	
}
