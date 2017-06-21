using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Player controller.
/// Make player parts instantiate, grow and rotate (move the player).
/// On object: Player Controller.
/// Uses: GameplayController, FadeController.
/// </summary>

public class PlayerController : MonoBehaviour {

	public GameplayController gameplayController;	// variable to reset stage or level, pause the game

	public GameObject fadePanel;			// fade in panel when you capture the target

	public GameObject head;					// players head
	public Vector2 []nextPlatformPosition;	// position of the target platform
	int platformCounter;

	public float speed;				// normal palyer scale speed
	public float secondSpeed;		// fast transition speed
	public float offset;			// player part spawning offset
	public GameObject playerPart;	// single part of the player

	public Vector2 currentPlatformPosition;	// position of the current platform
	bool starter;							// is it first player part to spawn

	Vector3 previousPosition;		// previous player part position
	Vector3 spawner;                // position of spawning next part

	public int maxNumberOfParts;		// set max number of parts can be on current level (with bonuses)
	int currentNumberOfParts;			// current max number of parts can be spawned
	int partIndex;						// index of spawning body part
	int tempPartIndex;					// temp part index to move body 'tail' in MovePlayerToNextPLatform()
    List<GameObject> parts = new List<GameObject>();		// list to search for parts
	GameObject part;				                        // variable to work with one part

	enum DirectionStates : byte {	// previous direction; need it to decide where to spawn next part
		right,
		left,
		up,
		down
	}
	DirectionStates lastDirectionState;

	Coroutine lastRoutine;		// variable to stop coroutines
	bool reverseCoroutineIsOn;	// while is fast movement can't spawn new parts

	bool doNothing;				// don't spawn new part
	bool goReverse;				// go in opposite direction

	// touch input vars
	Vector3 startTouchPos;
	Vector3 endTouchPos;
	float minDrag;

	enum SwipeState : byte {
		right,
		left,
		up,
		down,
		noSwipe
	}
	SwipeState swiper;

	public bool pauseGame;		// var to control game pause
	public bool PauseGame {
		get { return pauseGame;}
		set { pauseGame = value;}
	}

	bool startGame;		// var to control game start

 
	// Use this for initialization
	void Start () {

		PauseGame = false;
		startGame = false;

		StartInstantiate ();	// spawning unactive parts to activate them later

		swiper = SwipeState.noSwipe;	// no swipes at the beginning
		minDrag = Screen.height * 10 / 100;	// min drag screen percent to say it's a swipe

		platformCounter = 0;

		OnRestart ();

		head = Instantiate (head);
		head.transform.position = new Vector3 (currentPlatformPosition.x, currentPlatformPosition.y + 1f, -1);	// instantiating the head at the start point
	}

	void OnRestart() {		// basic resets

		currentNumberOfParts = gameplayController.StageStartPartsNumber ();	// each level stage has its own start number of parts you can spawn
		partIndex = 0;
		starter = true;
		lastRoutine = null;
		reverseCoroutineIsOn = false;

		gameplayController.SetDirectionText (currentNumberOfParts);
	}
	
