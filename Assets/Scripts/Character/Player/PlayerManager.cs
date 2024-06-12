using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Unity.Netcode;
using Unity.VisualScripting;

namespace AN
{
    public class PlayerManager : CharacterManager
    {
        [Header("Player's Mesh Renderers Object")] 
        public GameObject playerMeshRenderer;
        
        [Header("Debug Menu")] 
        [SerializeField] private bool respawnCharacter = false;
        [FormerlySerializedAs("setWeaponRightSlot")] [SerializeField] private bool setQuickSlot = false;
        [SerializeField] private bool setWeaponLeftSlot = false;
        
        [Header("Camera Mode")] 
        [HideInInspector] public bool isThirdPersonCamera = true;
    
        [HideInInspector] public PlayerAnimatorManager playerAnimatorManager;
        [HideInInspector] public PlayerLocomotionManager playerLocomotionManager;
        [HideInInspector] public PlayerNetworkManager playerNetworkManager;
        [HideInInspector] public PlayerStatManager playerStatManager;
        [HideInInspector] public PlayerInventoryManager playerInventoryManager;
        [HideInInspector] public PlayerEquipmentManager playerEquipmentManager;
        [HideInInspector] public PlayerCombatManager playerCombatManager;
        protected override void Awake()
        {
            base.Awake();

            playerLocomotionManager = GetComponent<PlayerLocomotionManager>();
            playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
            playerNetworkManager = GetComponent<PlayerNetworkManager>();
            playerStatManager = GetComponent<PlayerStatManager>();
            playerInventoryManager = GetComponent<PlayerInventoryManager>();
            playerEquipmentManager = GetComponent<PlayerEquipmentManager>();
            playerCombatManager = GetComponent<PlayerCombatManager>();
        }

