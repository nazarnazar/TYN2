using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UI Stages Panel.
/// Allow us to spawn little UI platforms as numebr of stages we have.
/// On object: UI Canvas/Stage Panel.
/// </summary>

public class UIStagesPanel : MonoBehaviour {

    public int stagesNum;
    public GameObject littlePlatform;
    public float yPlatformScale;
    public float yPlatformOffset;

    RectTransform rt;

    void Start()
    {
        if (stagesNum <= 1)
        {
            gameObject.SetActive(false);
        }
        else
        {
            rt = GetComponent<RectTransform>();

            for (int i = 0; i < stagesNum; i++)
            {
                GameObject tempObj = Instantiate(littlePlatform);
                tempObj.SetActive(false);
                tempObj.transform.SetParent(transform, false);
                tempObj.GetComponent<RectTransform>().sizeDelta = new Vector2(rt.sizeDelta.x / stagesNum, yPlatformScale);
                tempObj.GetComponent<RectTransform>().pivot = new Vector2(-i, yPlatformOffset);
                tempObj.SetActive(true);
            }
        }
    }
}
