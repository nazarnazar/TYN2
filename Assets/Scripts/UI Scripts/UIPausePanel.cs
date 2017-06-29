using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UI Pause Panel.
/// Allow us to move out and in our pause panel and pause, unoause the game.
/// On object: UI Canvas/Pause Panel.
/// </summary>

public class UIPausePanel : MonoBehaviour {

    public GameplayController gameplayController;
    public float speed;

    RectTransform rt;


	void Start ()
    {
        rt = GetComponent<RectTransform>();
	}

    public void MovePanelIn()
    {
        StartCoroutine(MovingPanelIn());
        gameplayController.Pause();
    }

    IEnumerator MovingPanelIn()
    {
        while (rt.pivot.x > 0.1)
        {
            rt.pivot = new Vector2(rt.pivot.x - speed, rt.pivot.y);
            yield return null;
        }
    }

    public void MovePanelOut()
    {
        StartCoroutine(MovingPanelOut());
        gameplayController.UnPause();
    }

    IEnumerator MovingPanelOut()
    {
        while (rt.pivot.x < 1.1)
        {
            rt.pivot = new Vector2(rt.pivot.x + speed, rt.pivot.y);
            yield return null;
        }
    }
}
