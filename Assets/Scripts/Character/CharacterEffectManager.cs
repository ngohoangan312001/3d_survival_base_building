using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AN
{
    public class CharacterEffectManager : MonoBehaviour
    {
        private CharacterManager character;

        [Header("VFX")] [SerializeField] private GameObject bloodSplatterVFX;
        private void Awake()
        {
            character = GetComponent<CharacterManager>();
        }

        //Instance Effect (take damage, heal,...)
        public virtual void ProcessInstanceEffect(InstanceCharacterEffect instanceCharacterEffect)
        {
            instanceCharacterEffect.ProcessEffect(character);
        }
        
        //Timed Effect (debuff, buff,...)
        public virtual void ProcessTimedEffect()
        {
            
        }
        
        //Static Effect (equipment stats,... => non expired)
        public virtual void ProcessStaticEffect()
        {
            
        }
        
        //--------------------
        // Visual Effect (VFX)
        //--------------------

        public void PlayBloodSplatterVFX(Vector3 contactPoint)
        {
            //If this model have a manual VFX, play it
            if (bloodSplatterVFX != null)
            {
                //Quaternion.identity tương ứng với “không xoay” - đối tượng được căn chỉnh hoàn toàn với các trục thế giới hoặc cha mẹ.
                //Khi gán Quaternion.identity cho thuộc tính rotation của một đối tượng, đối tượng đó sẽ có hướng xoay “mặc định” hoặc “tự nhiên” của nó.
                GameObject bloodSplatter = Instantiate(bloodSplatterVFX, contactPoint, Quaternion.identity);
            }
            //Else play the default version
            else
            {
                GameObject bloodSplatter = Instantiate(WorldCharacterEffectManager.Instance.bloodSplatterVFX, contactPoint, Quaternion.identity);
            }
        }
        
    }  
}
