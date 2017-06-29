using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI Game Over Panel.
/// Allow us to fade in game over panel and fade in stars we got.
/// On object: UI Canvas/Game Over Panel.
/// </summary>

public class UIGameOverPanel : MonoBehaviour {

    public GameObject[] stars;
    public float speed;

    RectTransform rt;


    void Start()
    {
        rt = GetComponent<RectTransform>();
    }

    public void MovePanelIn(bool won = false, int lives = 0)
    {
        StartCoroutine(MovingPanelIn(won, lives));
    }

    IEnumerator MovingPanelIn(bool won, int lives)
    {
        while (rt.pivot.y > -0.2)
        {
            rt.pivot = new Vector2(rt.pivot.x, rt.pivot.y - speed);
            yield return null;
        }
        if (won)
            FadeInStars(lives);
    }

    void FadeInStars(int lives)
    {
        StartCoroutine(FadingStarsIn(lives));
    }

    IEnumerator FadingStarsIn(int starsNum)
    {
        for (int i = 0; i < starsNum; i++)
        {
            while (stars[i].GetComponent<Image>().color.a < 1)
            {
                stars[i].GetComponent<Image>().color = new Color(1, 1, 1, stars[i].GetComponent<Image>().color.a + Time.deltaTime);
                yield return null;
            }
        }
    }
}
