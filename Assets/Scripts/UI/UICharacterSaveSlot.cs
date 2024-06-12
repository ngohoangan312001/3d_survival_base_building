using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace AN
{
    public class UICharacterSaveSlot : MonoBehaviour
    {
        private DataWriter dataWriter;

        [Header("Game Slot")] 
        public CharacterSlot characterSlot;
        public TextMeshProUGUI slotNumber;

        [Header("Character Info")] 
        public TextMeshProUGUI characterName;
        public TextMeshProUGUI timePlayed;
        public TextMeshProUGUI world;

        private void Awake()
        {
            dataWriter = new DataWriter();
        }

        private void OnEnable()
        {
            LoadSaveSlot();
        }

        private void LoadSaveSlot()
        {
            // foreach (CharacterSlot slot in (CharacterSlot[])Enum.GetValues(typeof(CharacterSlot)))
            // {
            //     if (characterSlot != slot) continue;
            //     
            //     getCharacterInfoInSlot( WorldSaveGameManager.instance.characterSlot02);
            //     break;
            // }
            
                switch (characterSlot)
                {
                    case CharacterSlot.CharacterSlot_01:
                        getCharacterInfoInSlot( WorldSaveGameManager.Instance.characterSlot01);
                        break;
                    case CharacterSlot.CharacterSlot_02:
                        getCharacterInfoInSlot( WorldSaveGameManager.Instance.characterSlot02);
                        break;
                    case CharacterSlot.CharacterSlot_03:
                        getCharacterInfoInSlot( WorldSaveGameManager.Instance.characterSlot03);
                        break;
                    case CharacterSlot.CharacterSlot_04:
                        getCharacterInfoInSlot( WorldSaveGameManager.Instance.characterSlot04);
                        break;
                    case CharacterSlot.CharacterSlot_05:
                        getCharacterInfoInSlot( WorldSaveGameManager.Instance.characterSlot05);
                        break;
                    case CharacterSlot.CharacterSlot_06:
                        getCharacterInfoInSlot( WorldSaveGameManager.Instance.characterSlot06);
                        break;
                    case CharacterSlot.CharacterSlot_07:
                        getCharacterInfoInSlot( WorldSaveGameManager.Instance.characterSlot07);
                        break;
                    case CharacterSlot.CharacterSlot_08:
                        getCharacterInfoInSlot( WorldSaveGameManager.Instance.characterSlot08);
                        break;
                    case CharacterSlot.CharacterSlot_09:
                        getCharacterInfoInSlot( WorldSaveGameManager.Instance.characterSlot09);
                        break;
                    case CharacterSlot.CharacterSlot_10:
                        getCharacterInfoInSlot( WorldSaveGameManager.Instance.characterSlot10);
                        break;
                    default:
                        break;
            }
        }
        
        private void getCharacterInfoInSlot( CharacterSaveData characterSaveData)
        {
            dataWriter.saveFileName =   
                WorldSaveGameManager.Instance.GetCharacterFileNameBaseOnCharacterSlot(characterSlot);

            if (dataWriter.CheckFileExists())
            {
                slotNumber.text = (Array.IndexOf(Enum.GetValues(typeof(CharacterSlot)), characterSlot) + 1).ToString();
                characterName.text = characterSaveData.characterName;
                timePlayed.text = "" + characterSaveData.secondsPlayed;
                world.text = "" + characterSaveData.sceneIndex;
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        public void LoadGameFromCharacterSlot()
        {
            WorldSaveGameManager.Instance.currentCharacterSlot = characterSlot;
            WorldSaveGameManager.Instance.LoadGame();
        }

        public void SelectCurrentSlot()
        {
            TitleScreenManager.instance.SelectCharacterSlot(characterSlot);
        }
    }
}
