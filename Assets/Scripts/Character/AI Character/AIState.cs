using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AN
{
    public class AIState : ScriptableObject
    {
        public virtual AIState Tick(AICharacterManager aiCharacterManager)
        {
            return this;
        }
    }
}
