using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AN
{
    public class PlayerLocomotionManager : CharacterLocomotionManager
    {
        PlayerManager player;

        [HideInInspector]
        public float verticalMovement;
        public float horizontalMovement;
        public float moveAmount;

        [Header("Movement Setting")]
        [SerializeField] float walkingSpeed = 1;
        [SerializeField] float runningSpeed = 5;
        [SerializeField] float sprintingSpeed = 8;
        [SerializeField] private int sprintStaminaCost = 0;
        
        [SerializeField] float rotationSpeed = 15;
        private Vector3 moveDirection;
        private Vector3 targetRotationDirection;

        [Header("Jump")]
        private Vector3 jumpDirection;
        [SerializeField] int jumpStaminaCost = 3;
        [SerializeField] int jumpHeight = 2;
        [SerializeField] int jumpForwardSpeed = 8;
        [SerializeField] int freeFallSpeed = 2;
        
        [Header("Roll")] 
        private Vector3 rollDirection;
        [SerializeField] int rollStaminaCost = 5;
        
        protected override void Awake()    
        {
            base.Awake();
            player = GetComponent<PlayerManager>();
        }

        protected override void Update()
        {
            base.Update();
            
            GetMovementValues();
            
            if (player.IsOwner)
            {
                player.characterNetworkManager.animaterVerticalNetworkParameter.Value = verticalMovement;
                player.characterNetworkManager.animaterHorizontalNetworkParameter.Value = horizontalMovement;
                player.characterNetworkManager.networkMoveAmount.Value = moveAmount;
            }
            else
            {
                verticalMovement = player.characterNetworkManager.animaterVerticalNetworkParameter.Value;
                horizontalMovement = player.characterNetworkManager.animaterHorizontalNetworkParameter.Value;
                moveAmount = player.characterNetworkManager.networkMoveAmount.Value;

                if ((player.playerNetworkManager.isAiming.Value || player.playerNetworkManager.isLockOn.Value) && !player.playerNetworkManager.isSprinting.Value)
                {
                    player.playerAnimatorManager.UpdateAnimatorMovementParameters(horizontalMovement ,verticalMovement);
                }
                else
                {
                    //not lock-on
                    player.playerAnimatorManager.UpdateAnimatorMovementParameters(0 ,moveAmount);
                }
                
            }
        }

        public void HandleAllMovement()
        {
            //Ground
            HandleGroundedMovement();
            HandleRotation();
            //Aerial
            HandleJumpMovement();
            HandleFreeFallMovement();
        }

        private void GetMovementValues()
        {
            verticalMovement = PlayerInputManager.Instance.verticalInput;
            horizontalMovement = PlayerInputManager.Instance.horizontalInput;
            moveAmount = PlayerInputManager.Instance.moveAmount;
        }
        
        public void HandleGroundedMovement()
        {
            float moveSpeed = 0;
            if (!player.canMove)
                return;
            
            // movement diraction base on camera's facing perspective and movement input
            moveDirection = PlayerCamera.Instance.transform.forward * verticalMovement;
            moveDirection += PlayerCamera.Instance.transform.right * horizontalMovement;

            //Vector holds 2 pieces of information - a point in space and a magnitude.
            //The magnitude is the length of the line formed between(0, 0, 0) and the point in space.
            //If you "normalize" a vector(also known as the "unit vector"),
            //the result is a line that starts a(0, 0, 0) and "points" to your original point in space.
            moveDirection.Normalize();
            moveDirection.y = 0;

            if (player.playerNetworkManager.isSprinting.Value)
            {
                moveSpeed = sprintingSpeed;
            }
            else
            {
                if(moveAmount > 0.5f)
                {
                    // Running
                    moveSpeed = runningSpeed;
                }
                else if (moveAmount <= 0.5f)
                {
                    //Walking
                    moveSpeed = walkingSpeed;
                }
            }
            
            player.characterController.Move(moveDirection * (moveSpeed * Time.deltaTime));
        }

        public void HandleRotation()
        {
            if (player.isDead.Value)
                return;
            
            if (!player.canRotate)
                return;
            
            if (player.playerNetworkManager.isLockOn.Value)
            {
                if (player.playerNetworkManager.isSprinting.Value || isRolling)
                {
                    targetRotationDirection = Vector3.zero;
                    targetRotationDirection = PlayerCamera.Instance.cameraObject.transform.forward * verticalMovement;
                    targetRotationDirection += PlayerCamera.Instance.cameraObject.transform.right * horizontalMovement;
                    targetRotationDirection.Normalize();
                    targetRotationDirection.y = 0;

                    if (targetRotationDirection == Vector3.zero)
                    {
                        targetRotationDirection = transform.forward;
                    }
                    
                    Quaternion newRotation = Quaternion.LookRotation(targetRotationDirection);
                    Quaternion targetRotation = Quaternion.Slerp(transform.rotation, newRotation, rotationSpeed * Time.deltaTime);
                    transform.rotation = targetRotation;
                }
                else
                {
                    if (player.playerCombatManager.currentTarget == null)
                        return;
                    
                    targetRotationDirection = player.playerCombatManager.currentTarget.transform.position - transform.position;
                    targetRotationDirection.y = 0;
                    targetRotationDirection.Normalize();
                    
                    Quaternion newRotation = Quaternion.LookRotation(targetRotationDirection);
                    Quaternion targetRotation = Quaternion.Slerp(transform.rotation, newRotation, rotationSpeed * Time.deltaTime);
                    transform.rotation = targetRotation;
                }
            }
            else
            {
                targetRotationDirection = Vector3.zero;

                if (player.playerNetworkManager.isAiming.Value)
                {
                    targetRotationDirection = PlayerCamera.Instance.cameraObject.transform.forward;
                    targetRotationDirection += PlayerCamera.Instance.cameraObject.transform.right;
                }
                else
                {
                    targetRotationDirection = PlayerCamera.Instance.cameraObject.transform.forward * verticalMovement;
                    targetRotationDirection += PlayerCamera.Instance.cameraObject.transform.right * horizontalMovement;
                    targetRotationDirection.y = 0;
                }
 
                targetRotationDirection.Normalize();
                
                if (targetRotationDirection == Vector3.zero)
                {
                    targetRotationDirection = transform.forward;
                }

                Quaternion newRotation = Quaternion.LookRotation(targetRotationDirection);
                Quaternion targetRotation =
                    Quaternion.Slerp(transform.rotation, newRotation, rotationSpeed * Time.deltaTime);

                transform.rotation = targetRotation;
            }
        }

        public void AttemptToPerformDodge()
        {
            if(player.isPerformingAction)
                return;
            
            //Out of stamina = do nothing
            if (player.playerNetworkManager.currentStamina.Value <= 0)
                return;
            
            if (moveAmount > 0)
            {
                rollDirection = PlayerCamera.Instance.transform.forward * verticalMovement;
                rollDirection += PlayerCamera.Instance.transform.right * horizontalMovement;
                rollDirection.Normalize();
                rollDirection.y = 0;
                
                Quaternion playerRotation = Quaternion.LookRotation(rollDirection);
                player.transform.rotation = playerRotation;
                
                //Roll animation
                player.playerAnimatorManager.PlayTargetActionAnimation("Roll_Forward",true,true);
                isRolling = true;
            }
            
            //if not moving => perform a backstep
            else
            {
                //Backstep animation
                player.playerAnimatorManager.PlayTargetActionAnimation("Roll_Backward",true,true);
            }
            
            player.playerNetworkManager.currentStamina.Value -= rollStaminaCost;
            
        }

        public void HandleSprinting()
        {
            if (player.isPerformingAction)
            {
                player.playerNetworkManager.isSprinting.Value = false;
                return;
            }
            
            if (player.playerNetworkManager.isAiming.Value)
            {
                player.playerNetworkManager.isSprinting.Value = false;
                return;
            }
            
            //Out of stamina = do nothing
            if (player.playerNetworkManager.currentStamina.Value <= 0)
            {
                player.playerNetworkManager.isSprinting.Value = false;
                return;
            }
            
            //Not moving = false
            //Moving = true
            if (moveAmount >= 0.5)
            {
                player.playerNetworkManager.isSprinting.Value = true;
            }
            else
            {
                player.playerNetworkManager.isSprinting.Value = false; 
            }

            if (needStaminaToSprint && player.playerNetworkManager.isSprinting.Value)
            {
                player.playerNetworkManager.currentStamina.Value -= sprintStaminaCost * Time.deltaTime;
            }
        }

        public void HandleJumping()
        {
            if(player.isPerformingAction)
                return;
            
            //Out of stamina = do nothing
            if (player.playerNetworkManager.currentStamina.Value <= 0)
                return;

            if (player.playerNetworkManager.isJumping.Value)
                return;
            
            if (!player.isGrounded)
                return;
            
            player.playerAnimatorManager.PlayTargetActionAnimation("Jump_Start", false);

            player.playerNetworkManager.isJumping.Value = true;
            
            player.playerNetworkManager.currentStamina.Value -= jumpStaminaCost;

            jumpDirection = PlayerCamera.Instance.transform.forward * PlayerInputManager.Instance.verticalInput;
            jumpDirection += PlayerCamera.Instance.transform.right * PlayerInputManager.Instance.horizontalInput;
            jumpDirection.y = 0;

            if (jumpDirection != Vector3.zero)
            {
                //Sprinting jump full disstance
                if (player.playerNetworkManager.isSprinting.Value)
                {
                    jumpDirection *= 1;
                } 
                //Run = 1/2 distance
                else if (PlayerInputManager.Instance.moveAmount > 0.5)
                {
                    jumpDirection *= 0.5f;
                } 
                //Walk = 1/4 distance
                else if (PlayerInputManager.Instance.moveAmount <= 0.5)
                {
                    jumpDirection *= 0.25f;
                }
            }
        }

        public void ApplyJumpingVelocity()
        {
                yVelocity.y = Mathf.Sqrt(jumpHeight * -2 * gravityForce);
        }

        public void HandleJumpMovement()
        {
            if (player.playerNetworkManager.isJumping.Value)
            {
                player.characterController.Move(jumpDirection * jumpForwardSpeed * Time.deltaTime);
            }
        }
        
        public void HandleFreeFallMovement()
        {
            if (!player.isGrounded)
            {
                Vector3 freeFallDirection =
                    PlayerCamera.Instance.transform.forward * PlayerInputManager.Instance.verticalInput;
                freeFallDirection += PlayerCamera.Instance.transform.right * PlayerInputManager.Instance.horizontalInput;
                freeFallDirection.y = 0;

                player.characterController.Move(freeFallDirection * freeFallSpeed * Time.deltaTime);
            }
        }
    }
}
