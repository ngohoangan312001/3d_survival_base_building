using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AN
{
    //[System.Serializable] được sử dụng để chỉ ra rằng một lớp hoặc cấu trúc có thể được tuần tự hóa.
    //cho phép lưu trữ trạng thái của một đối tượng và tái tạo nó sau này.
    //Hữu ích khi muốn lưu trữ dữ liệu trò chơi, chẳng hạn như điểm số, tiến trình của người chơi, cấu hình ...
    
    //Ngoài ra, thuộc tính [System.Serializable] cũng cho phép chỉnh sửa các đối tượng của lớp trong Inspector của Unity,
    //giúp thiết lập dữ liệu trò chơi mà không cần viết mã.
    [System.Serializable]
    public class CharacterSaveData
    {
        public string characterName = "UNKNOW";

        [Header("Scene Index")] public int sceneIndex = 1;
        
        [Header("Time Played")] 
        public float secondsPlayed;

        [Header("World Coordinates")] 
        public float xPosition;
        public float yPosition;
        public float zPosition;
        
        [Header("Stats")]
        public int vitality;
        public int intellect;
        public int endurance;
        
        [Header("Resources")]
        public float currentHealth;
        public float currentEnergy;
        public float currentStamina;
        
    }
}
