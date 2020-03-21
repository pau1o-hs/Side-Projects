using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CS_Event {

    [Header("Actions")]
    public CS_Item[] item;
    public Button[] buttons;
    public Lever[] levers;
    public CS_PressurePlate[] pressurePlates;

    [Header("Consequences")]
    public GameObject[] dropObject;
    public Door[] door;
    public Platform[] platform;
}

[System.Serializable]
public class Lever {

    public Alavanca lever;
    public bool active;
}

[System.Serializable]
public class Door {

    public Animator doorAnim;
    public Transform toDoor;
}

[System.Serializable]
public class Platform {

    public Transform _object;
    public Transform targetTransform;
    public float moveSpeed, rotateSpeed;
}
