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
        public int reloadTime;
        public float currentReloadProgressTime;
        public float timeEachAttack = 0;
        
        [Header("Attack Per Second")] 
        public int attackRate;
        
        [Header("Ammunition")]
        public bool needAmmo = true;
        public int currentAmmo;
        public int magSize;
        public float timeSinceLastAttack = 0;
        
        [HideInInspector] public bool isReloading = false;
        
        [Header("Actions")] 
        public WeaponItemAction reload_Action;
        
        //Weapon Deflection ( weapon will be bounce off another weapon if being guard )
        
        //Can be buffed

        public bool CanFire()
        {
            timeEachAttack = GetAttackTimePerMinute();
            // attackRate       => Attack per minute
            // attackRate/60    => Attack per second
            // 1/attackRate/60  => Time between each attack in 1 second
            if (timeSinceLastAttack < timeEachAttack)
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

            if (!PlayerInputManager.Instance.aimInput)
            {
                Debug.Log("Not Aiming");
                return false;
            }
            
            return true;
        }

        public float GetAttackTimePerMinute()
        {
            return 1f / (attackRate / 60f);
        }
        
        public bool CanReload()
        {
            if (!needAmmo) return false;

            if (currentAmmo >= magSize) return false;
            
            if (currentReloadProgressTime < reloadTime) return false;
            
            return true;
        }

        public void RangeReload()
        {
            currentAmmo = magSize;
            PlayerUIManager.instance.playerUIHudManager.SetMaxRangeAmunitionValue(currentAmmo);
        }
    }
}
