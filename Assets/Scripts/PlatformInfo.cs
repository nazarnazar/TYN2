using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
