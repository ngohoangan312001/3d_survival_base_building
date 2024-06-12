using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace AN
{
    public class PlayerUIHudManager : MonoBehaviour
    {
        [Header("Stat Bar")]
        [SerializeField] private UI_StatBar healthBar;
        [SerializeField] private UI_StatBar staminaBar;
        [SerializeField] private UI_StatBar energyBar;
        
        [Header("Quick Slot")]
        [SerializeField] private QuickSlotWeapon[] quickSlot;
        
        //Health Bar
        public void SetNewHealthValue(float oldValue, float newValue)
        {
            healthBar.SetStat(Mathf.RoundToInt(newValue));
        }
        
        public void SetMaxHealthValue(int maxValue)
        {
            healthBar.SetMaxStat(maxValue);
        }
        
        //Energy Bar
        public void SetNewEnergyValue(float oldValue, float newValue)
        {
            energyBar.SetStat(Mathf.RoundToInt(newValue));
        }
        
        public void SetMaxEnergyValue(int maxValue)
        {
            energyBar.SetMaxStat(maxValue);
        }
        
        //Stamina Bar
        public void SetNewStamninaValue(float oldValue, float newValue)
        {
            staminaBar.SetStat(Mathf.RoundToInt(newValue));
        }
        
        public void SetMaxStamninaValue(int maxValue)
        {
            staminaBar.SetMaxStat(maxValue);
        }


        public void RefreshHUD()
        {
            this.gameObject.SetActive(false);
            this.gameObject.SetActive(true);
        }

        public void SetQuickSlotIcon(int[] weaponIds, int activeSlotIndex = - 1)
        {
            for (int i = 0; i < weaponIds.Length; i++)
            {
                WeaponItem weapon = WorldItemDatabase.Instance.GetWeaponByID(weaponIds[i]);
                if(weapon == null)
                {
                    Debug.Log("Item In Slot " + i + " Is Null!");
                    quickSlot[i].itemIcon.enabled = false;
                    quickSlot[i].itemIcon.sprite = null;
                    
                    continue;
                }
                
                if(weapon.itemIcon == null)
                {
                    Debug.Log("Item In Slot " + i + " Has No Icon!");
                    quickSlot[i].itemIcon.enabled = false;
                    quickSlot[i].itemIcon.sprite = null;
                    
                    continue;
                }

                if (activeSlotIndex == i)
                {
                    quickSlot[i].isEquiping = true;
                    quickSlot[i].backGroundImage.color = Color.green;
                }
                else
                {
                    quickSlot[i].isEquiping = false;
                    quickSlot[i].backGroundImage.color = Color.black;
                }
                
                quickSlot[i].itemIcon.enabled = true;
                quickSlot[i].itemIcon.sprite = weapon.itemIcon;
            }
        }
        
        public void StartCoolDownWeaponSwitch(float characterWeaponSwitchCooldownTime){

            foreach (var slot in quickSlot)
            {
                slot.StartCooldown(characterWeaponSwitchCooldownTime);
            }
        }
    }
}