	// Update is called once per frame
	void Update () {

		if (reverseCoroutineIsOn || PauseGame) {
			return;
		} else if (partIndex > currentNumberOfParts - 1) {
			print ("No more parts!");
			return;
		} else {

			// phone input (swipes)
			if (Input.touchCount == 1) {

				Touch touch = Input.GetTouch (0);
				if (touch.phase == TouchPhase.Began) {

					startTouchPos = touch.position;
					endTouchPos = touch.position;
				} else if (touch.phase == TouchPhase.Moved) {

					endTouchPos = touch.position;
				} else if (touch.phase == TouchPhase.Ended) {

					endTouchPos = touch.position;

					if (Mathf.Abs (endTouchPos.x - startTouchPos.x) > minDrag || Mathf.Abs (endTouchPos.y - startTouchPos.y) > minDrag) {

						if (Mathf.Abs (endTouchPos.x - startTouchPos.x) > Mathf.Abs (endTouchPos.y - startTouchPos.y)) {

							if (endTouchPos.x > startTouchPos.x) {

								Debug.Log ("Right swipe");
								swiper = SwipeState.right;
							} else {

								Debug.Log ("Left swipe");
								swiper = SwipeState.left;
							}
						} else {

							if (endTouchPos.y > startTouchPos.y) {

								Debug.Log ("Up swipe");
								swiper = SwipeState.up;
							} else {

								Debug.Log ("Down swipe");
								swiper = SwipeState.down;
							}
						}
					} else {

						Debug.Log ("Tap");
						swiper = SwipeState.noSwipe;
					}
				}
			}


			if (swiper == SwipeState.right || Input.GetKeyDown (KeyCode.RightArrow)) {	// our next direction

				if (!startGame)
				{
					startGame = true;
					gameplayController.UnPause ();
				}

				swiper = SwipeState.noSwipe;
				doNothing = false;
				goReverse = false;

				if (starter) {				// if this is first part
				
					Starter (DirectionStates.right);
				} else {
	
					GiveMeCurrentPart();

					switch (lastDirectionState) {	// direction of previous part, so that we can spawn new part in the right place

					case DirectionStates.up:
						spawner = new Vector3 (previousPosition.x + offset, previousPosition.y + part.transform.localScale.y - offset, previousPosition.z);
						break;

					case DirectionStates.down:
						spawner = new Vector3 (previousPosition.x + offset, previousPosition.y - part.transform.localScale.y + offset, previousPosition.z);
						break;

					case DirectionStates.left:
						goReverse = true;
						break;

					case DirectionStates.right:
						doNothing = true;
						break;
					}

					if (!doNothing && !goReverse) {
						AddPart ();
					} else if (goReverse) {		// when go reverse we make fake stage reset (watch GameplayController)
						gameplayController.SetFakeReset ();
					}
				}

				Movement (DirectionStates.right, -90);		// function to calculate movement direction of spawned part

			} else if (swiper == SwipeState.left || Input.GetKeyDown (KeyCode.LeftArrow)) {

				if (!startGame)
				{
					startGame = true;
					gameplayController.UnPause ();
				}

				swiper = SwipeState.noSwipe;
				doNothing = false;
				goReverse = false;
			
				if (starter) {

					Starter (DirectionStates.left);
				} else {
					
					GiveMeCurrentPart();

					switch (lastDirectionState) {

					case DirectionStates.up:
						spawner = new Vector3 (previousPosition.x - offset, previousPosition.y + part.transform.localScale.y - offset, previousPosition.z);
						break;

					case DirectionStates.down:
						spawner = new Vector3 (previousPosition.x - offset, previousPosition.y - part.transform.localScale.y + offset, previousPosition.z);
						break;

					case DirectionStates.left:
						doNothing = true;
						break;

					case DirectionStates.right:
						goReverse = true;
						break;
					}

					if (!doNothing && !goReverse) {
                        AddPart();
					} else if (goReverse) {
						gameplayController.SetFakeReset ();
					}
				}

				Movement (DirectionStates.left, 90);

			} else if (swiper == SwipeState.up || Input.GetKeyDown (KeyCode.UpArrow)) {

				if (!startGame)
				{
					startGame = true;
					gameplayController.UnPause ();
				}

				swiper = SwipeState.noSwipe;
				doNothing = false;
				goReverse = false;
			
				if (starter) {

					Starter (DirectionStates.up);
				} else {
					
					GiveMeCurrentPart();

					switch (lastDirectionState) {

					case DirectionStates.up:
						doNothing = true;
						break;

					case DirectionStates.down:
						goReverse = true;
						break;

					case DirectionStates.left:
						spawner = new Vector3 (previousPosition.x - part.transform.localScale.y + offset, previousPosition.y + offset, previousPosition.z);
						break;

					case DirectionStates.right:
						spawner = new Vector3 (previousPosition.x + part.transform.localScale.y - offset, previousPosition.y + offset, previousPosition.z);
						break;
					}

					if (!doNothing && !goReverse) {
                        AddPart();
					} else if (goReverse) {
						gameplayController.SetFakeReset ();
					}
				}

				Movement (DirectionStates.up, 0);

			} else if (swiper == SwipeState.down || Input.GetKeyDown (KeyCode.DownArrow)) {

				if (!startGame)
				{
					startGame = true;
					gameplayController.UnPause ();
				}

				swiper = SwipeState.noSwipe;
				doNothing = false;
				goReverse = false;

				if (starter) {

					Starter (DirectionStates.down);
				} else {
					
					GiveMeCurrentPart();

					switch (lastDirectionState) {

					case DirectionStates.up:
						goReverse = true;
						break;

					case DirectionStates.down:
						doNothing = true;
						break;

					case DirectionStates.left:
						spawner = new Vector3 (previousPosition.x - part.transform.localScale.y + offset, previousPosition.y - offset, previousPosition.z);
						break;

					case DirectionStates.right:
						spawner = new Vector3 (previousPosition.x + part.transform.localScale.y - offset, previousPosition.y - offset, previousPosition.z);
						break;
					}

					if (!doNothing && !goReverse) {
                        AddPart();
					} else if (goReverse) {
						gameplayController.SetFakeReset ();
					}
				}

				Movement (DirectionStates.down, 180);

			}
		}

		gameplayController.SetDirectionText (currentNumberOfParts - partIndex);		// not best place..
	}


	// additional private functions

