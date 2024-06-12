using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace AN
{
    public class PlayerCombatManager : CharacterCombatManager
    {
        private PlayerManager player;
        public WeaponItem currentWeaponBeingUsed;

        [Header("Flags")] 
        public bool canComboWithMainHandWeapon;
        //public bool canComboWithOffHandWeapon;
        
        protected override void Awake()
        {
            base.Awake();
            player = GetComponent<PlayerManager>();
        }

        public void PerformWeaponBaseAction(WeaponItemAction weaponAction, WeaponItem weaponPerformingAction)
        {
            if (player.IsOwner)
            {
                //Perform weapon action
                weaponAction.AttempToPerformAction(player,weaponPerformingAction);
            }
            
            //Notify the server to play animation
            player.playerNetworkManager.NotifyServerOfWeaponActionServerRPC(NetworkManager.Singleton.LocalClientId, weaponAction.actionId,weaponPerformingAction.itemId);
        }

        public void DrainStaminaBaseOnAttack()
        {
            if (!player.IsOwner)
                return;

            if (currentWeaponBeingUsed == null) 
                return;
            
            float staminaDeducted = 0;

            switch (currentAttackType)
            {
                case AttackType.LightAttack01:
                    staminaDeducted = currentWeaponBeingUsed.baseStaminaCost *
                                      currentWeaponBeingUsed.lightAttackStaminaCostMultiplier;
                    break;
                // case AttackType.LightAttack02:
                //     staminaDeducted = currentWeaponBeingUsed.baseStaminaCost *
                //                       currentWeaponBeingUsed.lightAttackStaminaCostMultiplier;
                //     break;
                case AttackType.HeavyAttack01:
                    staminaDeducted = currentWeaponBeingUsed.baseStaminaCost *
                                      currentWeaponBeingUsed.heavyAttackStaminaCostMultiplier;
                    break;
                // case AttackType.HeavyAttack02:
                //     staminaDeducted = currentWeaponBeingUsed.baseStaminaCost *
                //                       currentWeaponBeingUsed.heavyAttackStaminaCostMultiplier;
                //     break;
                case AttackType.ChargeAttack01:
                    staminaDeducted = currentWeaponBeingUsed.baseStaminaCost *
                                      currentWeaponBeingUsed.chargeAttackStaminaCostMultiplier;
                    break;
                // case AttackType.ChargeAttack02:
                //     staminaDeducted = currentWeaponBeingUsed.baseStaminaCost *
                //                       currentWeaponBeingUsed.chargeAttackStaminaCostMultiplier;
                //     break;
                case AttackType.RunningLightAttack:
                    staminaDeducted = currentWeaponBeingUsed.baseStaminaCost *
                                      currentWeaponBeingUsed.runningLightAttackStaminaCostMultiplier;
                    break;
                case AttackType.RunningHeavyAttack:
                    staminaDeducted = currentWeaponBeingUsed.baseStaminaCost *
                                      currentWeaponBeingUsed.runningHeavyAttackStaminaCostMultiplier;
                    break;
                default:
                    break;
            }

            player.playerNetworkManager.currentStamina.Value -= Mathf.RoundToInt(staminaDeducted);
        }

        public override void SetTarget(CharacterManager newTarget)
        {
            base.SetTarget(newTarget);
            PlayerCamera.Instance.SetLockCameraHeight();
        }

    }
}
