using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;

namespace AN
{
    [CreateAssetMenu(menuName = "Character Effects/Instance Effects/Take Damage")]
    public class TakeDamageEffect : InstanceCharacterEffect
    {
        [Header("Character Causing Damage")] 
        //Damage cause by another character, the Character will be store here
        public CharacterManager characterCausingDamage;

        [Header("Damage")]
        public float physicalDamage = 0; //Standard, Slash, Pierce, Crush
        public float magicDamage = 0;
        public float pyroDamage = 0; //Electro
        public float hydroDamage = 0; //Ice
        public float geoDamage = 0;
        public float luminaDamage = 0;
        public float eclipseDamage = 0;

        [Header("Direction Damage taken from")]
        public float angleHitFrom; //Determine what damage animation to play (move backward, left, right,...)
        public Vector3 contactPoint; //Determine where the blood FX instantiate
        
        [Header("Final Damage")] 
        private int finalDamageDealt = 0;
        
        [Header("Poise")]
        public float poiseDamage = 0;
        public bool poiseIsBroken = false; //If character poise is broken, character will be stunned and play a damage animation
        
        //todo: Build up effects

        [Header("Animation")] 
        public bool playDamageAnimation = true;
        public bool manuallySelectDamageAnimation = false;
        public List<string> damageAnimationList;
        
        [Header("Sound FX")] 
        public bool willPlayDamageSFX = true;
        public AudioClip elementalDamageSFX; //Used on top of regular sfx if there is elemental damage present
        
        public override void ProcessEffect(CharacterManager character)
        {
            base.ProcessEffect(character);
            
            //No additional damage fx should be process if character is dead
            if (character.isDead.Value)
                return;

            CalculateDamage(character);

            //Todo: Check for "Invulnerablity"
            
            //Check direction damage came from
            PlayDirectionalBaseDamageAnimation(character);
            
            //Todo: Play damage animation
            //Todo: Check for buildup 
            //Play damage SFX
            PlayDamageSFX(character);
            //Play damage Visual FX (VFX)
            PlayDamageVFX(character);

            //Todo: If character is A.I, check for new target if character causing damage is present

        }

        private void CalculateDamage(CharacterManager character)
        {
            if (!character.IsOwner) return;
            
            if (characterCausingDamage != null)
            {
                //Todo: check for damage modifiers and modify base damage ( damage buff,....)
            }

            //Todo: Check for character flat defenses and subtract it from damage

            //Todo: Check character absorption and subtract it from damage
            
            //Add all damage types together and apply final damage
            finalDamageDealt = Mathf.RoundToInt(physicalDamage + magicDamage + pyroDamage + hydroDamage + geoDamage +
                                                luminaDamage + eclipseDamage);
            
            //Attack will deal at least 1 damage
            if (finalDamageDealt <= 0)
            {
                finalDamageDealt = 1;
            }

            character.characterNetworkManager.currentHealth.Value -= finalDamageDealt;
            
            Debug.Log("Total Dmg: " + finalDamageDealt);
            
            //Todo: Calculate poise damage
        }

        private void PlayDamageVFX(CharacterManager character)
        {
            //Play VFX with damage type
            
            character.characterEffectManager.PlayBloodSplatterVFX(contactPoint);
        }
        
        private void PlayDamageSFX(CharacterManager character)
        {
            //Play VFX with damage type

            AudioClip physicalDamageSFX = ArrayUtil.ChooseRandomFromArray(WorldSoundFXManager.Instance.physicalDamageSFX);
            
            character.characterSoundFXManager.PlaySoundFX(physicalDamageSFX);
        }
        
        private void PlayDirectionalBaseDamageAnimation(CharacterManager character)
        {
            if (!character.IsOwner) return;
            
            if (character.isDead.Value) return;
            
            poiseIsBroken = true;
            
            if (angleHitFrom >= 145 && angleHitFrom <= 180)
            {
                //Play Front Animation
                damageAnimationList = character.characterAnimatorManager.forward_Medium_Damges;
            }
            else if(angleHitFrom <= -145 && angleHitFrom >= -180)
            {
                //Play Front Animation
                damageAnimationList = character.characterAnimatorManager.forward_Medium_Damges;
            }
            else if(angleHitFrom >= -45 && angleHitFrom <= 45)
            {
                //Play Back Animation
                damageAnimationList = character.characterAnimatorManager.backward_Medium_Damges;
            }
            else if(angleHitFrom >= -144 && angleHitFrom <= -45)
            {
                //Play Left Animation
                damageAnimationList = character.characterAnimatorManager.left_Medium_Damges;
            }
            else if(angleHitFrom >= 45 && angleHitFrom <= 144)
            {
                //Play Right Animation
                damageAnimationList = character.characterAnimatorManager.right_Medium_Damges;
            }
            
            if(poiseIsBroken) character.characterAnimatorManager.PlayDamageAnimation(damageAnimationList);
        }
    }
}
