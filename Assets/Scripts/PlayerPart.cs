using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Player part.
/// Stores info about single body part (orientation, ...) and collisions with this part.
/// On object: Player Part.
/// Uses: GameplayController, PlayerController, CameraController. 
/// </summary>

public class PlayerPart : MonoBehaviour {

	public float obstacleForce;

	GameplayController gameplayController;
	CameraController cameraController;	// object to set camera position to the next target
	PlayerController playerController;

    public enum Orientation : byte
	{
		right,
		left,
		up,
		down
	}
	public Orientation partOrientation;		// part orientation holder

	public Orientation PartOrientation
	{

		get { return partOrientation; }
		set { partOrientation = value; }
	}

	void Start ()
	{
		gameplayController = FindObjectOfType<GameplayController> ();
		playerController = FindObjectOfType<PlayerController> ();
		cameraController = FindObjectOfType<CameraController> ();
	}

	void OnTriggerEnter2D(Collider2D trigger)
	{
        // Debug.Log ("Trigger");

        switch (trigger.gameObject.tag)         // decide what to do, depending on what we have triggered
        {
            case "Platform":

                print("Changed platform!");
                if (!gameplayController.moving)
                {
                    gameplayController.moving = true;
                    playerController.GoNextPlatform(new Vector2(trigger.gameObject.transform.localPosition.x, trigger.gameObject.transform.localPosition.y + 0.25f), true);
                }
                break;

            case "Obstacle":

                print("Minus life!");

                gameplayController.moving = true;
                gameplayController.MinusLife();
                trigger.attachedRigidbody.AddForce(new Vector2(-trigger.attachedRigidbody.velocity.x * obstacleForce, -trigger.attachedRigidbody.velocity.y * obstacleForce));
                break;

            case "Target":

                print("Stage Completed!");

                trigger.gameObject.SetActive(false);
                gameplayController.moving = true;
                playerController.GoNextPlatform(new Vector2(trigger.gameObject.transform.localPosition.x, trigger.gameObject.transform.localPosition.y - 1f), false);
                gameplayController.NextStage();
                cameraController.MoveCamera();
                break;

            case "DirectionBonus":

                print("Direction Bonus!");

                if (!gameplayController.moving)
                {
                    playerController.PartsBouns(trigger.gameObject.GetComponent<DirectionBonus>().directionsToAdd);
                    trigger.gameObject.SetActive(false);
                }
                break;

            case "TimeBonus":

                print("Time Bonus!");

                if (!gameplayController.moving)
                {
                    gameplayController.AddTime(trigger.gameObject.GetComponent<TimeBonus>().timeToAdd);
                    trigger.gameObject.SetActive(false);
                }
                break;

            default:
                Debug.Break();
                if (!gameplayController.moving)
                {
                    gameplayController.SetFakeReset();
                    gameplayController.moving = true;
                    playerController.CollisionHappened();
                }
                break;
        }
	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		// Debug.Log ("Collison");

	}
}