        protected override void Update()
        {
            base.Update();

            //if not the owner of this game object, return 
            if (!IsOwner)
            {
                return;
            }

            //Handle movement
            playerLocomotionManager.HandleAllMovement();
            
            //Stamina Regen
            playerStatManager.RegenerateStamina();
            DebugMenu();
            
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallBack;
            
            if (IsOwner)
            {
                
                //If is owner of this character and not the host => joining a server ===> reload character
                if (!IsServer)
                {
                    LoadGameDataFromCurrentCharacterData(ref WorldSaveGameManager.Instance.currentCharacterData);
                }
                
                PlayerCamera.Instance.player = this;
                PlayerInputManager.Instance.player = this;
                WorldSaveGameManager.Instance.player = this;
                
                // sử dụng += để đăng ký phương thức SetNewStamninaValue với OnValueChanged của NetworkVariable,
                // hai giá trị cũ và mới sẽ luôn được truyền vào phương thức đó khi sự kiện xảy ra.
                // do OnValueChanged là một sự kiện được thiết kế để thông báo về sự thay đổi của giá trị,
                // nó luôn luôn truyền vào hai giá trị: giá trị cũ và giá trị mới.
                // không cần phải chỉ định các giá trị này khi đăng ký phương thức với sự kiện, Unity sẽ tự động làm điều đó.
                
                //Update UI stat bar when value change
                playerNetworkManager.currentHealth.OnValueChanged += PlayerUIManager.instance.playerUIHudManager.SetNewHealthValue;
                playerNetworkManager.currentEnergy.OnValueChanged += PlayerUIManager.instance.playerUIHudManager.SetNewEnergyValue;
                playerNetworkManager.currentStamina.OnValueChanged += PlayerUIManager.instance.playerUIHudManager.SetNewStamninaValue;
                
                playerNetworkManager.currentStamina.OnValueChanged += playerStatManager.ResetStaminaRegenerationTimer;
                
                //Update Max value of resource when stat change
                playerNetworkManager.vitality.OnValueChanged += playerNetworkManager.SetNewMaxHealthValue;
                playerNetworkManager.intellect.OnValueChanged += playerNetworkManager.SetNewMaxEnergyValue;
                playerNetworkManager.endurance.OnValueChanged += playerNetworkManager.SetNewMaxStaminaValue;
                
                //Toggle Crosshair
                playerNetworkManager.isAiming.OnValueChanged += PlayerUIManager.instance.playerCrosshairManager.ToggleCrosshair;
            }
            
            playerNetworkManager.currentHealth.OnValueChanged += playerNetworkManager.CheckHP;
            
            //Lock On
            playerNetworkManager.isLockOn.OnValueChanged += playerNetworkManager.OnIsLockOnValueChange;
            playerNetworkManager.currentTargetNetworkObjectId.OnValueChanged += playerNetworkManager.OnLockOnTargetIdChange;
            
            //Load Weapon in weapon id change
            playerNetworkManager.currentRightHandWeaponId.OnValueChanged += playerNetworkManager.OnCurrentRightHandWeaponIDChange;
            playerNetworkManager.currentWeaponBeingUsedId.OnValueChanged += playerNetworkManager.OnCurrentUsingWeaponIDChange;
            
            //Flags
            playerNetworkManager.isChargingAttack.OnValueChanged += playerNetworkManager.OnIsChargingAttackChange;
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnectedCallBack;
            
            if (IsOwner)
            {
                //Update UI stat bar when value change
                playerNetworkManager.currentHealth.OnValueChanged -= PlayerUIManager.instance.playerUIHudManager.SetNewHealthValue;
                playerNetworkManager.currentEnergy.OnValueChanged -= PlayerUIManager.instance.playerUIHudManager.SetNewEnergyValue;
                playerNetworkManager.currentStamina.OnValueChanged -= PlayerUIManager.instance.playerUIHudManager.SetNewStamninaValue;
                
                playerNetworkManager.currentStamina.OnValueChanged -= playerStatManager.ResetStaminaRegenerationTimer;
                
                //Update Max value of resource when stat change
                playerNetworkManager.vitality.OnValueChanged -= playerNetworkManager.SetNewMaxHealthValue;
                playerNetworkManager.intellect.OnValueChanged -= playerNetworkManager.SetNewMaxEnergyValue;
                playerNetworkManager.endurance.OnValueChanged -= playerNetworkManager.SetNewMaxStaminaValue;
                
                //Toggle Crosshair
                playerNetworkManager.isAiming.OnValueChanged -= PlayerUIManager.instance.playerCrosshairManager.ToggleCrosshair;
            }
            
            playerNetworkManager.currentHealth.OnValueChanged -= playerNetworkManager.CheckHP;
            
            //Lock On
            playerNetworkManager.isLockOn.OnValueChanged -= playerNetworkManager.OnIsLockOnValueChange;
            playerNetworkManager.currentTargetNetworkObjectId.OnValueChanged -= playerNetworkManager.OnLockOnTargetIdChange;
            
            //Load Weapon in weapon id change
            playerNetworkManager.currentRightHandWeaponId.OnValueChanged -= playerNetworkManager.OnCurrentRightHandWeaponIDChange;
            playerNetworkManager.currentWeaponBeingUsedId.OnValueChanged -= playerNetworkManager.OnCurrentUsingWeaponIDChange;
            
            //Flags
            playerNetworkManager.isChargingAttack.OnValueChanged -= playerNetworkManager.OnIsChargingAttackChange;
        }
        
        private void OnClientConnectedCallBack(ulong clientId)
        {
            //Keep a list of player active in game
            WorldGameSessionManager.Instance.AddPlayerToActivePlayerList(this);
            
            //Host and Server don't need to load players to sync them
            //Only sync player gear when join the game late
            if (!IsServer && IsOwner)
            {
                
                foreach (var player in WorldGameSessionManager.Instance.players)
                {
                    if (player != this)
                    {
                        player.LoadOtherPlayerCharacterWhenJoiningServer();
                    }
                }
            }
        }
        
        public override IEnumerator ProcessDeathEvent(bool manuallySelectDeathAnimation = false)
        {
            if (IsOwner)
            {
                PlayerUIManager.instance.playerUIPopUpManager.OpenDeathPopUp();
            }
            
            return base.ProcessDeathEvent(manuallySelectDeathAnimation);

        }

        protected override void LateUpdate()
        {
            if (!IsOwner)
            {
                return;
            }
            base.LateUpdate();
            
            PlayerCamera.Instance.HandleAllCameraActions();
        }

