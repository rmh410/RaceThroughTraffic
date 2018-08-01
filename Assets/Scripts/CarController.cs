using System.Collections;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public Material[] paint;
	public float speed = 5.0f;
    private float leftBoundary = -55.0f;
    private float rightBoundary = 55.0f;
	
	void Start()
	{
        AssignCarColor();
	}

	void Update()
	{
		transform.position += transform.forward * Time.deltaTime * speed;
		if (transform.position.x < leftBoundary || transform.position.x > rightBoundary) {
			Destroy(gameObject);
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		string collisionName = collision.transform.name;
		if (Regex.IsMatch(collisionName, "Player*")) {
			GetComponent<Rigidbody>().isKinematic = true;
		}
	}

	private void OnCollisionExit(Collision collision)
	{
		GetComponent<Rigidbody>().isKinematic = false;
	}

    private void AssignCarColor() {
        float randomPaintIndex = Random.Range(0.0f, paint.Length);
        Material randomColor = paint[(int)randomPaintIndex];
        transform.GetChild(2).GetChild(0).GetComponent<Renderer>().material = randomColor; // Top
        transform.GetChild(2).GetChild(1).GetComponent<Renderer>().material = randomColor; // Bottom
    }
}
