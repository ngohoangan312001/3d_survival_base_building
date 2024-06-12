using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AN
{
    public class TitleScreenLoadInputManager : MonoBehaviour
    {
        private PlayerControls playerControls;
        [Header("Title Screen Input")] public bool deleteCharacterSlot;

        private void Update()
        {
            if (deleteCharacterSlot)
            {
                deleteCharacterSlot = false;
                
                TitleScreenManager.instance.AttemptToDeleteScharacterSlot();
            }
        }

        private void OnEnable()
        {
            if (playerControls == null)
            {
                playerControls = new PlayerControls();
                playerControls.UI.Delete.performed += i => deleteCharacterSlot = true;
            }
            
            playerControls.Enable();
        }

        private void OnDisable()
        {
            playerControls.Disable();
        }
    }
}
