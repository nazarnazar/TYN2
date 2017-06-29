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

    public GameObject Environment;
    EnvironmentScaler environmentScaler;
	public GameplayController gameplayController;	// variable to reset stage or level, pause the game
    public TurnsPanel turnsPanel;
	public GameObject fadePanel;			// fade in panel when you capture the target

	public GameObject head;					// players head
	// public Vector2 []nextPlatformPosition;	// position of the target platform
	// int platformCounter;
    Vector2 nextPlatformPosition;

	public float speed;				// normal palyer scale speed
	public float secondSpeed;		// fast transition speed
	public float offset;			// player part spawning offset
	public GameObject playerPart;	// single part of the player
    public GameObject playerTurnPart;

    public Vector2 currentPlatformPosition;	// position of the current platform
    Vector2 startPlatformPosition;
	public float platformOffset;
	bool starter;							// is it first player part to spawn

	Vector3 previousPosition;		// previous player part position
	Vector3 spawner;                // position of spawning next part

	public int maxNumberOfParts;		// set max number of parts can be on current level (with bonuses)
	int currentNumberOfParts;			// current max number of parts can be spawned
	int partIndex;						// index of spawning body part
	int tempPartIndex;					// temp part index to move body 'tail' in MovePlayerToNextPLatform()
    List<GameObject> parts = new List<GameObject>();		// list to search for parts
	GameObject part;				                        // variable to work with one part
    List<GameObject> turnParts = new List<GameObject>();
    GameObject turnPart;
    int turnPartIndex;

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
    bool middlePlatform;

 
	// Use this for initialization
	void Start () {

        environmentScaler = Environment.GetComponent<EnvironmentScaler>();

        startPlatformPosition = currentPlatformPosition;

		PauseGame = false;
		startGame = false;
        middlePlatform = false;

		StartInstantiate ();	// spawning unactive parts to activate them later
        StartTurnPartsInstantiate();

		swiper = SwipeState.noSwipe;	// no swipes at the beginning
		minDrag = Screen.height * 10 / 100;	// min drag screen percent to say it's a swipe

		OnRestart ();

		head = Instantiate (head);
        head.transform.localScale = new Vector3(head.transform.localScale.x * environmentScaler.resolutionFactor, head.transform.localScale.y * environmentScaler.resolutionFactor, head.transform.localScale.z);
        head.transform.SetParent(Environment.transform);
		head.transform.localPosition = new Vector3 (currentPlatformPosition.x, currentPlatformPosition.y + platformOffset, -1);	// instantiating the head at the start point
	}

	void OnRestart() {		// basic resets

		currentNumberOfParts = gameplayController.StageStartPartsNumber (currentNumberOfParts - partIndex);	// each level stage has its own start number of parts you can spawn

		partIndex = 0;
        turnPartIndex = 0;
		starter = true;
		lastRoutine = null;
		reverseCoroutineIsOn = false;
        middlePlatform = false;

		gameplayController.SetDirectionText (currentNumberOfParts);
	}

    void OnFakeRestart()
    {
        currentNumberOfParts -= partIndex;

        partIndex = 0;
        turnPartIndex = 0;
        starter = true;
        lastRoutine = null;
        reverseCoroutineIsOn = false;

        gameplayController.SetDirectionText(currentNumberOfParts);
    }
	
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
                            Debug.Log(head.transform.position);
                            PlaceTurnBodyPart(DirectionStates.up, DirectionStates.right);
                        break;

					case DirectionStates.down:
						spawner = new Vector3 (previousPosition.x + offset, previousPosition.y - part.transform.localScale.y + offset, previousPosition.z);
                        PlaceTurnBodyPart(DirectionStates.down, DirectionStates.right);
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
                        PlaceTurnBodyPart(DirectionStates.up, DirectionStates.left);
                        break;

					case DirectionStates.down:
						spawner = new Vector3 (previousPosition.x - offset, previousPosition.y - part.transform.localScale.y + offset, previousPosition.z);
                        PlaceTurnBodyPart(DirectionStates.down, DirectionStates.left);
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
                        PlaceTurnBodyPart(DirectionStates.left, DirectionStates.up);
                        break;

					case DirectionStates.right:
						spawner = new Vector3 (previousPosition.x + part.transform.localScale.y - offset, previousPosition.y + offset, previousPosition.z);
                        PlaceTurnBodyPart(DirectionStates.right, DirectionStates.up);
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
                        PlaceTurnBodyPart(DirectionStates.left, DirectionStates.down);
                        break;

					case DirectionStates.right:
						spawner = new Vector3 (previousPosition.x + part.transform.localScale.y - offset, previousPosition.y - offset, previousPosition.z);
                        PlaceTurnBodyPart(DirectionStates.right, DirectionStates.down);
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

        gameplayController.SetDirectionText(currentNumberOfParts - partIndex);		// not best place..
        turnsPanel.ActivateTurnsPanel(currentNumberOfParts - partIndex);
    }


	// additional private functions

	void StartInstantiate() {

		for (int i = 0; i < maxNumberOfParts; i++) {
			GameObject tempPart = Instantiate (playerPart);
            tempPart.transform.localScale = new Vector3(tempPart.transform.localScale.x * environmentScaler.resolutionFactor, tempPart.transform.localScale.y * environmentScaler.resolutionFactor, tempPart.transform.localScale.z);
            tempPart.transform.SetParent(Environment.transform);
			tempPart.SetActive (false);
			parts.Add (tempPart);
		}
	}

    void StartTurnPartsInstantiate()
    {
        for (int i = 0; i < maxNumberOfParts; i++)
        {
            GameObject tempPart = Instantiate(playerTurnPart);
            tempPart.transform.localScale = new Vector3(tempPart.transform.localScale.x * environmentScaler.resolutionFactor, tempPart.transform.localScale.y * environmentScaler.resolutionFactor, tempPart.transform.localScale.z);
            tempPart.transform.SetParent(Environment.transform);
            tempPart.SetActive(false);
            turnParts.Add(tempPart);
        }
    }

    void AddPart()
    {
		parts [partIndex].transform.localPosition = spawner;
        parts[partIndex].SetActive(true);
        partIndex++;

        head.transform.localPosition = spawner;                      // changing head position
        previousPosition = spawner;                             // remember current position which will be 'previous' on the next direction changing
    }

	void Starter(DirectionStates direction) {	// spawning first player part

		spawner = new Vector3 (currentPlatformPosition.x, currentPlatformPosition.y + platformOffset - offset, 0);

		parts [partIndex].transform.localPosition = spawner;
        parts[partIndex].SetActive(true);
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
			part.transform.Rotate (0, 0, zRotation);                                                // rotate to make sprite grow in needed way

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

    void PlaceTurnBodyPart(DirectionStates prev, DirectionStates now)
    {
        part.transform.localScale = new Vector3(part.transform.localScale.x, part.transform.localScale.y - 1f, part.transform.localScale.z);
        part.transform.DetachChildren();
        head.transform.SetParent(Environment.transform);
        Vector3 turnSpawner = head.transform.localPosition;

        if (prev == DirectionStates.up)
        {
            if (now == DirectionStates.right)
            {
                turnParts[turnPartIndex].transform.localPosition = new Vector3(turnSpawner.x, turnSpawner.y + offset, turnSpawner.z);
                turnParts[turnPartIndex].transform.localEulerAngles = new Vector3(0, 0, 90f);
            }
            else if (now == DirectionStates.left)
            {
                turnParts[turnPartIndex].transform.localPosition = new Vector3(turnSpawner.x, turnSpawner.y + offset, turnSpawner.z);
                turnParts[turnPartIndex].transform.localEulerAngles = new Vector3(0, 0, 0);
            }
        }
        else if (prev == DirectionStates.down)
        {
            if (now == DirectionStates.right)
            {
                turnParts[turnPartIndex].transform.localPosition = new Vector3(turnSpawner.x, turnSpawner.y - offset, turnSpawner.z);
                turnParts[turnPartIndex].transform.localEulerAngles = new Vector3(0, 0, 180f);
            }
            else if (now == DirectionStates.left)
            {
                turnParts[turnPartIndex].transform.localPosition = new Vector3(turnSpawner.x, turnSpawner.y - offset, turnSpawner.z);
                turnParts[turnPartIndex].transform.localEulerAngles = new Vector3(0, 0, 270f);
            }
        }
        else if (prev == DirectionStates.left)
        {
            if (now == DirectionStates.up)
            {
                turnParts[turnPartIndex].transform.localPosition = new Vector3(turnSpawner.x - offset, turnSpawner.y, turnSpawner.z);
                turnParts[turnPartIndex].transform.localEulerAngles = new Vector3(0, 0, 180f);
            }
            else if(now == DirectionStates.down)
            {
                turnParts[turnPartIndex].transform.localPosition = new Vector3(turnSpawner.x - offset, turnSpawner.y, turnSpawner.z);
                turnParts[turnPartIndex].transform.localEulerAngles = new Vector3(0, 0, 90f);
            }
        }
        else if (prev == DirectionStates.right)
        {
            if (now == DirectionStates.up)
            {
                turnParts[turnPartIndex].transform.localPosition = new Vector3(turnSpawner.x + offset, turnSpawner.y, turnSpawner.z);
                turnParts[turnPartIndex].transform.localEulerAngles = new Vector3(0, 0, 270f);
            }
            else if (now == DirectionStates.down)
            {
                turnParts[turnPartIndex].transform.localPosition = new Vector3(turnSpawner.x + offset, turnSpawner.y, turnSpawner.z);
                turnParts[turnPartIndex].transform.localEulerAngles = new Vector3(0, 0, 0f);
            }
        }

        turnParts[turnPartIndex].SetActive(true);
        turnPartIndex++;
    }

    void DeactivatePart() {
		part.SetActive (false);
		part.transform.localScale = new Vector3 (1, 1, 1);
		part.transform.localRotation = new Quaternion (0, 0, 0, 0);
		part.transform.localPosition = new Vector3 (0, 0, 0);
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
        head.transform.SetParent(Environment.transform);
		head.transform.localPosition = new Vector3 (xPos, yPos + platformOffset, -1);
		head.transform.localRotation = new Quaternion(0, 0, 0, 0);
		head.transform.localScale = new Vector3 (1, 1, head.transform.localScale.z);
	}


	// public functions

	public void CollisionHappened() {	// if we collide, we need to make player come back to start platform

		GiveMeCurrentPart ();

		if (lastRoutine != null) {
			StopCoroutine (lastRoutine);
		}

		if (!reverseCoroutineIsOn && part != null) {                    // if some other coroutine is not running

            turnPartIndex--;
			reverseCoroutineIsOn = true;				// while true we can't spawn new parts or change direction
			lastRoutine = StartCoroutine (MovePlayerReverse (part));		// start moving in reverse way
		} else {
			if (part == null) {		// if head is still on the platform

				gameplayController.StageRestart ();		
			}
		}
	}

	public void GoNextPlatform(Vector2 next, bool isMiddlePlatform) {		// if we collide with our target, we need to make player change the platform

        if (isMiddlePlatform)
            middlePlatform = true;
        else
            middlePlatform = false;

        nextPlatformPosition = next;

		GiveMeFirstPart ();
		tempPartIndex = 0;

		if (lastRoutine != null) {
			StopCoroutine (lastRoutine);
		}

		if (!reverseCoroutineIsOn) {					// if some other coroutine is not running

			reverseCoroutineIsOn = true;                // while true we can't spawn new parts or change direction
            lastRoutine = StartCoroutine(MovePlayerToNextPlatform (part));	// start moving to the next platform
		}
	}

	public void PartsBouns(int number) {	// now we can use more parts

		currentNumberOfParts += number;
	}

    public void SetStartGame()
    {
        startGame = false;
    }

    public void MovePlayerToStart()
    {
        if (lastRoutine != null)
            StopCoroutine(lastRoutine);

        GiveMeCurrentPart();

        if (part != null)
        {
            part.transform.DetachChildren();

            for (int i = 0; i < currentNumberOfParts; i++)
            {
                parts[i].SetActive(false);
                parts[i].transform.localScale = new Vector3(1, 1, 1);
                parts[i].transform.localRotation = new Quaternion(0, 0, 0, 0);
                parts[i].transform.localPosition = new Vector3(0, 0, 0);
            }

            for (int i = 0; i < currentNumberOfParts; i++)
                turnParts[i].SetActive(false);
        }

        head.transform.SetParent(Environment.transform);
        head.transform.localRotation = new Quaternion(0, 0, 0, 0);
        head.transform.localPosition = new Vector3(startPlatformPosition.x, startPlatformPosition.y + platformOffset, -1);
        gameplayController.moving = false;

        currentPlatformPosition = startPlatformPosition;

        gameplayController.StageRestart();  // restart the stage when collision happened
        OnRestart();                        // restart player
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
			head.transform.localScale = new Vector3 (environmentScaler.resolutionFactor, environmentScaler.resolutionFactor, head.transform.localScale.z);	// keeping head same size
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
			head.transform.localScale = new Vector3 (environmentScaler.resolutionFactor, environmentScaler.resolutionFactor, head.transform.localScale.z);
			head.transform.parent = part.transform;	// now head is a child of part

			head.transform.localRotation = new Quaternion(0, 0, 0, 0);	// preventing head from rotation
			head.transform.localPosition = new Vector3 (0, 1, -1);		// y local position of the head is allways 1
		}

        if (turnPartIndex >= 0)
            turnParts[turnPartIndex--].SetActive(false);
			
		// unchilding head
		part.transform.DetachChildren ();
	
		part.SetActive (false);
		part.transform.localScale = new Vector3 (1, 1, 1);
		part.transform.localRotation = new Quaternion (0, 0, 0, 0);
		part.transform.localPosition = new Vector3 (0, 0, 0);

		partIndex--;

		if (partIndex > 0) {
			part = parts [partIndex - 1];

			head.transform.parent = part.transform;

			head.transform.localScale = new Vector3 (1, 1f / part.transform.localScale.y, head.transform.localScale.z);	// set head scale reletive to the part 
			head.transform.localRotation = new Quaternion(0, 0, 0, 0);
			head.transform.localPosition = new Vector3 (0, 1, -1);

            lastRoutine = StartCoroutine(MovePlayerReverse (part));
		} else {
			
			ResetHeadTransform (currentPlatformPosition.x, currentPlatformPosition.y);
			gameplayController.moving = false;	// we are not moving 
            if (middlePlatform)
            {
                OnFakeRestart();
                // gameplayController.UnsetFakeReset();
            }
            else
            {
                gameplayController.StageRestart();	// restart the stage when collision happened
                OnRestart();                        // restart player
            }
		}
	}

	IEnumerator MovePlayerToNextPlatform(GameObject part) {	// moving player to the next platform (we know part orientation, so we rotate it by 180 and change its position in appropriate way)

		part.transform.Rotate (0, 0, 180);

		switch (part.GetComponent<PlayerPart> ().PartOrientation) {

		case PlayerPart.Orientation.right:
			
			part.transform.localPosition = new Vector3 (part.transform.localPosition.x + part.transform.localScale.y, part.transform.localPosition.y, part.transform.localPosition.z);
			break;

		case PlayerPart.Orientation.left:
			
			part.transform.localPosition = new Vector3 (part.transform.localPosition.x - part.transform.localScale.y, part.transform.localPosition.y, part.transform.localPosition.z);
			break;

		case PlayerPart.Orientation.up:
			
			part.transform.localPosition = new Vector3 (part.transform.localPosition.x, part.transform.localPosition.y + part.transform.localScale.y, part.transform.localPosition.z);
			break;

		case PlayerPart.Orientation.down:
			
			part.transform.localPosition = new Vector3 (part.transform.localPosition.x, part.transform.localPosition.y - part.transform.localScale.y, part.transform.localPosition.z);
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

        turnParts[tempPartIndex].SetActive(false);

        part.SetActive (false);
		part.transform.localScale = new Vector3 (1, 1, 1);
		part.transform.localRotation = new Quaternion (0, 0, 0, 0);
		part.transform.localPosition = new Vector3 (0, 0, 0);

		tempPartIndex++;

		if (tempPartIndex < partIndex) {

			part = parts [tempPartIndex];

			if (tempPartIndex == partIndex - 1) {

				fadePanel.SetActive (true);
				StartCoroutine (fadePanel.GetComponent<FadeController> ().FadeInPanel (1f / part.transform.localScale.y, new Color (1f, 0.4f, 0f, 0f)));
			}

            lastRoutine = StartCoroutine(MovePlayerToNextPlatform (part));
		} else {
			
			// unchilding head
			part.transform.DetachChildren ();	// SetActive() works at the end of frame, so we still able to get this part

			ResetHeadTransform (nextPlatformPosition.x, nextPlatformPosition.y);
			currentPlatformPosition = nextPlatformPosition;		// we reached the target, so our next platform becomes our current platform
			gameplayController.moving = false;
            if (middlePlatform)
                OnFakeRestart();
            else
            {
                startPlatformPosition = currentPlatformPosition;
                OnRestart();
            }
		}
	}
}
