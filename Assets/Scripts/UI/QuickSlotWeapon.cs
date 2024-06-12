using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AN
{
    public class QuickSlotWeapon : QuickSlotItem
    {
        [SerializeField] public bool isEquiping = false;

        public override void StartCooldown(float characterWeaponSwitchCooldownTime)
        {
            if(!isEquiping && itemIcon.enabled)
            {
                cooldownObject.SetActive(true);
            }
            isCooldown = true;
            cooldownSlider.maxValue = characterWeaponSwitchCooldownTime;
            cooldownSlider.value = characterWeaponSwitchCooldownTime;
            currentCooldownTime = characterWeaponSwitchCooldownTime;
        }
    }
}
