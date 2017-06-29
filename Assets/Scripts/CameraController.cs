using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Camera controller.
/// Script to move camera when stage completed.
/// On object: Main Camera.
/// Uses: -.
/// </summary>

public class CameraController : MonoBehaviour {

    public EnvironmentScaler es;
	public float speed;
	public int timesWeChangeLevel;
	public float distance;			// distance to move the camera

    void Start()
    {
        distance *= es.resolutionFactor;
    }

	public void MoveCamera()
	{
		if (timesWeChangeLevel > 0)
		{
			StartCoroutine (MovingCamera ());
			timesWeChangeLevel--;
		}
	}

	IEnumerator MovingCamera()
	{
		float tempDistance = distance;

		while (distance > 0)
		{
			float timer = Time.deltaTime;

			while (timer > 0)
			{
				timer -= Time.deltaTime;
				yield return null;
			}

			distance -= speed * Time.deltaTime;
			transform.position = new Vector3 (transform.position.x + speed * Time.deltaTime, transform.position.y, transform.position.z);
		}

		distance = tempDistance;
	}
}
