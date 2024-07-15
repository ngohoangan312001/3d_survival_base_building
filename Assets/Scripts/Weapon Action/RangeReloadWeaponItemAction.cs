using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace AN
{
    
    [CreateAssetMenu(menuName = "Character Actions/Weapon Actions/Range Reload Action")]
    public class RangeReloadWeaponItemAction : WeaponItemAction
    {
        [SerializeField] private string weapon_reload = "Main_Range_Reload";
        public override void AttempToPerformAction(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
        {
            base.AttempToPerformAction(playerPerformingAction, weaponPerformingAction);
            
            //If not the owner return, not perform the animation and the modifier 
            //==> only owner will need to do that
            if (!playerPerformingAction.IsOwner) return; 
            
            if (weaponPerformingAction is RangeWeaponItem rangeWeaponPerformingAction)
            {
                if (!rangeWeaponPerformingAction.CanReload())
                {
                    return;
                }
                
                PerformReloadAction(playerPerformingAction, rangeWeaponPerformingAction);
            }

            
        }
        
        public void PerformReloadAction(PlayerManager playerPerformingAction, RangeWeaponItem weaponPerformingAction)
        {
            if (playerPerformingAction.playerNetworkManager.isUsingRightHand.Value)
            {
                weaponPerformingAction.currentReloadProgressTime = 0;
                
                playerPerformingAction.playerAnimatorManager.PlayTargetActionAnimation(
                        weapon_reload,true,false,true,true);
                return;
            }
            
            if (playerPerformingAction.playerNetworkManager.isUsingLeftHand.Value)
            {
                return;
            }
        }
    }
}
