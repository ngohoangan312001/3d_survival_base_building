using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace AN
{
    public static class ArrayUtil
    {
        /**
         * Remove null entries from a List 
         */
        public static List<T> RemoveNullSlotInList<T>(List<T> inputList)
        {
            for (int i = inputList.Count - 1; i < -1; i--)
            {
                if (inputList[i] == null)
                {
                    inputList.RemoveAt(i);
                }
            }

            return inputList;
        }
        
        /**
         * Choose a random element from a List
         */
        public static T ChooseRandomFromList<T>(List<T> inputList)
        {
            int index = Random.Range(0, inputList.Count);
            
            return inputList[index];
        }
        
        /**
         * Choose a random element from a Array
         */
        public static T ChooseRandomFromArray<T>(T[] inputArray)
        {
            int index = Random.Range(0, inputArray.Length);
            return inputArray[index];
        }
        
    }

}
