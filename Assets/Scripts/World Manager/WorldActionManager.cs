using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AN
{
    public class WorldActionManager : MonoBehaviour
    {
        public static WorldActionManager Instance;

        [Header("Weapon Item Action")] public WeaponItemAction[] weaponItemActions;
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
            
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            for (int i = 0; i < weaponItemActions.Length; i++)
            {
                weaponItemActions[i].actionId = i;
            }
        }

        public WeaponItemAction GetWeaponItemAction(int id)
        {
            return weaponItemActions.FirstOrDefault(action => action.actionId == id);
        }
    }
}
