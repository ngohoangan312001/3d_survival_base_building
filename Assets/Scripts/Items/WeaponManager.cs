using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace AN
{
    public class WeaponManager : MonoBehaviour
    {
        public WeaponDamageCollider weaponDamageCollider;

        private void Awake()
        {
            weaponDamageCollider = GetComponentInChildren<WeaponDamageCollider>();
        }

        public void SetWeaponDamage(CharacterManager characterWieldingWeapon, WeaponItem weapon)
        {
            weaponDamageCollider.characterCausingDamage = characterWieldingWeapon;
            weaponDamageCollider.physicalDamage = weapon.physicalDamage;
            weaponDamageCollider.magicDamage = weapon.magicDamage;
            weaponDamageCollider.pyroDamage = weapon.pyroDamage; 
            weaponDamageCollider.hydroDamage = weapon.hydroDamage;
            weaponDamageCollider.geoDamage = weapon.geoDamage;
            weaponDamageCollider.luminaDamage = weapon.luminaDamage;
            weaponDamageCollider.eclipeDamage = weapon.eclipeDamage;
            
            weaponDamageCollider.poiseDamage = weapon.poiseDamage;
            
            //ATTACK MODIFIER
            //Light Atk
            weaponDamageCollider.lightAttack01Modifier = weapon.lightAttack01Modifier;
            weaponDamageCollider.lightAttack02Modifier = weapon.lightAttack02Modifier;
            //Heavy Atk
            weaponDamageCollider.heavyAttack01Modifier = weapon.heavyAttack01Modifier;
            weaponDamageCollider.heavyAttack02Modifier = weapon.heavyAttack02Modifier;
            //Charge Atk
            weaponDamageCollider.chargeAttack01Modifier = weapon.chargeAttack01Modifier;
            weaponDamageCollider.chargeAttack02Modifier = weapon.chargeAttack02Modifier;
            //Run Atk
            weaponDamageCollider.runningLightAttackModifier = weapon.runningLightAttackModifier;
            weaponDamageCollider.runningHeavyAttackModifier = weapon.runningHeavyAttackModifier;

            weaponDamageCollider.currentWeapon = weapon;
        }
    }
}
