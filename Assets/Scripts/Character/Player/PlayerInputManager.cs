using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.Serialization;

namespace AN
{
    public class PlayerInputManager : MonoBehaviour
    {
        public static PlayerInputManager Instance;

        public PlayerManager player;
        
        PlayerControls playerControls;
        
        [Header("MOVEMENT INPUT")]
        [SerializeField] private Vector2 movementInput;
        public float verticalInput;
        public float horizontalInput;
        public float moveAmount;
        
        [Header("CAMERA MOVEMENT INPUT")]
        [SerializeField] private Vector2 cameraInput;
        public float cameraVerticalInput;
        public float cameraHorizontalInput;
        
        [FormerlySerializedAs("lockOnInput")]
        [Header("LOCK ON INPUT")]
        [SerializeField] private bool lock_On_Input;
        [SerializeField] private bool lock_On_Left_Input;
        [SerializeField] private bool lockOnRightInput;
        private Coroutine lockOnCoroutine;
        
        [Header("ACTION INPUT")]
        [SerializeField] private bool rollInput;
        [SerializeField] private bool jumpInput;
        [SerializeField] private bool sprintInput;
        [SerializeField] private bool switchCameraMode;
        [SerializeField] private bool aimInput;
        
        [FormerlySerializedAs("leftClickInput")]
        [Header("COMBAT INPUT")]
        [SerializeField] private bool left_Click_Input;
        [SerializeField] private bool right_Click_Input;
        [SerializeField] private bool hold_Right_Click_Input;
        
        [Header("QUICK SLOT INPUT")]
        [SerializeField] private bool switchRightWeaponInput;
        [SerializeField] private bool switchLeftWeaponInput;
        
        private void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);

            // subcribe OnSceneChange function to activeSceneChanged so => When scene change run OnSceneChange function
            SceneManager.activeSceneChanged += OnSceneChange;

            Instance.enabled = false;

