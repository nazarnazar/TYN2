using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Turns Panel.
/// Allow us to control number of parts on top of the screen; that show us how many directions do we have.
/// On object: UI Canvas/Turns Panel.
/// </summary>

public class TurnsPanel : MonoBehaviour {

    public GameObject panelPart;
    public PlayerController playerController;
    public float yPartScaleOffset;

    RectTransform rt;
    int partsNumber;
    List<GameObject> parts = new List<GameObject>();

	void Start ()
    {
        rt = GetComponent<RectTransform>();
        partsNumber = playerController.maxNumberOfParts;
        InstantiateParts(partsNumber);
	}

    void InstantiateParts(int max)
    {
        for (int i = 0; i < max; i++)
        {
            GameObject tempObj = Instantiate(panelPart);
            tempObj.SetActive(false);
            tempObj.transform.SetParent(transform, false);
            tempObj.GetComponent<RectTransform>().sizeDelta = new Vector2(rt.sizeDelta.x / max, rt.sizeDelta.y- yPartScaleOffset);
            tempObj.GetComponent<RectTransform>().pivot = new Vector2(-i, 0.5f);
            parts.Add(tempObj);
        }
    }

    public void ActivateTurnsPanel(int num)
    {
        for (int i = 0; i < partsNumber; i++)
            parts[i].SetActive(false);

        for (int i = 0; i < num; i++)
            parts[i].SetActive(true);
    }
	
	void Update ()
    {}
}
