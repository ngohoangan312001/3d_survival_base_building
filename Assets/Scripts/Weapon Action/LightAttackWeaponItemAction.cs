using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AN
{
    
    [CreateAssetMenu(menuName = "Character Actions/Weapon Actions/Light Attack Action")]
    public class LightAttackWeaponItemAction : WeaponItemAction
    {
        [SerializeField] 
        private string light_Attack_01 = "Main_Light_Attack_01";
        private string light_Attack_02 = "Main_Light_Attack_02";
        public override void AttempToPerformAction(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
        {
            base.AttempToPerformAction(playerPerformingAction, weaponPerformingAction);
            
            //If not the owner return, not perform the animation and the modifier 
            //==> only oner will need to do that
            if (!playerPerformingAction.IsOwner) return; 
            
            //Check To Stop Action
            if (playerPerformingAction.playerNetworkManager.currentStamina.Value <= 0) 
                return;
            
            if (!playerPerformingAction.isGrounded) 
                return;

            PerformLightAttackAction(playerPerformingAction, weaponPerformingAction);
        }
        
        public void PerformLightAttackAction(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
        {
            //If Character is attacking and can do combo => perform combo attack
            if (playerPerformingAction.playerCombatManager.canComboWithMainHandWeapon && playerPerformingAction.isPerformingAction)
            {
                playerPerformingAction.playerCombatManager.canComboWithMainHandWeapon = false;
                
                //Perform attack base on previous attack
                if (playerPerformingAction.playerCombatManager.lastAttackAnimationPerformed == light_Attack_01)
                {
                    playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(AttackType.LightAttack02,light_Attack_02,true); 
                }
                else
                {
                    playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(AttackType.LightAttack01,light_Attack_01,true);
                }
                
            }
            else if(!playerPerformingAction.isPerformingAction)
            {
                playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(AttackType.LightAttack01,light_Attack_01,true);
            }
        }
    }
}
