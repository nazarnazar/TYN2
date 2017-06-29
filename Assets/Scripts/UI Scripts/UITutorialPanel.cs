using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITutorialPanel : MonoBehaviour {

    public int pagesNum;
    public UIChangeTutorialImage pageChanger;
    public GameplayController gc;
    public float speed;
    RectTransform rt;
    int myPagesCounter;
    bool tutorialOn;


    void Start()
    {
        rt = GetComponent<RectTransform>();
        MovePanel();
    }

    void Update()
    {
        if (tutorialOn)
            gc.Pause();
    }

    public void MovePanel()
    {
        tutorialOn = true;
        myPagesCounter = pagesNum;
        pageChanger.RefreshCounter();
        StartCoroutine(MovingPanel());
    }

    IEnumerator MovingPanel()
    {
        while (rt.pivot.y < 2.25)
        {
            rt.pivot = new Vector2(rt.pivot.x, rt.pivot.y + speed);
            yield return null;
        }
    }

    public void MovePanelOut()
    {
        StartCoroutine(MovingPanelOut());
    }

    IEnumerator MovingPanelOut()
    {
        while (rt.pivot.y > 1)
        {
            rt.pivot = new Vector2(rt.pivot.x, rt.pivot.y - speed);
            yield return null;
        }
        myPagesCounter--;
        if (myPagesCounter > 0)
        {
            pageChanger.NextSparite();
            StartCoroutine(MovingPanel());
        }
        else
        {
            tutorialOn = false;
            gc.UnPause();
        }
    }
}
