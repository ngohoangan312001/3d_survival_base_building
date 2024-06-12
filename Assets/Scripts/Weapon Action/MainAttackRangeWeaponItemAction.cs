using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AN
{
    
    [CreateAssetMenu(menuName = "Character Actions/Weapon Actions/Main Range Attack Action")]
    public class MainAttackRangeWeaponItemAction : WeaponItemAction
    {
        [SerializeField] private string main_Attack = "Main_Range_Attack";
        public override void AttempToPerformAction(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
        {
            base.AttempToPerformAction(playerPerformingAction, weaponPerformingAction);
            
            //If not the owner return, not perform the animation and the modifier 
            //==> only oner will need to do that
            if (!playerPerformingAction.IsOwner) return; 
            
            //Check To Stop Action
            if (playerPerformingAction.playerNetworkManager.currentStamina.Value <= 0) 
                return;

            if (weaponPerformingAction is RangeWeaponItem rangeWeaponPerformingAction)
            {
                if (!rangeWeaponPerformingAction.CanFire())
                {
                    return;
                }
            }

            PerformLightAttackAction(playerPerformingAction, weaponPerformingAction);
        }
        
        public void PerformLightAttackAction(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
        {
            if (playerPerformingAction.playerNetworkManager.isUsingRightHand.Value)
            {
                    playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(AttackType.LightAttack01,
                        main_Attack, true,false,true,true);

                return;
            }
            
            if (playerPerformingAction.playerNetworkManager.isUsingLeftHand.Value)
            {
                return;
            }
        }
    }
}
