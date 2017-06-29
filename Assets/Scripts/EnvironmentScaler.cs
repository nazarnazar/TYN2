using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentScaler : MonoBehaviour {

    float screenHeight;
    float screenWidth;
    float scaleHeightFactor;
    float scaleWidthFactor;

    public float resolutionFactor;


	void Awake ()
    {
        screenWidth = Screen.width;
        screenHeight = Screen.height;
        scaleWidthFactor = screenWidth / 16;
        scaleHeightFactor = screenHeight / 9;
        resolutionFactor = scaleWidthFactor / scaleHeightFactor;
        transform.localScale = new Vector3(resolutionFactor, resolutionFactor, 1);
	}
}
