using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// <summary>
// Platform Info.
// Contains informaition about pickup if it exists on certain platform.
// On object: all kinds of platforms.
// Uses: -.
// </summary>

public class PlatformInfo : MonoBehaviour {

    [Tooltip("Add PickUp if it exists")]
    public GameObject PickUp;

    [HideInInspector]
    public bool isExist;

    void Start(){

        if (PickUp != null)
            isExist = true;
        else
            isExist = false;
    }
}
