using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gameplay controller.
/// Allow us to detect: game over, time to reset current stage, control bonuses.
/// On object: Gameplay Controller.
/// Uses: PlayerController, Stage.
/// </summary>

public class GameplayController : MonoBehaviour {

	public PlayerController playerController;	// use to pause player control

	public Stage[] stage;		// stages that we have on one level
	int stageCounter;			// stages counter
	float currentStageTimer;	// each stage has start time to get to the target
	int lives;					// lives per level (or per stage)

	bool pause = true;			// is our game paused
	bool timeOver = false;		// is our time overed

	bool fakeReset = false;		// reset is going to be fake when 'true'

	public bool moving = false;	// player doing some coroutine

	string status;				// game status (playing, won, lost)

	bool restarted = true;		// we can't begin game if there was no restart

	void Start ()
	{
		status = "Playing!";
		stageCounter = 0;
		currentStageTimer = stage [stageCounter].GetStageTimer ();
		lives = stage [stageCounter].GetStageLives ();
	}

	void Update ()
	{
		if (!pause && !timeOver && !moving)		// if 'true' then the time is running
		{
			currentStageTimer -= Time.deltaTime;
			if (currentStageTimer <= 0)			// if time is over, we need to simulate collision and make MinusLife()
			{
				print ("Minus life!");
				timeOver = true;
				playerController.CollisionHappened ();
				MinusLife ();
			}
		}
	}

	public void StageRestart()
	{
		/*
		 * if fakeReset == true - we didn't lose life, so our try continues and we need only to reset our direction bonuses;
		 * it's okay because when collision happens (and we need to reset) player resets its start number of parts he can spawn on curret stage.
		 * We don't need to reset time bonuses, because, if we reset time, palyer will be able to collect time bonuses without loosing lives.
		*/
		if (fakeReset)
		{
			fakeReset = false;
			stage [stageCounter].ResetDirBonuses ();
		}
		else 	// Everything is okay, player lost his life, so we reset everything
		{
			stage [stageCounter].ResetDirBonuses ();
			stage [stageCounter].ResetTimeBonuses ();
			currentStageTimer = stage [stageCounter].GetStageTimer ();
			timeOver = false;
		}
	}

	public void NextStage()	// going to the next stage
	{
		stageCounter++;
		if (stageCounter >= stage.Length)
		{
			print ("You won!");
			status = "YouWon!";
			pause = true;
			playerController.PauseGame = true;
		} 
		else
			currentStageTimer = stage [stageCounter].GetStageTimer ();
	}

	public void AddTime(float time)		// we get time bonus, so we add time
	{
		currentStageTimer += time;
	}

	public void MinusLife()			// we get minus life, when we collide with dangerous obstacle or we have no time left
	{
		if (fakeReset)
		{
			fakeReset = false;
			StageRestart ();
			fakeReset = true;
		}

		lives--;
		if (lives == 0)
		{
			print ("Game Over!");
			status = "GameOver!";
			pause = true;
			playerController.PauseGame = true;
		}
	}

	public int StageStartPartsNumber()		// returns start parts number we can spawn on current stage
	{
		if (stageCounter < stage.Length)
			return stage [stageCounter].startPartsNumber;
		else
			return 0;
	}

	public void SetFakeReset()		// next reset will be fake
	{
		fakeReset = true;
	}

	void OnGUI()
	{
		GUI.skin.label.fontSize = 40;
		GUI.skin.button.fontSize = 40;

		GUI.Label (new Rect (10, 10, 200, 50), "Time: " + Mathf.RoundToInt (currentStageTimer));
		GUI.Label (new Rect (10, 60, 200, 50), "Lives: " + lives);

		GUI.Label (new Rect (10, 160, 400, 50), "Status: " + status);

		if (GUI.Button (new Rect (10, 230, 150, 50), "Restart"))
			Application.LoadLevel (0);
		
		if (GUI.Button (new Rect (170, 230, 150, 50), "Begin"))
		{
			if (pause && playerController.PauseGame && restarted)
			{
				restarted = false;

				playerController.PauseGame = false;
				pause = false;
				stage [stageCounter].BeginMovingObstacles ();
			}
		}
	}
}
