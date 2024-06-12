using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace AN
{
    public class WeaponItem : Item
    {
        //Todo: ANIMATOR CONTROLLER OVERRIDE (Change attack animations base on current weapon

        [Header("Weapon Model")] public GameObject weaponModel;
        
        [Header("Weapon Mechanic Option")] 
        public bool canAim = false;

        public string aim_State;
        public bool isRequiredTwoHand = false;
        
        [Header("Weapon Equip Option")] 
        public bool mainHandEquipable = true;
        public bool offHandEquipable = true;
        
        [Header("Weapon Requirement")] 
        public int levelREQ = 0;

        [Header("Weapon Guard Absorption")]
        public int guardAbsorption = 10;
        
        [Header("Weapon base Damage")]
        public int physicalDamage = 0; // Standard, Slash, Pierce, Crush
        public int magicDamage = 0;
        public int pyroDamage = 0; // Electro
        public int hydroDamage = 0; // Ice
        public int geoDamage = 0; 
        public int luminaDamage = 0; 
        public int eclipeDamage = 0;
        
        [Header("Weapon base poise Damage")]
        public int poiseDamage = 10;
        
        //WEAPON MODIFIERS
        [Header("Attack Modifier")]
        //Light Atk
        public float lightAttack01Modifier = 1f;
        public float lightAttack02Modifier = 1.1f;
        //Heavy Atk
        public float heavyAttack01Modifier = 1.5f;
        public float heavyAttack02Modifier = 1.8f;
        //Charge Atk
        public float chargeAttack01Modifier = 2f;
        public float chargeAttack02Modifier = 2.5f;
        //Run Atk
        public float runningLightAttackModifier = 1.2f;
        public float runningHeavyAttackModifier = 2f;
        
        //Critical Damage Modifier

        [Header("Stamina Cost Modifier")] 
        public int baseStaminaCost = 20;
        //Running Attack Stamina Cost Modifier
        public float runningLightAttackStaminaCostMultiplier = 1;
        public float runningHeavyAttackStaminaCostMultiplier = 1.5f;
        //Light Attack Stamina Cost Modifier
        public float lightAttackStaminaCostMultiplier = 0.8f;
        //Heavy Attack Stamina Cost Modifier
        public float heavyAttackStaminaCostMultiplier = 1.5f;
        //Charge Attack Stamina Cost Modifier
        public float chargeAttackStaminaCostMultiplier = 2f;

        // Item Base Action
        [Header("Actions")] 
        public WeaponItemAction oh_Attack_Action;//One Hand Attack Action
        public WeaponItemAction oh_Heavy_Attack_Action;//One Hand Heavy Attack Action

        // Weapon Skill

        // Blocking sound fx

        
        public void HideWeaponModel()
        {
            weaponModel.SetActive(false);
        }
        
        public void ShowWeaponModel()
        {
            weaponModel.SetActive(true);
        }
    }
}