            if (playerControls != null)
            {
                playerControls.Disable();
            }
        }

        private void OnSceneChange(Scene oldScene, Scene newScene)
        {
            if (newScene.buildIndex == WorldSaveGameManager.Instance.GetWorldSceneIndex())
            {
                Instance.enabled = true;
                Cursor.lockState = CursorLockMode.Locked;
                if (playerControls != null)
                {
                    playerControls.Enable();
                }
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Instance.enabled = false;
                if (playerControls != null)
                {
                    playerControls.Disable();
                }
            }
            
            
        }
        
        //OnEnable() được gọi trong các trường hợp sau:

        //Khi đối tượng được kích hoạt(enabled): Khi bật(hoặc kích hoạt) một đối tượng trong Unity(vd: bật một GameObject),
        //            hàm OnEnable() sẽ được thực thi.
        //=> đối tượng chuyển từ trạng thái không hoạt động sang trạng thái hoạt động.

        //Khi Component được kích hoạt (enabled): Nếu kích hoạt một Component (vd: script) trên một đối tượng,
        //            hàm OnEnable() của Component đó sẽ được gọi.
        //=> bật một Component sau khi đối tượng đã được khởi tạo.

        //OnEnable() thường được sử dụng để thực hiện các tác vụ liên quan đến việc khởi tạo và kích hoạt đối tượng trong Unity.
        //===>Thường được gọi sau hàm Awake() và trước hàm Start()

        private void OnEnable()
        {
            if(playerControls == null)
            {
                playerControls = new PlayerControls();

                // += operator asign function to the event
                
                //Movement
                playerControls.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
                
                //Camera
                playerControls.PlayerCamera.Movement.performed += i => cameraInput = i.ReadValue<Vector2>();
                playerControls.PlayerCamera.SwitchCameraMode.performed += i => switchCameraMode = true;
                
                //Aim & Lock On
                playerControls.PlayerAction.Aim.performed += i => aimInput = true;
                playerControls.PlayerAction.Aim.canceled += i => aimInput = false;
                playerControls.PlayerAction.LockOn.performed += i => lock_On_Input = true;
                playerControls.PlayerAction.SeekLeftLockOnTarget.performed += i => lock_On_Left_Input = true;
                playerControls.PlayerAction.SeekRightLockOnTarget.performed += i => lockOnRightInput = true;
                
                //Movement Actions
                playerControls.PlayerAction.Roll.performed += i => rollInput = true;
                playerControls.PlayerAction.Sprint.performed += i => sprintInput = true;
                playerControls.PlayerAction.Sprint.canceled += i => sprintInput = false;
                playerControls.PlayerAction.Jump.performed += i => jumpInput = true;
                
                //Combat Action
                playerControls.PlayerAction.LeftClick.performed += i => left_Click_Input = true;
                playerControls.PlayerAction.RightClick.performed += i => right_Click_Input = true;
                playerControls.PlayerAction.HoldRightClick.performed += i => hold_Right_Click_Input = true;
                playerControls.PlayerAction.HoldRightClick.canceled += i => hold_Right_Click_Input = false;
                
                //Switch Weapon
                playerControls.PlayerAction.SwitchRightWeapon.performed += i => switchRightWeaponInput = true;
                playerControls.PlayerAction.SwitchLeftWeapon.performed += i => switchLeftWeaponInput = true;
            }

            playerControls.Enable();
        }

        private void OnDestroy()
        {
            // if destroy this object, unsubcribe function from activeSceneChanged
            SceneManager.activeSceneChanged -= OnSceneChange;
        }

        private void Update()
        {
            HandleAllInput();
        }
        
        private void OnApplicationFocus(bool focus)
        {
            //If not active on application, disable control
            if (enabled)
            {
                if (focus)
                {
                    playerControls.Enable();
                }
                else
                {
                    playerControls.Disable();
                }
            }
        }

        private void HandleAllInput()
        {
            HandlePlayerMovementInput();
            HandleCameraMovementInput();
            HandlePlayerActionInput();
        }
        
        private void HandlePlayerActionInput()
        {
            HandleRollMovementInput();
            HandleSprintMovementInput();
            HandleJumpMovementInput();
            
            HandleAimInput();
            
            HandleLockOnInput();
            HandleLockOnTargetSwitchInput();
            
            HandleAttackInput();
            HandleHeavyAttackInput();
            HandleChargeAttackInput();
            
            HandleSwitchWeaponInput();
        }
        
        
        
        private void HandlePlayerMovementInput()
        {
            verticalInput = movementInput.y;
            horizontalInput = movementInput.x;

            //Clamp01: if value > 1 return 1 , if value < 0 return 0, else return value 
            moveAmount = Mathf.Clamp01(Mathf.Abs(verticalInput) + Mathf.Abs(horizontalInput));


            
            // Move speed will only be 0, 0.5 or 1
            if(moveAmount > 0 && moveAmount <= 0.5)
            {
                moveAmount = 0.5f;
            }
            else if (moveAmount > 0.5 && moveAmount <= 1)
            {
                moveAmount = 1;
            }

            if (moveAmount > 0 && player.playerNetworkManager.isSprinting.Value)
            {
                moveAmount = 2;
            }
            
            if ((player.playerNetworkManager.isAiming.Value || player.playerNetworkManager.isLockOn.Value) && !player.playerNetworkManager.isSprinting.Value)
            {
                player.playerAnimatorManager.UpdateAnimatorMovementParameters(horizontalInput ,verticalInput);
            }
            else
            {
                //not lock-on
                player.playerAnimatorManager.UpdateAnimatorMovementParameters(0 ,moveAmount);
            }
            
            
            PlayerUIManager.instance.playerCrosshairManager.value = moveAmount;
        }

        private void HandleCameraMovementInput()
        {
            cameraVerticalInput = cameraInput.y;
            cameraHorizontalInput = cameraInput.x;

            if (switchCameraMode)
            {
                switchCameraMode = false;
                PlayerCamera.Instance.SwitchCameraMode();
            }
        }

        private void HandleRollMovementInput()
        {
            if (rollInput)
            {
                rollInput = false;
                
                //TODO: return (do nothing) if menu or UI window is open
                
                //perform a roll
                player.playerLocomotionManager.AttemptToPerformDodge();
            }   
        }
        
        private void HandleSprintMovementInput()
        {
            if (sprintInput)
            {
                player.playerLocomotionManager.HandleSprinting();
            }
            else
            {
                player.playerNetworkManager.isSprinting.Value = false;
            }
        }
        
        private void HandleJumpMovementInput()
        {
            if (jumpInput)
            {
                jumpInput = false;
                
                //TODO: return (do nothing) if menu or UI window is open
                
                //perform a jump
                player.playerLocomotionManager.HandleJumping();
            }
        }
        
        private void HandleAttackInput()
        {
            if (left_Click_Input)
            {
                left_Click_Input = false;
                //TODO: return (do nothing) if menu or UI window is open
                
                player.playerNetworkManager.SetCharacterActionHand(true);
                
                //TODO: use 2 hand action when character equip 2 hand weapon
                
                player.playerCombatManager.PerformWeaponBaseAction(player.playerInventoryManager.currentRightHandWeapon.oh_Attack_Action,player.playerInventoryManager.currentRightHandWeapon);
            }
        }
        
        private void HandleHeavyAttackInput()
        {
            if (right_Click_Input)
            {
                right_Click_Input = false;
                //TODO: return (do nothing) if menu or UI window is open
                
                player.playerNetworkManager.SetCharacterActionHand(true);
                
                //TODO: use 2 hand action when character equip 2 hand weapon
                
                player.playerCombatManager.PerformWeaponBaseAction(player.playerInventoryManager.currentRightHandWeapon.oh_Heavy_Attack_Action,player.playerInventoryManager.currentRightHandWeapon);
            }
        }
        
        private void HandleChargeAttackInput()
        {
            //Only check for a charge when in a action that required it
            if (player.isPerformingAction)
            {
                if (player.playerNetworkManager.isUsingRightHand.Value)
                {
                    player.playerNetworkManager.isChargingAttack.Value = hold_Right_Click_Input;
                }
            }
        }
        
        private void HandleAimInput()
        {
            if (aimInput)
            {
                //Disable lock on when aiming
                player.playerNetworkManager.isLockOn.Value = !aimInput;
                
                if (!player.playerInventoryManager.currentRightHandWeapon.canAim)
                {
                    if (!player.playerInventoryManager.currentLeftHandWeapon.canAim)
                    {
                        player.playerNetworkManager.isAiming.Value = false;
                        return;
                    }
                }
            }
            
            player.playerNetworkManager.isAiming.Value = aimInput;
        }

        //---Lock On---//
        private void HandleLockOnInput()
        {
            //Check if target Lock On is dead ? Unlock/Change target : Unlock 
            if (player.playerNetworkManager.isLockOn.Value)
            {
                if (player.playerCombatManager.currentTarget == null)
                    return;
                
                if (player.playerCombatManager.currentTarget.isDead.Value)
                {
                    player.playerNetworkManager.isLockOn.Value = false;
                }
                
                //Find new target
                
                //Make sure that there will only one lockOnCoroutine can be run on this script
                if(lockOnCoroutine != null)
                    StopCoroutine(lockOnCoroutine);

                lockOnCoroutine = StartCoroutine(PlayerCamera.Instance.WaitThenFindNewTarget());
            }
            
            //Check if already Lock On ? Unlock : Lock On 
            if (lock_On_Input && player.playerNetworkManager.isLockOn.Value)
            {
                lock_On_Input = false;
                player.playerNetworkManager.isLockOn.Value = false;
                
                //Disable Lock On
                PlayerCamera.Instance.ClearLockOnTarget();
                
                return;
            }
            
            if (lock_On_Input && !player.playerNetworkManager.isLockOn.Value)
            {
                lock_On_Input = false;
                
                //If Using Range Weapon => Return
                if (player.playerNetworkManager.isAiming.Value)
                {
                    return;
                }

                //Lock On
                PlayerCamera.Instance.HandleLocatingLockOnTargets();

                if (PlayerCamera.Instance.nearestLockOnTarget != null)
                {
                    player.playerCombatManager.SetTarget(PlayerCamera.Instance.nearestLockOnTarget);
                    player.playerNetworkManager.isLockOn.Value = true;
                }
            }
        }
        
        private void HandleLockOnTargetSwitchInput()
        {
            if (lock_On_Left_Input)
            {
                TriggerLeftRightLockOnTarget(ref lock_On_Left_Input, ref PlayerCamera.Instance.leftLockOnTarget);
            }
            
            if (lockOnRightInput)
            {
                TriggerLeftRightLockOnTarget(ref lockOnRightInput, ref PlayerCamera.Instance.rightLockOnTarget);
            }
        }

        public void TriggerLeftRightLockOnTarget(ref bool lockOnDirectionInput, ref CharacterManager lockOnTargetOfDirection)
        {
            lockOnDirectionInput = false;

            if (player.playerNetworkManager.isLockOn.Value)
            {
                PlayerCamera.Instance.HandleLocatingLockOnTargets();

                if (lockOnTargetOfDirection != null)
                {
                    player.playerCombatManager.SetTarget(lockOnTargetOfDirection);
                }
            }
        }
        
        //---End Lock On---//
        private void HandleSwitchWeaponInput()
        {
            if (switchRightWeaponInput)
            {
                player.playerEquipmentManager.SwitchRightHandWeapon();

                //!impotant: this is for testing only, need to be delete after found a way to sync weapon from inventory
                PlayerUIManager.instance.playerUIHudManager.SetQuickSlotIcon(player.playerInventoryManager.weaponInRightHandSlots.Select(e => e.itemId).ToArray(), player.playerInventoryManager.rightHandWeaponIndex);
                
                PlayerUIManager.instance.playerUIHudManager.StartCoolDownWeaponSwitch(player.playerNetworkManager.weaponSwitchCooldownTime.Value);
            }
            switchRightWeaponInput = false;
        }
    }
}
