using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
	//private static Color[] paint = { Color.blue, Color.cyan, Color.green, Color.red, Color.yellow, Color.magenta };
<<<<<<< HEAD
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
=======
	//private static int lastPaintIndex = paint.Length - 1;
	private float speed = 5.0f;

	void Start()
	{
		 //Assign random color to vehicle
		//Color randomColor = paint[(int)Random.Range(0.0f, lastPaintIndex - 1)];
		//GameObject.Find("Body Top").GetComponent<Renderer>().material.color = randomColor;
		//GameObject.Find("Body Bottom").GetComponent<Renderer>().material.color = randomColor;
	}

	void Update()
	{
		transform.position += transform.forward * Time.deltaTime * speed;

>>>>>>> df06f355a6c2505e5463272b3e69f250b34a5192
	}
}
