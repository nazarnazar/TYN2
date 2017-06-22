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

		switch (trigger.gameObject.tag)			// decide what to do, depending on what we have triggered
		{
			case "Obstacle": //Also a tag for lower part of platform
				
				print ("Minus life!");
				gameplayController.MinusLife ();
				gameplayController.moving = true;
				trigger.attachedRigidbody.AddForce (new Vector2 (-trigger.attachedRigidbody.velocity.x * obstacleForce, -trigger.attachedRigidbody.velocity.y * obstacleForce));
				playerController.CollisionHappened ();
				break;

            case "AllowedArea": // A tag for top part of platform

                print("New Platform Arrived");
                gameplayController.moving = true;
                playerController.ArrivedOnPlatform(trigger.gameObject);
                if(trigger.gameObject.GetComponent<PlatformInfo>().isExist) // if this platform has any pikup on it
                {
                    switch (trigger.gameObject.GetComponent<PlatformInfo>().PickUp.tag)
                    {
                        case "Target": //final pickup that is necessary for passing the stage
                            print("Stage Completed!");
                            playerController.VictoryHappened();
                            cameraController.MoveCamera();
                            break;

                        case "PickUp":
                            print("PickUp catched up!");
                            cameraController.MoveCamera();
                            break;

                        default:
                            trigger.gameObject.GetComponent<PlatformInfo>().PickUp.SetActive(false);
                            break;
                    }
                    trigger.gameObject.GetComponent<PlatformInfo>().PickUp.SetActive(false);
                }
                break;

            case "Target": //final pickup that is necessary for passing the stages

                print ("Stage Completed!");
				trigger.gameObject.SetActive (false);
				gameplayController.moving = true;
				playerController.VictoryHappened ();
				cameraController.MoveCamera ();
				break;

            case "PickUp":
                print("PickUp catched up!");
                trigger.gameObject.SetActive(false);
                gameplayController.moving = true;
                playerController.ArrivedOnPlatform(trigger.gameObject.GetComponent<PickUpInfo>().ParentPlatform);
                cameraController.MoveCamera();
                break;

            case "DirectionBonus":

				print ("Direction Bonus!");
				playerController.PartsBouns (trigger.gameObject.GetComponent<DirectionBonus> ().directionsToAdd);
				trigger.gameObject.SetActive (false);
				break;

			case "TimeBonus":

				print ("Time Bonus!");
				gameplayController.AddTime (trigger.gameObject.GetComponent<TimeBonus> ().timeToAdd);
				trigger.gameObject.SetActive (false);
				break;

			default: 

				gameplayController.SetFakeReset ();
				gameplayController.moving = true;
				playerController.CollisionHappened ();
				break;
		}
	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		// Debug.Log ("Collison");

	}
}
