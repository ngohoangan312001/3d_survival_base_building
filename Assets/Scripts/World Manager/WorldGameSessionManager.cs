using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AN
{
    public class WorldGameSessionManager : MonoBehaviour
    {
        public static WorldGameSessionManager Instance;
        
        [Header("Active Players In Session")] 
        public List<PlayerManager> players = new List<PlayerManager>();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void AddPlayerToActivePlayerList(PlayerManager player)
        {
            //Check if player already in the list
            if (!players.Contains(player))
            {
                players.Add(player);
            }
            
            //Check list for null slot from the end of entries then remove
            //To make sure there won't be any case that we delete player but it remain on the list with null value.
            players = ArrayUtil.RemoveNullSlotInList(players);
        }
        
        public void RemovePlayerToActivePlayerList(PlayerManager player)
        {
            //Check if player already in the list
            if (players.Contains(player))
            {
                players.Remove(player);
            }
            
            players = ArrayUtil.RemoveNullSlotInList(players);
        }
    }
}
