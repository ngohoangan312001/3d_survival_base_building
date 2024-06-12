using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AN
{
    public enum CharacterGroup
    {
        player,
        team01,
        team02
    }
    
    public enum CharacterSlot
    {
        CharacterSlot_01,
        CharacterSlot_02,
        CharacterSlot_03,
        CharacterSlot_04,
        CharacterSlot_05,
        CharacterSlot_06,
        CharacterSlot_07,
        CharacterSlot_08,
        CharacterSlot_09,
        CharacterSlot_10,
        NO_SLOT
    }
    
    public enum WeaponModelSlot
    {
        RightHand,
        LeftHand,
        RightHip,
        LeftHip,
        Back
    }

    public enum AttackType
    {
        LightAttack01,
        LightAttack02,
        HeavyAttack01,
        HeavyAttack02,
        ChargeAttack01,
        ChargeAttack02,
        RunningLightAttack,
        RunningHeavyAttack
    }
}