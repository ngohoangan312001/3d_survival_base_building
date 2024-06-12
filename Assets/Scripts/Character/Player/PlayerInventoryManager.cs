using System;
using System.Collections;
using System.Collections.Generic;
using AN;
using UnityEngine;

public class PlayerInventoryManager : CharacterInventoryManager
{
    public WeaponItem currentRightHandWeapon;
    public WeaponItem currentLeftHandWeapon;

    [Header("Quick Slot")]
    public WeaponItem[] weaponInRightHandSlots = new WeaponItem[4];
    public int rightHandWeaponIndex = -1;
    public WeaponItem[] weaponInLeftHandSlots = new WeaponItem[2];
    public int leftHandWeaponIndex = -1;

    public void HideRightHandWeapon()
    {
        currentRightHandWeapon.HideWeaponModel();
    }
    
    public void ShowRightHandWeapon()
    {
        currentRightHandWeapon.ShowWeaponModel();
    }
}
