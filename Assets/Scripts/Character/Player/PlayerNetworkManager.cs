using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;

namespace AN
{
    public class PlayerNetworkManager : CharacterNetworkManager
    {
        private PlayerManager player;
        public NetworkVariable<FixedString64Bytes> characterName = new NetworkVariable<FixedString64Bytes>("Unknown", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        
        [Header("Equipments")]
        public NetworkVariable<int> currentWeaponBeingUsedId = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<int> currentRightHandWeaponId = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<int> currentLeftHandWeaponId = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<bool> isUsingRightHand = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<bool> isUsingLeftHand = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        protected override void Awake()
        {
            base.Awake();
            player = GetComponent<PlayerManager>();
        }

        public void SetCharacterActionHand(bool rightHandAction)
        {
            if (rightHandAction)
            {
                isUsingRightHand.Value = true;
                isUsingLeftHand.Value = false;
            }
            else
            {
                isUsingRightHand.Value = false;
                isUsingLeftHand.Value = true;
            }
        }
        
        public void SetNewMaxHealthValue(int oldValue, int newValue)
        {
            maxHealth.Value = player.playerStatManager.CalculateHealthBaseOnStat(newValue);
            PlayerUIManager.instance.playerUIHudManager.SetMaxHealthValue(maxHealth.Value);
            currentHealth.Value = maxHealth.Value;
        }
        public void SetNewMaxEnergyValue(int oldValue, int newValue)
        {
            maxEnergy.Value = player.playerStatManager.CalculateEnergyBaseOnStat(newValue);
            PlayerUIManager.instance.playerUIHudManager.SetMaxEnergyValue(maxEnergy.Value);
            currentEnergy.Value = maxEnergy.Value;
        }
        public void SetNewMaxStaminaValue(int oldValue, int newValue)
        {
            maxStamina.Value = player.playerStatManager.CalculateStaminaBaseOnStat(newValue);
            PlayerUIManager.instance.playerUIHudManager.SetMaxStamninaValue(maxStamina.Value);
            currentStamina.Value = maxStamina.Value;
        }
        
        public void OnCurrentRightHandWeaponIDChange(int oldID, int newID)
        {
            WeaponItem newWeapon = Instantiate(WorldItemDatabase.Instance.GetWeaponByID(newID));
            player.playerInventoryManager.currentRightHandWeapon = newWeapon;
            player.playerEquipmentManager.loadWeaponOnRightHand();

            if (player.IsOwner)
            {
                PlayerUIManager.instance.playerUIHudManager.SetQuickSlotIcon(new []{newID});
            }
        }
        
        public void OnCurrentUsingWeaponIDChange(int oldID, int newID)
        {
            WeaponItem newWeapon = Instantiate(WorldItemDatabase.Instance.GetWeaponByID(newID));
            player.playerCombatManager.currentWeaponBeingUsed = newWeapon;
        }

        //Weapon Action
        [ServerRpc]
        public void NotifyServerOfWeaponActionServerRPC(ulong clientId, int actionId, int weaponId)
        {
            if (IsServer)
            {
                NotifyClientOfWeaponActionClientRPC(clientId, actionId, weaponId);
            }
        }
        
        [ClientRpc]
        private void NotifyClientOfWeaponActionClientRPC(ulong clientId, int actionId, int weaponId)
        {
            if (clientId != NetworkManager.Singleton.LocalClientId)
            {
                PerformWeaponBaseAction(actionId, weaponId);
            }
        }

        private void PerformWeaponBaseAction(int actionId, int weaponId)
        {
            WeaponItemAction weaponAction = WorldActionManager.Instance.GetWeaponItemAction(actionId);

            if (weaponAction != null)
            {
                weaponAction.AttempToPerformAction(player, WorldItemDatabase.Instance.GetWeaponByID(weaponId));
            }
            else
            {
                Debug.LogError("Weapon not found, can not perform action");
            }
        }
    }
}