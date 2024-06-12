using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Serialization;

namespace AN
{
    public class CharacterManager : NetworkBehaviour
    {
        [HideInInspector] public CharacterController characterController;
        [HideInInspector] public Animator animator;
        [HideInInspector] public CharacterNetworkManager characterNetworkManager;
        [HideInInspector] public CharacterEffectManager characterEffectManager;
        [HideInInspector] public CharacterAnimatorManager characterAnimatorManager;
        [HideInInspector] public CharacterCombatManager characterCombatManager;
        [HideInInspector] public CharacterLocomotionManager characterLocomotionManager;
        [HideInInspector] public CharacterSoundFXManager characterSoundFXManager;
        
        [Header("Status")]
        public NetworkVariable<bool> isDead = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        [Header("Character Group")] 
        public CharacterGroup characterGroup;
        
        [Header("Flag")] 
        public bool isPerformingAction = false;
        public bool isGrounded = false;
        public bool applyRootMotion = false;
        public bool canRotate = true;
        public bool canMove = true;
        
        protected virtual void Awake()
        {
            DontDestroyOnLoad(this);
            characterCombatManager = GetComponent<CharacterCombatManager>();
            characterController = GetComponent<CharacterController>();
            characterNetworkManager = GetComponent<CharacterNetworkManager>();
            animator = GetComponent<Animator>();
            characterEffectManager = GetComponent<CharacterEffectManager>();
            characterAnimatorManager = GetComponent<CharacterAnimatorManager>();
            characterLocomotionManager = GetComponent<CharacterLocomotionManager>();
            characterSoundFXManager = GetComponent<CharacterSoundFXManager>();
        }

        protected virtual void Start()
        {
            IgnoreMyOwnColliders();
        }

        protected virtual void Update()
        {
            animator.SetBool("isGrounded", isGrounded);
            animator.SetBool("isAiming", characterNetworkManager.isAiming.Value);
            // if character being controlled by owner, then assign posiotion to it network position to make it move on other client
            if (IsOwner)
            {
                characterNetworkManager.networkPosition.Value = transform.position;
                characterNetworkManager.networkRotation.Value = transform.rotation;
            }
            //if character is being controller by other, get it network position and update it position on our client
            else
            { 
                
            //=====Position=====
            //Hàm Vector3.SmoothDamp trong Unity được sử dụng để mượt dần một vector về một mục tiêu mong muốn theo thời gian.
            //Thường được sử dụng để làm cho một vector di chuyển mượt mà, vd như trong trường hợp của một camera theo dõi

            //Parameter
            //current: Vector hiện tại bạn muốn mượt dần.
            //target: Vector mục tiêu bạn muốn đạt được.
            //currentVelocity: Tham chiếu đến vector tốc độ hiện tại(sẽ được cập nhật bởi hàm).
            //smoothTime: Thời gian mượt dần(thường là giá trị dương).
            //maxSpeed(tùy chọn): Tốc độ tối đa(mặc định là vô cùng).
            //deltaTime(tùy chọn): Khoảng thời gian giữa các khung hình(mặc định là Time.deltaTime).
               transform.position = Vector3.SmoothDamp(
                    transform.position,
                    characterNetworkManager.networkPosition.Value,
                    ref characterNetworkManager.networkPositionVelocity,
                    characterNetworkManager.networkPositionSmoothTime);
               
               
            //=====Rotation=====
               
            // Quaternion.Slerp (short for Spherical Linear Interpolation) là một phương thức trong Unity
            // được sử dụng để mượt dần giữa hai Quaternion theo một đường cung trên bề mặt của một quả cầu.
            // Thường được sử dụng để tạo ra chuyển động mượt mà giữa hai hướng quay trong không gian 3D .   
               transform.rotation = Quaternion.Slerp(
                   transform.rotation,
                   characterNetworkManager.networkRotation.Value,
                   characterNetworkManager.networkRotationSmoothTime);
            }
            
        }

        protected virtual void LateUpdate()
        {
        }
        
        protected virtual void FixedUpdate()
        {
        }
        public virtual IEnumerator ProcessDeathEvent(bool manuallySelectDeathAnimation = false)
        {
            if (IsOwner)
            {
                characterNetworkManager.currentHealth.Value = 0;
                isDead.Value = true;
                
                //Reset any flag need to be reset
                
                //Todo: if not grounded play aerial death animation
                
                if (!manuallySelectDeathAnimation)
                {
                    characterAnimatorManager.PlayTargetActionAnimation("Death",true,false,false,false);
                }
            }
            
            //Todo: play death SFX
            
            yield return new WaitForSeconds(5);
            
            //Todo: receive item if A.I die
            
            //Todo: Disable character
        }

        public virtual void ReviveCharacter()
        {
            
        }

        protected virtual void IgnoreMyOwnColliders()
        {
            Collider characterControlCollider = GetComponent<Collider>();
            Collider[] damageableCollider = GetComponentsInChildren<Collider>();

            //List of all collider in character
            List<Collider> ignoreColliders = new List<Collider>();
            
            //Add all collider in character game object to list
            ignoreColliders.AddRange(damageableCollider);
            //Add character controller collider to list
            ignoreColliders.Add(characterControlCollider);

            foreach (var a in ignoreColliders)
            {
                foreach (var otherCollider in ignoreColliders)
                {
                    Physics.IgnoreCollision(a, otherCollider, true);
                }
            }
        }
    }
}
