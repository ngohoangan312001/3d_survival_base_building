using System;
using System.Collections;
using System.Collections.Generic;
using AN;
using Unity.Netcode;
using UnityEngine;

public class PlayerUIManager : MonoBehaviour
{
    public static PlayerUIManager instance;

    [HideInInspector] public PlayerUIHudManager playerUIHudManager;
    [HideInInspector] public PlayerUIPopUpManager playerUIPopUpManager;
    [HideInInspector] public PlayerCrosshairManager playerCrosshairManager;

    private Canvas canvas;

    [SerializeField] private bool joinNetwork;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        playerUIHudManager = GetComponentInChildren<PlayerUIHudManager>();
        
        playerUIPopUpManager = GetComponentInChildren<PlayerUIPopUpManager>();
        
        playerCrosshairManager = GetComponentInChildren<PlayerCrosshairManager>();

        canvas = GetComponent<Canvas>();
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (joinNetwork)
        {
            joinNetwork = false;
            StartGameAsClient();
        }
    }

    public void StartGameAsClient()
    {
        //Shut down because have started as host during the title screen
        NetworkManager.Singleton.Shutdown();

        //Then restart as client
        NetworkManager.Singleton.StartClient();
    }

}
