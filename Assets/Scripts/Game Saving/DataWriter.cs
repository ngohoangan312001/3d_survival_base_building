using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace AN
{
    public class DataWriter
    {
        // Application.persistentDataPath is generally data path that work on every machine
        private string saveDataDirectoryPath = Application.persistentDataPath;
        public string saveFileName = "";
        
        
        /**
         * check if character slot already exists
         */
        public bool CheckFileExists()
        {
            if (File.Exists(Path.Combine(saveDataDirectoryPath, saveFileName)))
            {
                return true;
            }
            
            return false;
            
        }

        /**
         * Delete character
         */
        public void DeleteSaveFile()
        {
            if (CheckFileExists())
            {
                File.Delete(Path.Combine(saveDataDirectoryPath, saveFileName));
            }
        }
        
        /**
         * Create new save file upon starting new game
         */
        public void CreateNewSaveFile(CharacterSaveData characterData)
        {
            //Path to save file (local location)
            string savePath = Path.Combine(saveDataDirectoryPath, saveFileName);

            try
            {
                //create directory to save file, if not exist
                Directory.CreateDirectory(Path.GetDirectoryName(savePath));
                Debug.Log("Creating save file at: " + savePath);

                //Serialize the c# game data to json
                string dataToStore = JsonUtility.ToJson(characterData, true);

                //Write file to system
                using (FileStream stream = new FileStream(savePath, FileMode.Create))
                {
                    using (StreamWriter fileWriter = new StreamWriter(stream))
                    {
                        fileWriter.Write(dataToStore);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Error while trying to save character data.\nGame not saved " + savePath + "\n" + ex);
            }
        }
        
        /**
         * Load save file
         */
        public CharacterSaveData LoadSaveFile()
        {
            CharacterSaveData characterSaveData = null;
            
            //Path to load file (local location)
            string loadPath = Path.Combine(saveDataDirectoryPath, saveFileName);

            if (CheckFileExists())
            {
                try
                {
                    string dataToLoad = "";
                    //Write file to system
                    using (FileStream stream = new FileStream(loadPath, FileMode.Open))
                    {
                        using (StreamReader fileReader = new StreamReader(stream))
                        {
                            dataToLoad = fileReader.ReadToEnd();
                        }
                    }
                    //Deserialize the c# game data to json
                    characterSaveData = JsonUtility.FromJson<CharacterSaveData>(dataToLoad);
                }
                catch (Exception ex)
                {
                    Debug.LogError("Error while trying to load character data.\nCan not load " + loadPath + "\n" + ex);
                }
            }

            return characterSaveData;
        }
    }
}

