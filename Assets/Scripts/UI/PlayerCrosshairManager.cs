using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AN
{
    public class PlayerCrosshairManager : MonoBehaviour
    {
        [SerializeField] private GameObject CrossHairComponent;
        
        [Range(0, 100)] 
        public float value;
        public float multiplier = 5;
        public float speed;

        public float margin;
        public RectTransform center, top, down, right, left;

        private void Update()
        {
            ExpanseCrossHairOnValueChange();
        }

        private void ExpanseCrossHairOnValueChange()
        {
            float topValue, downValue, rightValue, leftValue;
            value *= multiplier;
            topValue = Mathf.Lerp(top.position.y, center.position.y + margin + value, speed * Time.deltaTime);
            downValue = Mathf.Lerp(down.position.y, center.position.y - margin - value, speed * Time.deltaTime);
            
            rightValue = Mathf.Lerp(right.position.x, center.position.x + margin + value, speed * Time.deltaTime);
            leftValue = Mathf.Lerp(left.position.x, center.position.x - margin - value, speed * Time.deltaTime);

            top.position = new Vector2(center.position.x, topValue);
            down.position = new Vector2(center.position.x, downValue);
            right.position = new Vector2(rightValue, center.position.y);
            left.position = new Vector2(leftValue, center.position.y);
        }

        public void ToggleCrosshair(bool previousValue, bool newValue)
        {
            CrossHairComponent.SetActive(newValue);
        }
    }
}
