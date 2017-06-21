using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Moving obstacle.
/// Script to move obstacle.
/// On object: Obstacle.
/// Uses: -.
/// </summary>

public class MovingObstacle : MonoBehaviour {

	public float timer;
	public float forceX, forceY;
	public bool onPause = true;

	Rigidbody2D rb;

	void Start ()
	{
		rb = GetComponent<Rigidbody2D> ();
	}

	void Update ()
	{
		if (!onPause)
		{
			timer -= Time.deltaTime;
			if (timer <= 0)
			{
				rb.AddForce (new Vector2 (forceX, forceY));
				onPause = true;
			}
		}
	}

	void OnTriggerEnter2D(Collider2D trigger)
	{
		if (trigger.gameObject.tag == "PlayerPart")
			Destroy (gameObject);
	}
}
