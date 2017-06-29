using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIComixChanger : MonoBehaviour {

    public Sprite[] pages;
    int numOfPages;
    int counter;

    void Start()
    {
        numOfPages = pages.Length;
        counter = 0;
        GetComponent<Image>().sprite = pages[counter++];
    }

    public void NextSparite()
    {
        if (counter < numOfPages)
            GetComponent<Image>().sprite = pages[counter++];
    }

    public void RefreshPages()
    {
        counter = 0;
        GetComponent<Image>().sprite = pages[counter++];
    }
}
