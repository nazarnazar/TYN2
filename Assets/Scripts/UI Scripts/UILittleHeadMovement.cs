using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UI Little Head Movement.
/// Allow us to move mini UI head.
/// On object: UI Canvas/Stage Panel/Head.
/// </summary>

public class UILittleHeadMovement : MonoBehaviour {

    public int stagesNum;
    public GameObject parentTransform;
    public float headOffset;
    public float yHeadOffset;
    public float speed;

    RectTransform rt;
    float mult;
    int indexPos;

    void Start()
    {
        rt = GetComponent<RectTransform>();
        mult = parentTransform.GetComponent<RectTransform>().sizeDelta.x / rt.sizeDelta.x / stagesNum;
        indexPos = 0;

        rt.pivot = new Vector2(-(mult * indexPos + headOffset), yHeadOffset);
        indexPos++;
    }

    public IEnumerator MoveMiniHead()
    {
        while (rt.pivot.x > -(mult * indexPos + headOffset))
        {
            rt.pivot = new Vector2(rt.pivot.x - speed, rt.pivot.y);
            yield return null;
        }
        indexPos++;
    }
}
