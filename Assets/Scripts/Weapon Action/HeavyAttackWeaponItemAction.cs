using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace AN
{
    
    [CreateAssetMenu(menuName = "Character Actions/Weapon Actions/Heavy Attack Action")]
    public class HeavyAttackWeaponItemAction : WeaponItemAction
    {
        [SerializeField] private string heavy_Attack_01 = "Main_Heavy_Attack_01";
        [SerializeField] private string heavy_Attack_02 = "Main_Heavy_Attack_02";
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

            PerformHeavyAttackAction(playerPerformingAction, weaponPerformingAction);
        }
        
        public void PerformHeavyAttackAction(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
        {
            //If Character is attacking and can do combo => perform combo attack
            if (playerPerformingAction.playerCombatManager.canComboWithMainHandWeapon && playerPerformingAction.isPerformingAction)
            {
                playerPerformingAction.playerCombatManager.canComboWithMainHandWeapon = false;
                
                //Perform attack base on previous attack
                if (playerPerformingAction.playerCombatManager.lastAttackAnimationPerformed == heavy_Attack_01)
                {
                    playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(AttackType.HeavyAttack02,heavy_Attack_02,true);
                }
                else
                {
                    playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(AttackType.HeavyAttack01,heavy_Attack_01,true);
                }
                
            }
            else if(!playerPerformingAction.isPerformingAction)
            {
                playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(AttackType.HeavyAttack01,heavy_Attack_01,true);
            }
                
               
        }
    }
}
