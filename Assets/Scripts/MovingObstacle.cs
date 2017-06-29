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

    public EnvironmentScaler es;
    public float firstTimer;
	public float timer;
	public float forceX, forceY;
    public Vector2 startPosition;
    public bool stop = false;

    bool onPause = false;
    float time;

	Rigidbody2D rb;

	void Start ()
	{
        forceX *= es.resolutionFactor;
        forceY *= es.resolutionFactor;

		rb = GetComponent<Rigidbody2D> ();
        // time = timer;
        time = firstTimer;
	}

	void Update ()
	{
		if (!onPause)
		{
			time -= Time.deltaTime;
			if (time <= 0)
			{
                rb.gravityScale = 1f;
                rb.AddForce (new Vector2 (forceX, forceY));
                time = timer;
				onPause = true;
			}
		}
	}

    public void StartMoving()
    {
        onPause = false;
    }

    public void StopMoving()
    {
        stop = true;
    }

    void OnTriggerEnter2D(Collider2D trigger)
	{
        if (trigger.gameObject.tag == "PlayerPart")
        {
            Destroy(gameObject);
        }
        else if (trigger.gameObject.tag == "ObstacleBoxDestroyer")
        {
            if (stop)
            {
                rb.velocity = new Vector2(0, 0);
                rb.gravityScale = 0f;
            }
            else
            {
                gameObject.transform.localPosition = new Vector3(startPosition.x, startPosition.y, -1f);
                rb.velocity = new Vector2(0, 0);
                rb.gravityScale = 0f;
                StartMoving();
            }
        }   
	}
}