        //reference to current character data
        //that mean anything happen to character data in this 'SaveGameDataToCurrentCharacterData' or 'LoadGameDataFromCurrentCharacterData'
        //Will change the value of the currentCharacterData
        //but in this case, use reference to make sure that the currentCharacterData
        //is not any currentCharacterData but the current have been pass down this function
        public void SaveGameDataToCurrentCharacterData(ref CharacterSaveData currentCharacterData)
        {
            currentCharacterData.characterName = playerNetworkManager.characterName.Value.ToString();
            currentCharacterData.xPosition = transform.position.x;
            currentCharacterData.yPosition = transform.position.y;
            currentCharacterData.zPosition = transform.position.z;

            currentCharacterData.vitality = playerNetworkManager.vitality.Value;
            currentCharacterData.intellect = playerNetworkManager.intellect.Value;
            currentCharacterData.endurance = playerNetworkManager.endurance.Value;
            
            currentCharacterData.currentHealth = playerNetworkManager.currentHealth.Value;
            currentCharacterData.currentEnergy = playerNetworkManager.currentEnergy.Value;
            currentCharacterData.currentStamina = playerNetworkManager.currentStamina.Value;
        }

        public void LoadGameDataFromCurrentCharacterData(ref CharacterSaveData currentCharacterData)
        {
            currentCharacterData.characterName = playerNetworkManager.characterName.Value.ToString();
            Vector3 characterLoadedPosition = new Vector3(currentCharacterData.xPosition,currentCharacterData.yPosition,currentCharacterData.zPosition);
            transform.position = characterLoadedPosition;
            
            //Sync stat to network manager
            playerNetworkManager.vitality.Value = currentCharacterData.vitality ;
            playerNetworkManager.intellect.Value = currentCharacterData.intellect;
            playerNetworkManager.endurance.Value = currentCharacterData.endurance;
            
            //Set character healt/energy/stamina base on stat
            //Health
            int healthBaseOnStat = playerStatManager.CalculateHealthBaseOnStat(playerNetworkManager.vitality.Value);
            playerNetworkManager.maxHealth.Value = healthBaseOnStat;
            playerNetworkManager.currentHealth.Value = currentCharacterData.currentHealth;
            PlayerUIManager.instance.playerUIHudManager.SetMaxHealthValue(playerNetworkManager.maxHealth.Value);
            //Energy
            int energyBaseOnStat = playerStatManager.CalculateEnergyBaseOnStat(playerNetworkManager.intellect.Value);
            playerNetworkManager.maxEnergy.Value = energyBaseOnStat;
            playerNetworkManager.currentEnergy.Value = currentCharacterData.currentEnergy;
            PlayerUIManager.instance.playerUIHudManager.SetMaxEnergyValue(playerNetworkManager.maxEnergy.Value);
            //Stamina
            int staminaBaseOnStat = playerStatManager.CalculateStaminaBaseOnStat(playerNetworkManager.endurance.Value);
            playerNetworkManager.maxStamina.Value = staminaBaseOnStat;
            playerNetworkManager.currentStamina.Value = currentCharacterData.currentStamina;
            PlayerUIManager.instance.playerUIHudManager.SetMaxStamninaValue(playerNetworkManager.maxStamina.Value);
        }

        //Load All other character when joining the server
        public void LoadOtherPlayerCharacterWhenJoiningServer()
        {
            //Sync Weapon
            playerNetworkManager.OnCurrentRightHandWeaponIDChange(0,playerNetworkManager.currentRightHandWeaponId.Value);
            
            //Sync Lock On Target
            if (playerNetworkManager.isLockOn.Value)
            {
                playerNetworkManager.OnLockOnTargetIdChange(0,playerNetworkManager.currentTargetNetworkObjectId.Value);
            }
        }
        
        public override void ReviveCharacter()
        {
            base.ReviveCharacter();
            
            if (IsOwner)
            {
                isDead.Value = false;
                playerNetworkManager.currentHealth.Value = playerNetworkManager.maxHealth.Value;
                playerNetworkManager.currentEnergy.Value = playerNetworkManager.maxEnergy.Value;
                playerNetworkManager.currentStamina.Value = playerNetworkManager.maxStamina.Value;
                
                //Play rebirth animation
                playerAnimatorManager.PlayTargetActionAnimation("Revive",false);
            }
        }

        // Will need to be delete later
        private void DebugMenu()
        {
            if (respawnCharacter)
            {
                respawnCharacter = false;
                ReviveCharacter();
            }

            if (setQuickSlot)
            {
                setQuickSlot = false;
                PlayerUIManager.instance.playerUIHudManager.SetQuickSlotIcon(playerInventoryManager.weaponInRightHandSlots.Select(e => e.itemId).ToArray(),playerInventoryManager.rightHandWeaponIndex);
            }
        }
    }
}
