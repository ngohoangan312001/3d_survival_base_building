using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyNamespace
{
    public class WorldAIManager : MonoBehaviour
    {
        public static WorldAIManager Instance;

        [Header("Characters")] 
        [SerializeField] private bool despawnAllCharacter;
        [SerializeField] private bool respawnAllCharacter;
        
        [Header("Characters")] 
        [SerializeField] private GameObject[] aiCharacter;
        [SerializeField] private List<GameObject> spawnedInChracter;

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

        private void Update()
        {
            if (respawnAllCharacter)
            {
                respawnAllCharacter = false;
                SpawnAllCharacters();
            }
            
            if (despawnAllCharacter)
            {
                despawnAllCharacter = false;
                DespawnAllCharacters();
            }
        }

        private void Start()
        {
            if (NetworkManager.Singleton.IsServer)
            {
                StartCoroutine(WaitForSceneToLoadAndSpawnCharacter());
            }
        }

        public IEnumerator WaitForSceneToLoadAndSpawnCharacter()
        {
            while (!SceneManager.GetActiveScene().isLoaded)
            {
                yield return null;
            }

            SpawnAllCharacters();
        }

        public void SpawnAllCharacters()
        {
            foreach (var character in aiCharacter)
            {
                Debug.Log("spawn");
                GameObject instantiateCharacter = Instantiate(character);
                instantiateCharacter.GetComponent<NetworkObject>().Spawn();
                
                spawnedInChracter.Add(instantiateCharacter);
            }
        }
        
        public void DespawnAllCharacters()
        {
            foreach (var character in spawnedInChracter)
            {
                Debug.Log("spawn");
                character.GetComponent<NetworkObject>().Despawn();
            }
            
            spawnedInChracter.Clear();
        }

        public void DisableAllCharacter()
        {
            //Todo: 
        }
    }
}
