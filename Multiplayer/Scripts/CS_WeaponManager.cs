using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CS_WeaponManager : MonoBehaviour {

    [SerializeField] private CS_Weapon primaryWeapon;
    [SerializeField] private Transform weaponHolder;

    private CS_Weapon currentWeapon;
    private CS_WeaponGraphics currentGraphics; 

    // Use this for initialization
    void Start () {

        EquipWeapon(primaryWeapon);
	}
	
    public CS_Weapon GetCurrentWeapon ()
    {
        return currentWeapon;
    }

    public CS_WeaponGraphics GetCurrentGraphics()
    {
        return currentGraphics;
    }

    // Update is called once per frame
    void EquipWeapon (CS_Weapon _weapon) {

        GameObject _weaponInst = Instantiate(_weapon.graphics, weaponHolder.position, weaponHolder.rotation);
        _weaponInst.transform.SetParent(weaponHolder);
        _weapon.bulletSpawn = _weaponInst.transform.Find("Fire Point");
        
        currentWeapon = _weapon;
        currentGraphics = _weaponInst.GetComponent<CS_WeaponGraphics>();
        currentGraphics.m_Collider.SetParent(transform.Find("Colliders"));
        GetComponent<NetworkTransformChild>().target = currentGraphics.m_Collider;
    }
}
