using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AN
{
    [CreateAssetMenu(menuName = "Items/Weapon Item/Range Weapon")]
    public class RangeWeaponItem : WeaponItem
    
    {
        [Header("Weapon Range Option")] 
        public float maxDistance;
        public int attackRate;
        public int reloadTime;
        public float timeEachAttack = 0;
        
        [Header("Ammunition")]
        public bool needAmmo = true;
        public int currentAmmo;
        public int magSize;
        public float timeSinceLastAttack = 0;
        
        [HideInInspector] public bool isReloading = false;
        //Weapon Deflection ( weapon will be bounce off another weapon if being guard )
        
        //Can be buffed

        public bool CanFire()
        {
            timeEachAttack = 1f / (attackRate / 60f);
            // attackRate       => Attack per minute
            // attackRate/60    => Attack per second
            // 1/attackRate/60  => Time between each attack in 1 second
            if (timeSinceLastAttack < (1f/attackRate/60f))
            {
                Debug.Log("Attack To Fast !!! "+ ((1f/attackRate/60f) - timeSinceLastAttack) + " In Cooldown");
                return false;
            }
            
            if (needAmmo && currentAmmo <= 0)
            {
                Debug.Log("Out of Ammo");
                return false;
            }   

            if (isReloading)
            {
                Debug.Log("Reloading");
                return false;
            }
            
            
            return true;
        }
    }
}
