using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace AN
{
    public class CharacterCombatManager : NetworkBehaviour
    {
        protected CharacterManager character;
        [Header("Current Target Lock On")]
        public CharacterManager currentTarget;
        
        [Header("Lock On Transform")]
        public Transform lockOnTransform;
        
        [Header("Current Attack Type")]
        public AttackType currentAttackType; 
        
        [Header("Last Attack Animation Performed")]
        public string lastAttackAnimationPerformed; 
        
        protected virtual void Awake()
        {
            character = GetComponent<CharacterManager>();
        }

        public virtual void SetTarget(CharacterManager newTarget)
        {
            if (character.IsOwner)
            {
                if (newTarget != null)
                {
                    currentTarget = newTarget;
                    //Notify the network that target have been found and provide it info to network
                    character.characterNetworkManager.currentTargetNetworkObjectId.Value =
                        newTarget.GetComponent<NetworkObject>().NetworkObjectId;
                }
                else
                {
                    currentTarget = null;
                }
            }
        }
    }
}
