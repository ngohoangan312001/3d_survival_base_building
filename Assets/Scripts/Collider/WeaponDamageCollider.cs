using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace AN
{
    public class WeaponDamageCollider : DamageCollider
    {
        public WeaponItem currentWeapon;
        
        [Header("Attacking Charater")] 
        public CharacterManager characterCausingDamage; 
        
        [Header("Weapon Attack Modifier")] 
        //Light Attack
        public float lightAttack01Modifier;
        public float lightAttack02Modifier;
        //Heavy Attack
        public float heavyAttack01Modifier;
        public float heavyAttack02Modifier;
        //Charge Attack
        public float chargeAttack01Modifier;
        public float chargeAttack02Modifier;
        //Running Attack
        public float runningLightAttackModifier;
        public float runningHeavyAttackModifier;

        protected override void DamageTarget(CharacterManager damageTarget)
        {
            //return if the target have been damaged by this
            //so the target will not be damaged more than once in a single attack
            if (characterDamaged.Contains(damageTarget)) return;
            
            characterDamaged.Add(damageTarget);

            TakeDamageEffect damageEffect = Instantiate(WorldCharacterEffectManager.Instance.takeDamageEffect);
                
            PassDamageToDamageEffect(damageEffect);
            damageEffect.angleHitFrom = Vector3.SignedAngle(characterCausingDamage.transform.forward, damageTarget.transform.forward, Vector3.up);
            
            switch (characterCausingDamage.characterCombatManager.currentAttackType)
            {
                case AttackType.LightAttack01:
                    ApplyAttackModifier(lightAttack01Modifier, damageEffect);
                    break;
                case AttackType.LightAttack02:
                    ApplyAttackModifier(lightAttack02Modifier, damageEffect);
                    break;
                case AttackType.HeavyAttack01:
                    ApplyAttackModifier(heavyAttack01Modifier, damageEffect);
                    break;
                case AttackType.HeavyAttack02:
                    ApplyAttackModifier(heavyAttack02Modifier, damageEffect);
                    break;
                case AttackType.ChargeAttack01:
                    ApplyAttackModifier(chargeAttack01Modifier, damageEffect);
                    break;
                case AttackType.ChargeAttack02:
                    ApplyAttackModifier(chargeAttack02Modifier, damageEffect);
                    break;
                case AttackType.RunningLightAttack:
                    ApplyAttackModifier(runningLightAttackModifier, damageEffect);
                    break;
                case AttackType.RunningHeavyAttack:
                    ApplyAttackModifier(runningHeavyAttackModifier, damageEffect);
                    break;
                default:
                    break;
            }
            
            //damageTarget.characterEffectManager.ProcessInstanceEffect(damageEffect);
            
            //FOR NETWORK
            if (characterCausingDamage.IsOwner)
            {
                damageTarget.characterNetworkManager.NotifyServerOfCharacterDamageServerRPC
                    (
                        damageTarget.NetworkObjectId,
                        characterCausingDamage.NetworkObjectId,
                        damageEffect.physicalDamage,
                        damageEffect.magicDamage,
                        damageEffect.pyroDamage,
                        damageEffect.hydroDamage,
                        damageEffect.geoDamage,
                        damageEffect.luminaDamage,
                        damageEffect.eclipseDamage,
                        damageEffect.poiseDamage,
                        damageEffect.angleHitFrom,
                        damageEffect.contactPoint.x,
                        damageEffect.contactPoint.y,
                        damageEffect.contactPoint.z
                    );
            }
            
        }

        protected virtual void ApplyAttackModifier(float modifier, TakeDamageEffect damageEffect)
        {
            damageEffect.physicalDamage *= modifier;
            damageEffect.magicDamage *= modifier;
            damageEffect.pyroDamage *= modifier; 
            damageEffect.hydroDamage *= modifier;
            damageEffect.geoDamage *= modifier;
            damageEffect.luminaDamage *= modifier;
            damageEffect.eclipseDamage *= modifier;

            damageEffect.poiseDamage *= modifier;
            
            //Todo: Apply full charge attack modifier after if attack is fully charge
        }
    }
}
