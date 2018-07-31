﻿using System.Collections;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{

	public float speed = 5.0f;
	public Material[] paint;

	void Start()
	{
		// Assign random color to vehicle
		Material randomColor = paint[(int)Random.Range(0.0f, paint.Length)];
		transform.GetChild(2).GetChild(0).GetComponent<Renderer>().material = randomColor;
		transform.GetChild(2).GetChild(1).GetComponent<Renderer>().material = randomColor;
	}

	void Update()
	{
		// move car forward
		transform.position += transform.forward * Time.deltaTime * speed;
		// destroy car if its off the map
		if (transform.position.x < -55.0f || transform.position.x > 55.0f) {
			Destroy(gameObject);
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		string collisionName = collision.transform.name;
		Debug.Log(collisionName);
		if (Regex.IsMatch(collisionName, "Player*")) {
			Debug.Log("Hit Player");
			GetComponent<Rigidbody>().isKinematic = true;
		}
	}

	private void OnCollisionExit(Collision collision)
	{
		GetComponent<Rigidbody>().isKinematic = false;
	}
}
