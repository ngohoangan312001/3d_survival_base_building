using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AN
{
    public class MeleeWeaponDamageCollider : WeaponDamageCollider
    {
        protected override void Awake()
        {
            base.Awake();

            if (damageCollider == null)
            {
                damageCollider = GetComponent<Collider>();
            }
            
            //Disable weapon damage collider at start
            DisableDamageCollider();
        }

        protected override void OnTriggerEnter(Collider other)
        {
            CharacterManager damageTarget = other.GetComponentInParent<CharacterManager>();
            
            if (damageTarget != null)
            {
                //Prevent weapon to damaged the character wielding it
                if (damageTarget == characterCausingDamage) return;
                
                contactPoint = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
                
                //Todo: Check if the damageTarget can receive damage from this gameObject
                
                //Todo: Check if target is blocking
                
                //Todo: Check if target is invulnerable 

                DamageTarget(damageTarget);
            }
            
        }
    }
}
