using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
	private static Color[] paint = { Color.blue, Color.cyan, Color.green, Color.red, Color.yellow, Color.magenta };
	private static int lastPaintIndex = paint.Length - 1;
	private float speed = 5.0f;

	void Start()
	{
		// Assign random color to vehicle
		Color randomColor = paint[(int)Random.Range(0.0f, lastPaintIndex - 1)];
		GameObject.Find("Body Top").GetComponent<Renderer>().material.color = randomColor;
		GameObject.Find("Body Bottom").GetComponent<Renderer>().material.color = randomColor;
	}

	void Update()
	{
		transform.position += transform.forward * Time.deltaTime * speed;
	}
}
