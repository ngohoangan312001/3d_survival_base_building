using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AN
{
    public class WorldUtilityManager : MonoBehaviour
    {
        public static WorldUtilityManager Instance;

        [Header("Layers")]
        [SerializeField] private LayerMask characterLayers;
        [SerializeField] private LayerMask enviromentLayers;
        
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

        public LayerMask GetCharacterLayers()
        {
            return characterLayers;
        }
        
        public LayerMask GetEnviromentLayers()
        {
            return enviromentLayers;
        }

        public bool CheckCharacterGroup(CharacterGroup attackingCharacter, CharacterGroup targetCharacter)
        {
            if (attackingCharacter != targetCharacter) return true;

            return false;
        }
    }
}
