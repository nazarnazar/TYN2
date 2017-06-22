using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// <summary>
// PickUp Info.
// Contains informaition about parent platform on which this pickup is.
// On object: PickUp.
// Uses: -.
// </summary>


    //На будущее:
//можно сделать абстстрактный класс "пикапинфо" и наследоваться от него для каждого отдельного вида пикапа
//в этом классе сделать виртуальную функцию UsePickUp(), и там уже давать определенные бонусы в зависимости от пикапа
//тогда тот-же TimeBonus можно просто наследовать от пикапинфо и т.д.


public class PickUpInfo : MonoBehaviour {

    [Tooltip("Platform where the pick up is")]
    public GameObject ParentPlatform;
}
