using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    [SerializeField] Transform weaponHolder;

    [SerializeField] GunController gunHeld;

    public void SetRotation(float angle)
    {
        weaponHolder.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    public void SetWeaponHeld(GunController gun)
    {
        gunHeld = gun;
    }
    public GunController GetWeaponHeld()
    {
        return gunHeld;
    }
}
