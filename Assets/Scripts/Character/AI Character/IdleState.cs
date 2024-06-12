using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AN
{
    [CreateAssetMenu(menuName = "A.I/State/Idle")]
    public class IdleState : AIState
    {
        public override AIState Tick(AICharacterManager aiCharacterManager)
        {
            if (aiCharacterManager.characterCombatManager.currentTarget != null)
            {
                Debug.Log( " Target "+aiCharacterManager.characterCombatManager.currentTarget.name+" Detected");
            }
            else
            {
                // continue search for target until find one
                aiCharacterManager.aiCharacterCombatManager.FindTargetViaLineOfSign(aiCharacterManager);
                Debug.Log(aiCharacterManager.name + " Searching for target");
            }
            
            return this;
        }
    }
}