	void StartInstantiate() {

		for (int i = 0; i < maxNumberOfParts; i++) {
			GameObject tempPart = Instantiate (playerPart);
			tempPart.SetActive (false);
			parts.Add (tempPart);
		}
	}

    void AddPart()
    {
		parts [partIndex].SetActive (true);
		parts [partIndex].transform.position = spawner;
		partIndex++;

        head.transform.position = spawner;                      // changing head position
        previousPosition = spawner;                             // remember current position which will be 'previous' on the next direction changing
    }

	void Starter(DirectionStates direction) {	// spawning first player part

		spawner = new Vector3 (currentPlatformPosition.x, currentPlatformPosition.y + offset, 0);

		parts [partIndex].SetActive (true);
		parts [partIndex].transform.position = spawner;
		partIndex++;

		previousPosition = spawner;	// remember current position which will be 'previous' on the next direction changing
		starter = false;			// next part will not be a starter
	}

	void Movement(DirectionStates direction, float zRotation) {		// function to calculate movement direction of spawned part

		if (doNothing) { 

		} else if (goReverse) {		// behaviour of opposite direction is handled as CollisionHappened() (go back to start platform)

			CollisionHappened (); 
		} else {					// calculating movement direction of new part
			
			lastDirectionState = direction;

			GiveMeCurrentPart();

			part.GetComponent<PlayerPart> ().PartOrientation = (PlayerPart.Orientation) direction;	// holding direction
			part.transform.Rotate (0, 0, zRotation);												// rotate to make sprite grow in needed way

			// head child settings	
			head.transform.parent = part.transform;
			head.transform.localRotation = new Quaternion(0, 0, 0, 0);
			head.transform.localPosition = new Vector3 (0, 1, -1);

			part.GetComponent<BoxCollider2D> ().isTrigger = true;	// needed to handle collision with parts that were spawned before

			if (lastRoutine != null) {							// stop previous part growing
				StopCoroutine (lastRoutine);
			}
			lastRoutine = StartCoroutine (MovePlayer (part));	// start growing new part
		}
	}

	void DeactivatePart() {
		part.SetActive (false);
		part.transform.localScale = new Vector3 (1, 1, 1);
		part.transform.localRotation = new Quaternion (0, 0, 0, 0);
		part.transform.position = new Vector3 (0, 0, 0);
	}

	void GiveMeCurrentPart() {
		if (partIndex > 0) {
			part = parts [partIndex - 1];
		} else {
			part = null;
		}
	}

	void GiveMeFirstPart() {	// part variable will hold first body part
		part = parts [0];
	}

	void ResetHeadTransform(float xPos, float yPos) {	// resets head trannsform for needed platform

		head.transform.position = new Vector3 (xPos, yPos + 2 * offset, -1);
		head.transform.localRotation = new Quaternion(0, 0, 0, 0);
		head.transform.localScale = new Vector3 (1, 1, head.transform.localScale.z);
	}


	// public functions

	public void CollisionHappened() {	// if we collide, we need to make player come back to start platform

		GiveMeCurrentPart ();

		if (lastRoutine != null) {
			StopCoroutine (lastRoutine);
		}

		if (!reverseCoroutineIsOn && part != null) {					// if some other coroutine is not running
			
			reverseCoroutineIsOn = true;				// while true we can't spawn new parts or change direction
			StartCoroutine (MovePlayerReverse (part));		// start moving in reverse way
		} else {
			if (part == null) {		// if head is still on the platform

				gameplayController.StageRestart ();		
			}
		}
	}

	public void VictoryHappened() {		// if we collide with our target, we need to make player change the platform

		GiveMeFirstPart ();
		tempPartIndex = 0;

		if (lastRoutine != null) {
			StopCoroutine (lastRoutine);
		}

		if (!reverseCoroutineIsOn) {					// if some other coroutine is not running

			reverseCoroutineIsOn = true;				// while true we can't spawn new parts or change direction
			StartCoroutine (MovePlayerToNextPlatform (part));	// start moving to the next platform
		}
	}

	public void PartsBouns(int number) {	// now we can use more parts

		currentNumberOfParts += number;
	}


	// iterators

	IEnumerator MovePlayer(GameObject part) {			// moving player iterator (we scale player part in y dimention each Time.deltaTime)

		while (true) {

			float timer = Time.deltaTime;

			while (timer > 0) {

				timer -= Time.deltaTime;
				yield return null;
			}

			part.transform.DetachChildren ();			// unchild head to scale body part without scaling the head and then child head again
			part.transform.localScale = new Vector3 (part.transform.localScale.x, part.transform.localScale.y + speed * Time.deltaTime, part.transform.localScale.z);
			head.transform.localScale = new Vector3 (1, 1, head.transform.localScale.z);	// keeping head same size
			head.transform.parent = part.transform;		// now head is a child of part

			head.transform.localRotation = new Quaternion(0, 0, 0, 0);			// preventing head from rotation
			head.transform.localPosition = new Vector3 (0, 1, -1);				// y local position of the head is allways 1
		}
	}

