using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stage.
/// Stores info about current stage of a level:
/// Stage bonuses, stage start time, stage start max number of body parts.
/// On object: Stage.
/// Uses: MovingObstacle, DirectionBonus, TimeBonus.
/// </summary>

public class Stage : MonoBehaviour {

	public GameObject[] movingObstacles;
	public GameObject[] dirBonuses;
	public GameObject[] timeBonuses;

	public int lives;
	public float timer;

	public int startPartsNumber;

	public void ResetTimeBonuses()
	{
		foreach (var item in timeBonuses)
			item.SetActive (true);
	}

	public void ResetDirBonuses()
	{
		foreach (var item in dirBonuses)
			item.SetActive (true);
	}

	public float GetStageTimer()
	{
		return timer;
	}

	public int GetStageLives()
	{
		return lives;
	}

	/* public void BeginMovingObstacles()
	{
        foreach (var item in movingObstacles)
            item.GetComponent<MovingObstacle>().StartMoving();
	} */

    public void StopMovingObstacles()
    {
        foreach (var item in movingObstacles)
        {
            if (item) item.GetComponent<MovingObstacle>().StopMoving();
        }
    }
}
