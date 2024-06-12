using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AN
{
    public class WorldItemDatabase : MonoBehaviour
    {
        public static WorldItemDatabase Instance;
        
        public WeaponItem unarmedWeapon;
        
        //List of every weapon in game
        [Header("Weapons")]
        [SerializeField] private List<WeaponItem> weapons = new List<WeaponItem>();
        
        //List of every item in game
        [Header("Items")]
        [SerializeField] private List<Item> items = new List<Item>();
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

            //add all weapon to list of item
            foreach (WeaponItem weapon in weapons)
            {
                items.Add(weapon);
            }

            //set id for all item
            for (int i = 0; i < items.Count; i++)
            {
                items[i].itemId = i;
            }
        }

        public WeaponItem GetWeaponByID(int id)
        {
            return weapons.FirstOrDefault(weapon => weapon.itemId == id);
        }
    }
}
