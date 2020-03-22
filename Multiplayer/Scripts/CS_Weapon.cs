using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CS_Weapon {

    public GameObject graphics;
    [HideInInspector] public Transform bulletSpawn;

    public string weaponName;
    public float fireRate, recoil;
    public int damage, fireRange;
}
