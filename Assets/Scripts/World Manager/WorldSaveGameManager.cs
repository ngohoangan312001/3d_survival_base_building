using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AN
{
    public class WorldSaveGameManager : MonoBehaviour
    {
        public static WorldSaveGameManager Instance;

        public PlayerManager player;

        [Header("Save/Load")] 
        [SerializeField] private bool saveGame;
        [SerializeField] private bool loadGame;
        
        [Header("World Sence Index")]
        [SerializeField] int worldSceneIndex = 1;

        [Header("Current character data")] 
        public CharacterSlot currentCharacterSlot;

        public CharacterSaveData currentCharacterData;

        [Header("Save Data Writer")] private DataWriter dataWriter;
        
        [Header("Character Slots")] 
        public CharacterSaveData characterSlot01;
        public CharacterSaveData characterSlot02;
        public CharacterSaveData characterSlot03;
        public CharacterSaveData characterSlot04;
        public CharacterSaveData characterSlot05;
        public CharacterSaveData characterSlot06;
        public CharacterSaveData characterSlot07;
        public CharacterSaveData characterSlot08;
        public CharacterSaveData characterSlot09;
        public CharacterSaveData characterSlot10;
        private void Awake()
        {
            //Can only have 1 instance of WorldSaveGameManager
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            dataWriter = new DataWriter();
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
            LoadAllCharacterProfiles();
        }

        private void Update()
        {
            if (saveGame)
            {
                saveGame = false;
                SaveGame();
            }
            
            if (loadGame)
            {
                loadGame = false;
                LoadGame();
            }
        }

        public string GetCharacterFileNameBaseOnCharacterSlot(CharacterSlot characterSlot)
        {
            string fileName = "";

            foreach (CharacterSlot slot in (CharacterSlot[])Enum.GetValues(typeof(CharacterSlot)))
            {
                if (characterSlot == CharacterSlot.NO_SLOT || characterSlot != slot) continue;
                
                fileName = slot.ToString();
                break;
            }

            return fileName;
        }

        public void AttemptToCreateNewGame()
        {
            bool haveEmptySlot = false;

            foreach (CharacterSlot slot in (CharacterSlot[]) Enum.GetValues(typeof(CharacterSlot)))
            {
                if(slot == CharacterSlot.NO_SLOT) continue;
                
                haveEmptySlot = CheckAndCreateNewGameFileOnEmptySlot(slot);
               
                if (haveEmptySlot) break;
            }
            
            if (!haveEmptySlot)
            {
                TitleScreenManager.instance.OpenNoEmptySlotPopUp();
            }
            
        }

        private bool CheckAndCreateNewGameFileOnEmptySlot(CharacterSlot characterSlot)
        {
            //Check if able to create new file (have empty slot)
            dataWriter.saveFileName = GetCharacterFileNameBaseOnCharacterSlot(characterSlot);
            if (!dataWriter.CheckFileExists() )
            {
                //If slot is not taken => using this to create new game data
                currentCharacterSlot = characterSlot;
                currentCharacterData = new CharacterSaveData();
                NewGame();
                return true;
            }

            return false;
        }
        
        //Load all character data on device
        private void LoadAllCharacterProfiles()
        {
            
            dataWriter.saveFileName = GetCharacterFileNameBaseOnCharacterSlot(CharacterSlot.CharacterSlot_01);
            characterSlot01 = dataWriter.LoadSaveFile();

            dataWriter.saveFileName = GetCharacterFileNameBaseOnCharacterSlot(CharacterSlot.CharacterSlot_02);
            characterSlot02 = dataWriter.LoadSaveFile();
            
            dataWriter.saveFileName = GetCharacterFileNameBaseOnCharacterSlot(CharacterSlot.CharacterSlot_03);
            characterSlot03 = dataWriter.LoadSaveFile();
            
            dataWriter.saveFileName = GetCharacterFileNameBaseOnCharacterSlot(CharacterSlot.CharacterSlot_04);
            characterSlot04 = dataWriter.LoadSaveFile();
            
            dataWriter.saveFileName = GetCharacterFileNameBaseOnCharacterSlot(CharacterSlot.CharacterSlot_05);
            characterSlot05 = dataWriter.LoadSaveFile();
            
            dataWriter.saveFileName = GetCharacterFileNameBaseOnCharacterSlot(CharacterSlot.CharacterSlot_06);
            characterSlot06 = dataWriter.LoadSaveFile();
            
            dataWriter.saveFileName = GetCharacterFileNameBaseOnCharacterSlot(CharacterSlot.CharacterSlot_07);
            characterSlot07 = dataWriter.LoadSaveFile();
            
            dataWriter.saveFileName = GetCharacterFileNameBaseOnCharacterSlot(CharacterSlot.CharacterSlot_08);
            characterSlot08 = dataWriter.LoadSaveFile();
            
            dataWriter.saveFileName = GetCharacterFileNameBaseOnCharacterSlot(CharacterSlot.CharacterSlot_09);
            characterSlot09 = dataWriter.LoadSaveFile();
            
            dataWriter.saveFileName = GetCharacterFileNameBaseOnCharacterSlot(CharacterSlot.CharacterSlot_10);
            characterSlot10 = dataWriter.LoadSaveFile();
        }

        private void NewGame()
        {
            //Save game immediately after create new character
            player.playerNetworkManager.vitality.Value = 5;
            player.playerNetworkManager.intellect.Value = 5;
            player.playerNetworkManager.endurance.Value = 5;
            SaveGame();
            StartCoroutine(LoadWorldSence());
        }
        
        public void LoadGame()
        {
            //load file base on character slot
            dataWriter.saveFileName = GetCharacterFileNameBaseOnCharacterSlot(currentCharacterSlot);
            
            currentCharacterData = dataWriter.LoadSaveFile();
            
            //Pass player data to current player
            player.LoadGameDataFromCurrentCharacterData(ref currentCharacterData);
            
            StartCoroutine(LoadWorldSence());
        }

        public void SaveGame()
        {
            //Save file base on character slot
            dataWriter.saveFileName = GetCharacterFileNameBaseOnCharacterSlot(currentCharacterSlot);
            
            //Pass player info to save file
            player.SaveGameDataToCurrentCharacterData(ref currentCharacterData);
            //Write player info to json file and save on local machine
            dataWriter.CreateNewSaveFile(currentCharacterData);

        }

        public void DeleteGame(CharacterSlot characterSlot)
        {
            //load file base on character slot
            dataWriter.saveFileName = GetCharacterFileNameBaseOnCharacterSlot(characterSlot);
            
            dataWriter.DeleteSaveFile();
        }
        
        public IEnumerator LoadWorldSence()
        {
            //Only 1 world scene
            //AsyncOperation loadOperation = SceneManager.LoadSceneAsync(worldSceneIndex);

            //To use different scene for level
            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(currentCharacterData.sceneIndex);
            
            player.LoadGameDataFromCurrentCharacterData(ref currentCharacterData);
            
            yield return null;
        }

        public int GetWorldSceneIndex()
        {
            return worldSceneIndex;
        }
    }
}