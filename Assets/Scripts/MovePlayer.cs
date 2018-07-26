using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlayer : MonoBehaviour {

	public GameObject player;
	public GameObject plane;
	public float maxSpeed;
	public float accelTime;

	private float speed;
	private string state;
	private float buttonTime;
	private float lastSpeed;
	

	// Use this for initialization
	void Start () {
		state = "still";
		buttonTime = -5.0f;
		player.GetComponent<Rigidbody>().isKinematic = false;
		speed = 0.0f;
		lastSpeed = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {
		
		Rigidbody rb = player.GetComponent<Rigidbody>();
		Camera cam = Camera.main;

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
			Vector3 dest = player.transform.position + speed * cam.transform.forward * Time.deltaTime;
			dest.y = player.transform.position.y;
			// update position
			player.transform.position = dest;
			// if its been three seconds go to gliding state
			if(Time.fixedTime - buttonTime >= accelTime) {
				state = "gliding";
			}
			
		}

		else if (state == "gliding") {
			// move forward
			// slice out y component
			Vector3 dest = player.transform.position + maxSpeed * cam.transform.forward * Time.deltaTime;
			dest.y = player.transform.position.y;
			player.transform.position = dest;
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
			Vector3 dest = player.transform.position + speed * cam.transform.forward * Time.deltaTime;
			dest.y = player.transform.position.y;
			player.transform.position = dest;
			// if its been three seconds stop
			if(Time.fixedTime - buttonTime >= accelTime) {
				state = "still";
				speed = 0.0f;
			}
		}

	}
}
