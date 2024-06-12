using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AN
{
    public class PlayerEffectManager : CharacterEffectManager
    {
        [Header("debug")] 
        [SerializeField] private InstanceCharacterEffect _effectTest;
        [SerializeField] private bool processEffect;

        private void Update()
        {
            if (processEffect)
            {
                processEffect = false;
                
                //Instantiating a coppy of InstanceCharacterEffect which is a Scriptable Object
                //To not change the value of the original Scriptable Object when calculating
                //Because any change make on the original value will be save
                //EX: damageValue = 20 ===> after calculate will be 15
                //But the value on the original will stay 20, because it calculate on a coppy one
                InstanceCharacterEffect effect = Instantiate(_effectTest);
                ProcessInstanceEffect(effect);
            }
        }
    }
}
