using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILocationChanger : MonoBehaviour {

    public GameObject[] locationPanels;
    public float speed;

    int placer;


    void Start()
    {
        placer = 1;
    }

    public void MovePanelsRight()
    {
        if (placer < locationPanels.Length)
        {
            foreach (var item in locationPanels)
                StartCoroutine(MovingLeft(item));
            placer++;
        }
    }

    public void MovePanelsLeft()
    {
        if (placer > 1)
        {
            foreach (var item in locationPanels)
                StartCoroutine(MovingRight(item));
            placer--;
        }
    }

    public IEnumerator MovingRight(GameObject panel)
    {
        RectTransform rt = panel.GetComponent<RectTransform>();
        float target = rt.pivot.x - 1f;

        while (rt.pivot.x > target)
        {
            rt.pivot = new Vector2(rt.pivot.x - speed, rt.pivot.y);
            yield return null;
        }
    }

    public IEnumerator MovingLeft(GameObject panel)
    {
        RectTransform rt = panel.GetComponent<RectTransform>();
        float target = rt.pivot.x + 1f;

        while (rt.pivot.x < target)
        {
            rt.pivot = new Vector2(rt.pivot.x + speed, rt.pivot.y);
            yield return null;
        }
    }
}
