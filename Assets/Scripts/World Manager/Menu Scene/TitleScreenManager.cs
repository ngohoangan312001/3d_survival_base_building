using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
namespace AN
{
    public class TitleScreenManager : MonoBehaviour
    {
        public static TitleScreenManager instance;
            
        [Header("Menus")]
        [SerializeField] private GameObject titleScreenMainMenu;
        [SerializeField] private GameObject titleScreenLoadMenu;
        
        [Header("Buttons")]
        [SerializeField] Button loadGameMenuReturnButton;
        [SerializeField] Button loadGameMenuOpenButton;
        [SerializeField] Button titleScreenMainMenuNewGameButton;
        [SerializeField] private Button noCharacterSlotPopupConfirmButton;
        [SerializeField] private Button deleteCharacterSlotPopupConfirmButton;
        [SerializeField] private Button deleteCharacterSlotPopupReturnButton;

        [Header("Pop Ups")]
        [SerializeField] private GameObject noCharacterSlotPopup;
        [SerializeField] private GameObject deleteCharacterSlotPopup;

        [Header("Character Slots")] public CharacterSlot currentSelectedSlot = CharacterSlot.NO_SLOT;
        
        private void Awake()
        {
            //Can only have 1 instance of WorldSaveGameManager
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        public void StartNetworkAtHost()
        {
            NetworkManager.Singleton.StartHost();
        }

        public void StartNewGame()
        {
            WorldSaveGameManager.Instance.AttemptToCreateNewGame();
        }
        
        public void ExitGame()
        {
            Application.Quit();
        }
        //--------------------CHRACTER SLOT--------------------------
        public void OpenTheLoadGameMenu()
        {
            //Open load menu
            titleScreenLoadMenu.SetActive(true);
            //Close main menu
            titleScreenMainMenu.SetActive(false);
            
            //select return button
            loadGameMenuReturnButton.Select(); 
        }
        
        public void CloseTheLoadGameMenu()
        {
            //Close load menu
            titleScreenLoadMenu.SetActive(false);
            //Open main menu
            titleScreenMainMenu.SetActive(true);
            
            //select load game button
            loadGameMenuOpenButton.Select();
        }

        public void OpenNoEmptySlotPopUp()
        {
            noCharacterSlotPopup.SetActive(true);
            noCharacterSlotPopupConfirmButton.Select();
        }
        
        public void CloseNoEmptySlotPopUp()
        {
            noCharacterSlotPopup.SetActive(false);
            titleScreenMainMenuNewGameButton.Select();
        }

        public void SelectCharacterSlot(CharacterSlot characterSlot)
        {
            currentSelectedSlot = characterSlot;
        }

        public void SelectNoSlot()
        {
            currentSelectedSlot = CharacterSlot.NO_SLOT;
        }

        public void AttemptToDeleteScharacterSlot()
        {
            if (currentSelectedSlot == CharacterSlot.NO_SLOT)
            {
                return;
            }
            deleteCharacterSlotPopup.SetActive(true);
            deleteCharacterSlotPopupReturnButton.Select();
        }
        
        public void ConfirmDeleteScharacterSlotPopUp()
        {
            WorldSaveGameManager.Instance.DeleteGame(currentSelectedSlot);
            
            //refresh game slot list
            titleScreenLoadMenu.SetActive(false);
            titleScreenLoadMenu.SetActive(true);
            
            CloseDeleteScharacterSlotPopUp();
        }
        
        public void CloseDeleteScharacterSlotPopUp()
        {
            deleteCharacterSlotPopup.SetActive(false);
            loadGameMenuReturnButton.Select();
        }
        
        //-------------------------END CHARACTER SLOT--------------------------
    }

}
