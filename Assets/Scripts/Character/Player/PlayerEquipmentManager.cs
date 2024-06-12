using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AN
{
    public class PlayerEquipmentManager : CharacterEquipmentManager
    {
        private PlayerManager player;
        public WeaponModelInstantiationSlot rightHandSlot;
        public WeaponModelInstantiationSlot leftHandSlot;

        [SerializeField] private WeaponManager rightWeaponManager;
        [SerializeField] private WeaponManager leftWeaponManager;
        
        [SerializeField] private string swapRightWeaponAnimation = "Swap_Right_Weapon";
        [SerializeField] private string swapLeftWeaponAnimation = "Swap_Left_Weapon";
        
        public GameObject rightHandWeaponModel;
        public GameObject leftHandWeaponModel;
        
        protected override void Awake()
        {
            base.Awake();

            player = GetComponent<PlayerManager>();
            
            InstantializeWeaponSlot();
        }

        protected override void Start()
        {
            loadWeaponOnBothHand();
        }

        public void InstantializeWeaponSlot()
        {
            WeaponModelInstantiationSlot[] weaponSlotList = GetComponentsInChildren<WeaponModelInstantiationSlot>();

            foreach (var weaponSlot in weaponSlotList)
            {
                    if (weaponSlot.weaponModelSlot == WeaponModelSlot.RightHand)
                    {
                        rightHandSlot = weaponSlot;
                    }
                    else if (weaponSlot.weaponModelSlot == WeaponModelSlot.LeftHand)
                    {
                        leftHandSlot = weaponSlot;
                    }
            }
        }

        public void SwitchRightHandWeapon()
        {
            if (!player.IsOwner) return;
            
            //Weapon Swapping:
            //Check if there is another weapon beside main weapon, if do, rotate bewtween weapon
            //If not, swap to unarmed then skip the remain empty slot

            WeaponItem selectedWeapon = null;
            
            //Todo: Disable 2 handing if character are 2 handing
            
            //get next weapon index to switch into
            player.playerInventoryManager.rightHandWeaponIndex += 1;
            
            //If weapon index is out of bounds, reset to 0 then check for all weapon in slot
            if (player.playerInventoryManager.rightHandWeaponIndex < 0 ||
                player.playerInventoryManager.rightHandWeaponIndex >
                player.playerInventoryManager.weaponInRightHandSlots.Length - 1)
            {
                //set to first weapon index if the index is outbound
                player.playerInventoryManager.rightHandWeaponIndex = 0;
                
                //Check if player is holding more than 1 weapon
                float weaponCount = 0;
                WeaponItem firstWeapon = null;
                int firstWeaponIndex = 0;
                
                for (int i = 0; i < player.playerInventoryManager.weaponInRightHandSlots.Length; i++)
                {
                    if (player.playerInventoryManager.weaponInRightHandSlots[i].itemId !=
                        WorldItemDatabase.Instance.unarmedWeapon.itemId)
                    {
                        //If this weapon in slot is not unarmed, weapon count + 1
                        weaponCount++;

                        if (firstWeapon == null)
                        {
                            //If first weapon variable is null => then this is first weapon
                            firstWeapon = player.playerInventoryManager.weaponInRightHandSlots[i];
                            firstWeaponIndex = i;
                        }
                    }
                }
                
                //if there is only 1 weapon, next weapon switch will be unarmed and ser the index to -1 => weapon index is outbound
                if (weaponCount <= 1)
                {
                    //Set weapon index to outbound
                    player.playerInventoryManager.rightHandWeaponIndex = -1;
                    selectedWeapon = WorldItemDatabase.Instance.unarmedWeapon;
                    player.playerNetworkManager.currentRightHandWeaponId.Value = selectedWeapon.itemId;
                }
                //If there is more than 1 weapon
                else
                {
                    //Get the first weapon
                    player.playerInventoryManager.rightHandWeaponIndex = firstWeaponIndex;
                    //Set network right hand weapon index to first weapon
                    player.playerNetworkManager.currentRightHandWeaponId.Value = firstWeapon.itemId;
                    
                    //Animation
                    player.playerAnimatorManager.PlayTargetActionAnimation(swapRightWeaponAnimation,false, false, true, true);

                }

                return;
            }

            //If index is not outnound check next weapon
            foreach (WeaponItem weapon in player.playerInventoryManager.weaponInRightHandSlots)
            {
                //check if next weapon slot is not unarmed weapon => get that weapon
                if (player.playerInventoryManager
                        .weaponInRightHandSlots[player.playerInventoryManager.rightHandWeaponIndex].itemId !=
                    WorldItemDatabase.Instance.unarmedWeapon.itemId)
                {
                    //Select weapon to switch
                    selectedWeapon =
                        player.playerInventoryManager.weaponInRightHandSlots[
                            player.playerInventoryManager.rightHandWeaponIndex];

                    //Assign weapon id to network to switch weapon on all connected client 
                    player.playerNetworkManager.currentRightHandWeaponId.Value = selectedWeapon.itemId;
                    //Animation
                    player.playerAnimatorManager.PlayTargetActionAnimation(swapRightWeaponAnimation, false, false, true,
                        true);

                    return;
                }
            }

            //If next weapon is unarmed (selectedWeapon == null) and next weapon index is not the last slot
            //==> Call this function again
            if (selectedWeapon == null && player.playerInventoryManager.rightHandWeaponIndex <
                player.playerInventoryManager.weaponInRightHandSlots.Length)
            {
                SwitchRightHandWeapon();
            }
        }
        
        public void SwitchLeftHandWeapon()
        {
            if (!player.IsOwner) return;
            
            //Weapon Swapping:
            //Check if there is another weapon beside main weapon, if do, rotate bewtween weapon
            //If not, swap to unarmed then skip the remain empty slot

            WeaponItem selectedWeapon = null;
            
            //Todo: Disable 2 handing if character are 2 handing
            
            //get next weapon index to switch into
            player.playerInventoryManager.leftHandWeaponIndex += 1;
            
            //If weapon index is out of bounds, reset to 0 then check for all weapon in slot
            if (player.playerInventoryManager.leftHandWeaponIndex < 0 ||
                player.playerInventoryManager.leftHandWeaponIndex >
                player.playerInventoryManager.weaponInLeftHandSlots.Length - 1)
            {
                //set to first weapon index if the index is outbound
                player.playerInventoryManager.leftHandWeaponIndex = 0;
                
                //Check if player is holding more than 1 weapon
                float weaponCount = 0;
                WeaponItem firstWeapon = null;
                int firstWeaponIndex = 0;
                
                for (int i = 0; i < player.playerInventoryManager.weaponInLeftHandSlots.Length; i++)
                {
                    if (player.playerInventoryManager.weaponInLeftHandSlots[i].itemId !=
                        WorldItemDatabase.Instance.unarmedWeapon.itemId)
                    {
                        //If this weapon in slot is not unarmed, weapon count + 1
                        weaponCount++;

                        if (firstWeapon == null)
                        {
                            //If first weapon variable is null => then this is first weapon
                            firstWeapon = player.playerInventoryManager.weaponInLeftHandSlots[i];
                            firstWeaponIndex = i;
                        }
                    }
                }
                
                //if there is only 1 weapon, next weapon switch will be unarmed and ser the index to -1 => weapon index is outbound
                if (weaponCount <= 1)
                {
                    //Set weapon index to outbound
                    player.playerInventoryManager.leftHandWeaponIndex = -1;
                    selectedWeapon = WorldItemDatabase.Instance.unarmedWeapon;
                    player.playerNetworkManager.currentLeftHandWeaponId.Value = selectedWeapon.itemId;
                }
                //If there is more than 1 weapon
                else
                {
                    //Get the first weapon
                    player.playerInventoryManager.leftHandWeaponIndex = firstWeaponIndex;
                    //Set network left hand weapon index to first weapon
                    player.playerNetworkManager.currentLeftHandWeaponId.Value = firstWeapon.itemId;
                    
                    //Animation
                    player.playerAnimatorManager.PlayTargetActionAnimation(swapLeftWeaponAnimation,false, false, true, true);

                }

                return;
            }
            
            //If index is not outnound check next weapon
            foreach (WeaponItem weapon in player.playerInventoryManager.weaponInRightHandSlots)
            {
                //check if next weapon slot is not unarmed weapon => get that weapon
                if (player.playerInventoryManager
                        .weaponInLeftHandSlots[player.playerInventoryManager.leftHandWeaponIndex].itemId !=
                    WorldItemDatabase.Instance.unarmedWeapon.itemId)
                {
                    //Select weapon to switch
                    selectedWeapon =
                        player.playerInventoryManager.weaponInLeftHandSlots[
                            player.playerInventoryManager.leftHandWeaponIndex];

                    //Assign weapon id to network to switch weapon on all connected client 
                    player.playerNetworkManager.currentLeftHandWeaponId.Value = selectedWeapon.itemId;
                    //Animation
                    player.playerAnimatorManager.PlayTargetActionAnimation(swapLeftWeaponAnimation, false, false, true,
                        true);

                    return;
                }
            }

            //If next weapon is unarmed (selectedWeapon == null) and next weapon index is not the last slot
            //==> Call this function again
            if (selectedWeapon == null && player.playerInventoryManager.leftHandWeaponIndex <
                player.playerInventoryManager.weaponInLeftHandSlots.Length)
            {
                SwitchLeftHandWeapon();
            }
        }
        
        public void loadWeaponOnBothHand()
        {
            loadWeaponOnRightHand();
            loadWeaponOnLeftHand();
        }
        
        public void loadWeaponOnRightHand()
        {
            if (player.playerInventoryManager.currentRightHandWeapon != null && rightHandSlot != null)
            {
                //Remove Old Weapon
                rightHandSlot.UnloadWeapon();
                
                //Get New weapon
                rightHandWeaponModel = Instantiate(player.playerInventoryManager.currentRightHandWeapon.weaponModel);
                rightHandSlot.LoadWeapon(rightHandWeaponModel);

                //Assign weapon damage to it collider
                rightWeaponManager = rightHandWeaponModel.GetComponent<WeaponManager>();
                rightWeaponManager.SetWeaponDamage(player, player.playerInventoryManager.currentRightHandWeapon);
            }
        }
        
        public void loadWeaponOnLeftHand()
        {
            if (player.playerInventoryManager.currentLeftHandWeapon != null && leftHandSlot != null)
            {
                //Remove Old Weapon
                leftHandSlot.UnloadWeapon();
                
                //Get New weapon
                leftHandWeaponModel = Instantiate(player.playerInventoryManager.currentLeftHandWeapon.weaponModel);
                leftHandSlot.LoadWeapon(leftHandWeaponModel);
                
                //Assign weapon damage to it collider
                leftWeaponManager = leftHandWeaponModel.GetComponent<WeaponManager>();
                leftWeaponManager.SetWeaponDamage(player,  player.playerInventoryManager.currentLeftHandWeapon);
            }
        }
        
        //DAMAGE COLLIDER
        public void OpenDamageCollider()
        {
            if (player.playerNetworkManager.isUsingRightHand.Value)
            {
                rightWeaponManager.weaponDamageCollider.EnableDamageCollider();
            }
            else if (player.playerNetworkManager.isUsingLeftHand.Value)
            {
                leftWeaponManager.weaponDamageCollider.EnableDamageCollider();
            }
            
            //Play Sound FX
        }
        
        public void CloseDamageCollider()
        {
            if (player.playerNetworkManager.isUsingRightHand.Value)
            {
                rightWeaponManager.weaponDamageCollider.DisableDamageCollider();
            }
            else if (player.playerNetworkManager.isUsingLeftHand.Value)
            {
                leftWeaponManager.weaponDamageCollider.DisableDamageCollider();
            }
        }
        
        public void FireRangeWeaponAttack()
        {
            if (player.playerNetworkManager.isUsingRightHand.Value)
            {
                DamageCollider weaponDamageCollider = rightWeaponManager.weaponDamageCollider;
                if (weaponDamageCollider is RangeWeaponDamageCollider rangeWeaponDamageCollider)
                {
                    rangeWeaponDamageCollider.FireRangeAttack();
                }
            }
            
            //Play Sound FX
        }
    }
}
