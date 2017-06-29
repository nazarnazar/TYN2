using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIChangeTutorialImage : MonoBehaviour {

    public Sprite[] pages;
    int counter;

	void Start ()
    {
        counter = 0;
        GetComponent<Image>().sprite = pages[counter++];
	}

    public void NextSparite()
    {
        GetComponent<Image>().sprite = pages[counter++];
    }

    public void RefreshCounter()
    {
        counter = 0;
        GetComponent<Image>().sprite = pages[counter++];
    }
}
