using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace AN
{
    public class CharacterNetworkManager : NetworkBehaviour
    {
        private CharacterManager character;
        
        [Header("Positions")]
        public NetworkVariable<Vector3> networkPosition = new NetworkVariable<Vector3>(Vector3.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<Quaternion> networkRotation = new NetworkVariable<Quaternion>(Quaternion.identity, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public Vector3 networkPositionVelocity;
        public float networkPositionSmoothTime = 0.1f;
        public float networkRotationSmoothTime = 0.1f;
        
        [Header("Animators")]
        public NetworkVariable<float> animaterHorizontalNetworkParameter = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<float> animaterVerticalNetworkParameter = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<float> networkMoveAmount = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        [Header("Flags")]
        public NetworkVariable<ulong> currentTargetNetworkObjectId = new NetworkVariable<ulong>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        [Header("Flags")]
        public NetworkVariable<bool> isSprinting = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<bool> isAiming = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<bool> isLockOn = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<bool> isJumping = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<bool> isChargingAttack = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        [Header("Stats")]
        public NetworkVariable<int> vitality = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<int> intellect = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<int> endurance = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        [Header("Resources")]
        public NetworkVariable<float> currentHealth = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<int> maxHealth = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<float> currentEnergy = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<int> maxEnergy = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<float> currentStamina = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<int> maxStamina = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<float> weaponSwitchCooldownTime = new NetworkVariable<float>(3, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        
        
        protected virtual void Awake()
        {
            character = GetComponent<CharacterManager>();
        }

        public void CheckHP(float oldValue, float newValue)
        {
            if (!IsOwner) return;
            
            if (currentHealth.Value <= 0)
            {
                StartCoroutine(character.ProcessDeathEvent());
            }

            if (currentHealth.Value > maxHealth.Value)
            {
                currentHealth.Value = maxHealth.Value;
            }
        }

        public void OnIsChargingAttackChange(bool oldValue, bool newValue)
        {
            character.animator.SetBool("isChargingAttack", newValue);
        }
        
        public void OnLockOnTargetIdChange(ulong oldId, ulong newId)
        {
            if (!IsOwner)
            {
                //Truy cập vào một đối tượng đã được tạo (spawned) thông qua NetworkManager
                character.characterCombatManager.currentTarget = NetworkManager.Singleton.SpawnManager.SpawnedObjects[newId]
                    .GetComponent<CharacterManager>();
            }
        }
        
        public void OnIsLockOnValueChange(bool oldValue, bool newValue)
        {
            //if not lock on clear current lock on target
            if (!isLockOn.Value)
            {
                character.characterCombatManager.currentTarget = null;
            }
        }
        
        //--------------------
        //ACTION ANIMATION RPC
        //--------------------
        
        //Code that is run on the Server (Host) , called by a Client.
        [ServerRpc]
        public void NotifyServerOfActionAnimationServerRPC(ulong clientId, string animationId, bool applyRootMotion)
        {
            // This character is the host -> send it id and animation to all client for another client to see it animation
            // IsServer: This property returns true if the object is active on an active server. This is only true if the object has been spawned
            if (IsServer)
            {
                PlayActionAnimationForAllClientClientRPC(clientId, animationId, applyRootMotion);
            }
        }
        
        //Code that is run on the Client, called by a Server (Host).
        //When the server is noticed that there is an action animation being performed
        //It will call this procedure, in this case: this character.animation == the once character who noticed the server will be call
        [ClientRpc]
        private void PlayActionAnimationForAllClientClientRPC(ulong clientId, string animationId, bool applyRootMotion)
        {
            if (clientId != NetworkManager.Singleton.LocalClientId)
            {
                PerformActionAnimationFormServer(animationId, applyRootMotion);
            }
        }

        private void PerformActionAnimationFormServer( string animationId, bool applyRootMotion)
        {
            character.applyRootMotion = applyRootMotion;
            character.animator.CrossFade(animationId, 0.2f);
        }
        
        //---------------------------
        //ATTACK ACTION ANIMATION RPC
        //---------------------------
        
        [ServerRpc]
        public void NotifyServerOfAttackActionAnimationServerRPC(ulong clientId, string animationId, bool applyRootMotion)
        {
            // This character is the host -> send it id and animation to all client for another client to see it animation
            // IsServer: This property returns true if the object is active on an active server. This is only true if the object has been spawned
            if (IsServer)
            {
                PlayAttackActionAnimationForAllClientClientRPC(clientId, animationId, applyRootMotion);
            }
        }
        
        [ClientRpc]
        private void PlayAttackActionAnimationForAllClientClientRPC(ulong clientId, string animationId, bool applyRootMotion)
        {
            if (clientId != NetworkManager.Singleton.LocalClientId)
            {
                PerformAttackActionAnimationFormServer(animationId, applyRootMotion);
            }
        }

        private void PerformAttackActionAnimationFormServer( string animationId, bool applyRootMotion)
        {
            character.applyRootMotion = applyRootMotion;
            character.animator.CrossFade(animationId, 0.2f);
        }
        
        //---------------------------
        //DAMAGE RPC
        //---------------------------
        
        [ServerRpc(RequireOwnership = false)]
        public void NotifyServerOfCharacterDamageServerRPC
        (
            ulong damagedCharacterId, 
            ulong characterCausingDamageId,
            float physicalDamage,
            float magicDamage,
            float pyroDamage, 
            float hydroDamage,
            float geoDamage,
            float luminaDamage,
            float eclipseDamage,
            float poiseDamage,
            float angleHitFrom,
            float contactPointX,
            float contactPointY,
            float contactPointZ
        )
        {
                NotifyServerOfCharacterDamageClientRPC
                (
                    damagedCharacterId, 
                    characterCausingDamageId,
                    physicalDamage,
                    magicDamage,
                    pyroDamage, 
                    hydroDamage,
                    geoDamage,
                    luminaDamage,
                    eclipseDamage,
                    poiseDamage,
                    angleHitFrom,
                    contactPointX,
                    contactPointY,
                    contactPointZ
                );
        }
        
       
        [ClientRpc]
        public void NotifyServerOfCharacterDamageClientRPC
        (
            ulong damagedCharacterId, 
            ulong characterCausingDamageId,
            float physicalDamage,
            float magicDamage,
            float pyroDamage, 
            float hydroDamage,
            float geoDamage,
            float luminaDamage,
            float eclipseDamage,
            float poiseDamage,
            float angleHitFrom,
            float contactPointX,
            float contactPointY,
            float contactPointZ
        )
        {
                ProcessCharacterDamageFromServer
                (
                    damagedCharacterId, 
                    characterCausingDamageId,
                    physicalDamage,
                    magicDamage,
                    pyroDamage, 
                    hydroDamage,
                    geoDamage,
                    luminaDamage,
                    eclipseDamage,
                    poiseDamage,
                    angleHitFrom,
                    contactPointX,
                    contactPointY,
                    contactPointZ
                );
        }

        private void ProcessCharacterDamageFromServer
        (
            ulong damagedCharacterId, 
            ulong characterCausingDamageId,
            float physicalDamage,
            float magicDamage,
            float pyroDamage, 
            float hydroDamage,
            float geoDamage,
            float luminaDamage,
            float eclipseDamage,
            float poiseDamage,
            float angleHitFrom,
            float contactPointX,
            float contactPointY,
            float contactPointZ
        )
        {
            CharacterManager damagedCharacter = NetworkManager.Singleton.SpawnManager.SpawnedObjects[damagedCharacterId]
                .gameObject.GetComponent<CharacterManager>();
            
            CharacterManager characterCausingDamage = NetworkManager.Singleton.SpawnManager.SpawnedObjects[characterCausingDamageId]
                .gameObject.GetComponent<CharacterManager>();

            TakeDamageEffect damageEffect = Instantiate(WorldCharacterEffectManager.Instance.takeDamageEffect);
            
            damageEffect.physicalDamage = physicalDamage;
            damageEffect.magicDamage = magicDamage;
            damageEffect.pyroDamage = pyroDamage;
            damageEffect.hydroDamage = hydroDamage;
            damageEffect.geoDamage = geoDamage;
            damageEffect.luminaDamage = luminaDamage;
            damageEffect.eclipseDamage = eclipseDamage;
            damageEffect.poiseDamage = poiseDamage;
            
            damageEffect.angleHitFrom = angleHitFrom;
            damageEffect.contactPoint = new Vector3(contactPointX, contactPointY, contactPointZ);
            damageEffect.characterCausingDamage = characterCausingDamage;
            
            damagedCharacter.characterEffectManager.ProcessInstanceEffect(damageEffect);
        }
    }
}
