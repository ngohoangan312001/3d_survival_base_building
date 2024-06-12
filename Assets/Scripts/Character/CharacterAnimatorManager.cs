using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
namespace AN
{
    public class CharacterAnimatorManager : MonoBehaviour
    {
        CharacterManager character;

        private float vertical;
        private float horizontal;

        [Header("Damage Animations")] 
        [SerializeField] private string lastDamageAnimationPlayed;
        
        [SerializeField] private string hit_Forward_Medium_01 = "hit_Forward_Medium_01";
        [SerializeField] private string hit_Forward_Medium_02 = "hit_Forward_Medium_02";
        
        [SerializeField] private string hit_Backward_Medium_01 = "hit_Backward_Medium_01";
        [SerializeField] private string hit_Backward_Medium_02 = "hit_Backward_Medium_02";
        
        [SerializeField] private string hit_Left_Medium_01 = "hit_Left_Medium_01";
        [SerializeField] private string hit_Left_Medium_02 = "hit_Left_Medium_02";
        
        [SerializeField] private string hit_Right_Medium_01 = "hit_Right_Medium_01";
        [SerializeField] private string hit_Right_Medium_02 = "hit_Right_Medium_02";
        
        public List<string> forward_Medium_Damges = new List<string>();
        public List<string> backward_Medium_Damges = new List<string>();
        public List<string> left_Medium_Damges = new List<string>();
        public List<string> right_Medium_Damges = new List<string>();
        protected virtual void Awake()
        {
            character = GetComponent<CharacterManager>();
        }

        protected virtual void Start()
        {
            forward_Medium_Damges.Add(hit_Forward_Medium_01);
            forward_Medium_Damges.Add(hit_Forward_Medium_02);
            
            backward_Medium_Damges.Add(hit_Backward_Medium_01);
            backward_Medium_Damges.Add(hit_Backward_Medium_02);
            
            left_Medium_Damges.Add(hit_Left_Medium_01);
            left_Medium_Damges.Add(hit_Left_Medium_02);
            
            right_Medium_Damges.Add(hit_Right_Medium_01);
            right_Medium_Damges.Add(hit_Right_Medium_02);
                
        }

        public void PlayDamageAnimation(List<string> animationList)
        {
            lastDamageAnimationPlayed = GetRandomAnimationFromList(animationList);

            if (lastDamageAnimationPlayed != null)
            {
                character.characterAnimatorManager.PlayTargetActionAnimation(lastDamageAnimationPlayed,true);
            }
        }

        public string GetRandomAnimationFromList(List<string> animationList)
        {
            List<string> finalList = new List<string>();

            foreach (var item in animationList)
            {
                finalList.Add(item);
            }

            //Remove the animation damaged that already played if list have more than 1 animation
            if (finalList.Count > 1) finalList.Remove(lastDamageAnimationPlayed);
            
            //Check null entry and remove it
            ArrayUtil.RemoveNullSlotInList(finalList);

            if (finalList.Count > 0)
            {
                return ArrayUtil.ChooseRandomFromList(finalList);
            }

            return null;
        }
            
        
        public void UpdateAnimatorMovementParameters(float horizontalValue, float verticalValue)
        {
            if (!character.canMove)
            {
                return;
            }

            float snappedHorizontalMovement = GetSnappedMovementValue(horizontalValue);
            float snappedVerticalMovement = GetSnappedMovementValue(verticalValue);
            
            if (character.characterNetworkManager.isSprinting.Value)
            {
                snappedVerticalMovement = 2;
            }
            
            character.animator.SetFloat("horizontal",snappedHorizontalMovement, 0.1f, Time.deltaTime);
            character.animator.SetFloat("vertical",snappedVerticalMovement, 0.1f, Time.deltaTime);
        }

        public float GetSnappedMovementValue(float movementValue)
        {
            float snappedResult = 0;
            if (movementValue > 0 && movementValue <= 0.5f)
            {
                snappedResult = 0.5f;
            }
            else if (movementValue > 0.5f && movementValue <= 1)
            {
                snappedResult = 1f;
            }
            else if (movementValue >= -0.5f && movementValue < 0 )
            {
                snappedResult = -0.5f;
            }
            else if (movementValue >= -1 && movementValue < -0.5 )
            {
                snappedResult = -1;
            }

            return snappedResult;
        }
        
        public virtual void PlayTargetActionAnimation(
            string targetAction, 
            bool isPerformingAction, 
            bool applyRootMotion = true, 
            bool canRotate = false, 
            bool canMove = false)
        {
            //apply motion from the animation
            character.applyRootMotion = applyRootMotion;
            
            character.animator.CrossFade(targetAction, 0.2f);

            //Stop character form attempting new action
            //ex: turn flag to true when character get damaged and is performing damage animation
            //so we can check this flag before attempting a new action
            character.isPerformingAction = isPerformingAction;
            character.canRotate = canRotate;
            character.canMove = canMove;
            
            //tell server/host to play animation
            character.characterNetworkManager.NotifyServerOfActionAnimationServerRPC(NetworkManager.Singleton.LocalClientId, targetAction, applyRootMotion);
        }
        
        public virtual void PlayTargetAttackActionAnimation(
            AttackType attackType,
            string targetAction, 
            bool isPerformingAction, 
            bool applyRootMotion = true, 
            bool canRotate = false, 
            bool canMove = false)
        {
            //TODO: Check last attack performed ==> for combos
            
            //Check current attack type
            character.characterCombatManager.currentAttackType = attackType;
            
            //TODO: Update animation set to current weapon animation
            //TODO: Check if weapon can be parried
            //TODO: Update isAttacking flag to network
            
            //Save last attack animation
            character.characterCombatManager.lastAttackAnimationPerformed = targetAction;
            
            //apply motion from the animation
            character.applyRootMotion = applyRootMotion;
            
            character.animator.CrossFade(targetAction, 0.2f);

            //Stop character form attempting new action
            //ex: turn flag to true when character get damaged and is performing damage animation
            //so we can check this flag before attempting a new action
            character.isPerformingAction = isPerformingAction;
            character.canRotate = canRotate;
            character.canMove = canMove;
            
            //tell server/host to play animation
            character.characterNetworkManager.NotifyServerOfAttackActionAnimationServerRPC(NetworkManager.Singleton.LocalClientId, targetAction, applyRootMotion);
        }
        
        public virtual void EnableCanDoCombo()
        {
        }
        
        public virtual void DisableCanDoCombo()
        {
        }
    }
    
    
}
