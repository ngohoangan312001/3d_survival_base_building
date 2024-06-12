using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AN
{
    public class PlayerStatManager : CharacterStatManager
    {
        private PlayerManager player;

        protected override void Awake()
        {
            base.Awake();
            player = GetComponent<PlayerManager>();
        }

        protected override void Start()
        {
            base.Start();
            
            //Calculate the starter stat by class
            CalculateHealthBaseOnStat(player.playerNetworkManager.vitality.Value);
            CalculateEnergyBaseOnStat(player.playerNetworkManager.intellect.Value);
            CalculateStaminaBaseOnStat(player.playerNetworkManager.endurance.Value);
        }
    }
}
