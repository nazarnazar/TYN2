using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UI Lives Controller.
/// Allow us to deactivate hearts when we have minus life.
/// On object: UI Canvas/Hearts Panel.
/// </summary>

public class UILivesController : MonoBehaviour {

    public GameObject[] hearts;

    public void MinusHeart()
    {
        for (int i = hearts.Length - 1; i >= 0; i--)
        {
            if (hearts[i].activeSelf)
            {
                hearts[i].SetActive(false);
                break;
            }
        }
    }
}