	IEnumerator MovePlayerReverse(GameObject part) {	// moving player in reverse

		while (part.transform.localScale.y > 0) {

			float timer = Time.deltaTime;

			while (timer > 0) {

				timer -= Time.deltaTime;
				yield return null;
			}

			part.transform.DetachChildren ();		// unchild head to scale body part without scaling the head and then child head again
			part.transform.localScale = new Vector3 (part.transform.localScale.x, part.transform.localScale.y - secondSpeed * Time.deltaTime, part.transform.localScale.z);
			head.transform.localScale = new Vector3 (1, 1, head.transform.localScale.z);
			head.transform.parent = part.transform;	// now head is a child of part

			head.transform.localRotation = new Quaternion(0, 0, 0, 0);	// preventing head from rotation
			head.transform.localPosition = new Vector3 (0, 1, -1);		// y local position of the head is allways 1
		}
			
		// unchilding head
		part.transform.DetachChildren ();
	
		part.SetActive (false);
		part.transform.localScale = new Vector3 (1, 1, 1);
		part.transform.localRotation = new Quaternion (0, 0, 0, 0);
		part.transform.position = new Vector3 (0, 0, 0);

		partIndex--;

		if (partIndex > 0) {
			part = parts [partIndex - 1];

			head.transform.parent = part.transform;

			head.transform.localScale = new Vector3 (1, 1f / part.transform.localScale.y, head.transform.localScale.z);	// set head scale reletive to the part 
			head.transform.localRotation = new Quaternion(0, 0, 0, 0);
			head.transform.localPosition = new Vector3 (0, 1, -1);

			StartCoroutine (MovePlayerReverse (part));
		} else {
			
			ResetHeadTransform (currentPlatformPosition.x, currentPlatformPosition.y);
			gameplayController.StageRestart ();	// restart the stage when collision happened
			gameplayController.moving = false;	// we are not moving 
			OnRestart ();						// restart player
		}
	}

	IEnumerator MovePlayerToNextPlatform(GameObject part) {	// moving player to the next platform (we know part orientation, so we rotate it by 180 and change its position in appropriate way)

		part.transform.Rotate (0, 0, 180);

		switch (part.GetComponent<PlayerPart> ().PartOrientation) {

		case PlayerPart.Orientation.right:
			
			part.transform.position = new Vector3 (part.transform.position.x + part.transform.localScale.y, part.transform.position.y, part.transform.position.z);
			break;

		case PlayerPart.Orientation.left:
			
			part.transform.position = new Vector3 (part.transform.position.x - part.transform.localScale.y, part.transform.position.y, part.transform.position.z);
			break;

		case PlayerPart.Orientation.up:
			
			part.transform.position = new Vector3 (part.transform.position.x, part.transform.position.y + part.transform.localScale.y, part.transform.position.z);
			break;

		case PlayerPart.Orientation.down:
			
			part.transform.position = new Vector3 (part.transform.position.x, part.transform.position.y - part.transform.localScale.y, part.transform.position.z);
			break;
		}

		while (part.transform.localScale.y > 0) {

			float timer = Time.deltaTime;

			while (timer > 0) {

				timer -= Time.deltaTime;
				yield return null;
			}

			part.transform.localScale = new Vector3 (part.transform.localScale.x, part.transform.localScale.y - secondSpeed * Time.deltaTime, part.transform.localScale.z);
		}

		part.SetActive (false);
		part.transform.localScale = new Vector3 (1, 1, 1);
		part.transform.localRotation = new Quaternion (0, 0, 0, 0);
		part.transform.position = new Vector3 (0, 0, 0);

		tempPartIndex++;

		if (tempPartIndex < partIndex) {

			part = parts [tempPartIndex];

			if (tempPartIndex == partIndex - 1) {

				fadePanel.SetActive (true);
				StartCoroutine (fadePanel.GetComponent<FadeController> ().FadeInPanel (1f / part.transform.localScale.y, new Color (1f, 0.4f, 0f, 0f)));
			}

			StartCoroutine (MovePlayerToNextPlatform (part));
		} else {
			
			// unchilding head
			part.transform.DetachChildren ();	// SetActive() works at the end of frame, so we still able to get this part

			ResetHeadTransform (nextPlatformPosition [platformCounter].x, nextPlatformPosition [platformCounter].y);
			currentPlatformPosition = nextPlatformPosition [platformCounter++];		// we reached the target, so our next platform becomes our current platform
			gameplayController.NextStage ();
			gameplayController.moving = false;
			OnRestart ();
		}
	}
}
