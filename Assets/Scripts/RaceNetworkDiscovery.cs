using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RaceNetworkDiscovery : NetworkDiscovery
{
	private bool inGame = false;
	private GameParameters paramComponent;
	private Transform canvas;

	// Use this for initialization
	void Start () {
		Initialize();
		paramComponent = this.GetComponent<GameParameters>();
		canvas = GameObject.Find("Matchmaking Canvas").transform;
	}
	
	public override void OnReceivedBroadcast(string fromAddress, string data) {
		// If we are not in a game should be trying to join
		if (!inGame) {
			base.OnReceivedBroadcast(fromAddress, data);
			string[] addressSplit = fromAddress.Split(':');
			string[] dataSplit = data.Split(':');
			paramComponent.seed = int.Parse(dataSplit[1]);

			NetworkManager.singleton.networkAddress = addressSplit[addressSplit.Length-1];
			NetworkManager.singleton.networkPort = int.Parse(dataSplit[0]);
			NetworkManager.singleton.StartClient();

			// remove button
			canvas.GetChild(1).gameObject.SetActive(false);

			// seed rng
			Random.seed = paramComponent.seed;

			// stop listening and trying to join
			inGame = true;
		}
	}

	public void StartListeningBroadcast() {
		// change text to searching for game
		Transform joinButton = canvas.GetChild(1);
		joinButton.GetChild(0).gameObject.SetActive(false);
		joinButton.GetChild(1).gameObject.SetActive(true);
		canvas.GetChild(0).gameObject.SetActive(false);

		// start listening for server to broadcast
		StartAsClient();
	}

	public void StartAsHost() {
		// start the game server
		NetworkManager.singleton.StartHost();

		// remove both buttons
		canvas.GetChild(0).gameObject.SetActive(false);
		canvas.GetChild(1).gameObject.SetActive(false);

		// decide on a random seed and broadcast host data across the network
		int seed = (int)System.DateTime.Now.Ticks;
		paramComponent.seed = seed;
		broadcastData = paramComponent.port + ":" + seed.ToString();
		StartAsServer();

		// seed rng
		Random.seed = paramComponent.seed;
	}

	public void ResetClient() {
        Debug.Log("Hello from ResetClient");
		// make avilable to receive brodacasts again
		inGame = false;
		// draw the canvas back up
		Transform joinButton = canvas.GetChild(1);
		joinButton.GetChild(0).gameObject.SetActive(true);
		joinButton.GetChild(1).gameObject.SetActive(false);
		canvas.GetChild(0).gameObject.SetActive(true);
		canvas.gameObject.SetActive(true);
	}
}
