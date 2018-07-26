using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientScript : MonoBehaviour {

	public bool inGame;
	public GameObject myPlayer;
	public GameObject playerSpawn;

	// Use this for initialization
	void Start () {
		// instantiate a player prefab at the player spawn

		
	}
	
	// Update is called once per frame
	void Update () {
		// if not in a game look for one
		if(!inGame) {
			// ping for a host if close to a second
			if(Time.fixedTime - (int)Time.fixedTime < 0.05f) {

			}


		}


		// actual gameplay
		else {
			// move our player capsule to correct location
			Vector3 dest = Camera.main.transform.position;
			dest.y = dest.y - 0.4f;
			myPlayer.transform.position = dest;

			// 

		}
		
	}
}
