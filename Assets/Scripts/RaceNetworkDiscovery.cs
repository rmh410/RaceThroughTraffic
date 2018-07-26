using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RaceNetworkDiscovery : NetworkDiscovery {
	private bool inGame = false;

	// Use this for initialization
	void Start () {
		Initialize();
	}
	
	public override void OnReceivedBroadcast(string fromAddress, string data) {
		// if we are not in a game should be trying to join
		if(!inGame) {
			base.OnReceivedBroadcast(fromAddress, data);
			Debug.Log("FromAddress: " + fromAddress + " Data: " + data);
			string[] addressSplit = fromAddress.Split(':');
			string[] dataSplit = data.Split(';');
			NetworkManager.singleton.networkAddress = addressSplit[addressSplit.Length-1];
			NetworkManager.singleton.networkPort = int.Parse(dataSplit[dataSplit.Length-1]);
			NetworkManager.singleton.StartClient();
		}
	}

	public void StartListeningBroadcast() {
		StartAsClient();
	}

	public void StartAsHost() {
		NetworkManager.singleton.StartHost();
		StartAsServer();
	}
}
