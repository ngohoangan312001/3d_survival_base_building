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
                
            }
            else
            {
                // continue search for target until find one
                aiCharacterManager.aiCharacterCombatManager.FindTargetViaLineOfSign(aiCharacterManager);
            }
            
            return this;
        }
    }
}
