using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AN
{
    public class InstanceCharacterEffect : ScriptableObject
    {
        [Header("effect ID")] public int instanceEffectID;

        public virtual void ProcessEffect(CharacterManager character)
        {
            
        }
    }
}

