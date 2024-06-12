using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace AN
{
    public class UI_StatBar : MonoBehaviour
    {
        private Slider slider;
        private RectTransform rectTransform;
        
        [Header("Bar Option")] 
        [SerializeField] protected bool scaleBarLengthWithStatus = true;
        [SerializeField] protected float widthScaleMultiplier = 1;
        [SerializeField] protected float minWidthScale = 200;
        [SerializeField] protected float maxWidthScale = 800;
        [SerializeField] protected bool showBarValue = true;
        [SerializeField] private GameObject  valueHolder;
        [SerializeField] private TextMeshProUGUI  currentValueText;
        [SerializeField] private TextMeshProUGUI maxValueText;
        //Todo: secondary bar for effect (yellow bar to slow lost value)

        protected void Awake()
        {
            slider = GetComponent<Slider>();
            rectTransform = GetComponent<RectTransform>();
        }

        public virtual void SetStat(float newValue)
        {
            slider.value = newValue;

            if (currentValueText != null && maxValueText != null && showBarValue)
            {
                if (showBarValue)
                {
                    valueHolder.SetActive(true);
                    currentValueText.text = slider.value.ToString();
                }
                else
                {
                    valueHolder.SetActive(false);
                }
            }
                
        }
        
        public virtual void SetMaxStat(int maxValue)
        {
            slider.maxValue = maxValue;

            if (currentValueText != null && maxValueText != null)
            {
                if (showBarValue)
                {
                    valueHolder.SetActive(true);
                    maxValueText.text = slider.maxValue.ToString();
                    currentValueText.text = slider.maxValue.ToString();
                }
                else
                {
                    valueHolder.SetActive(false);
                }
                
            }
            
            //Max value change, set current value to max
            slider.value = maxValue;

            if (scaleBarLengthWithStatus)
            {
                float newWidth = 
                (
                    maxWidthScale < (maxValue * widthScaleMultiplier) 
                    ? 
                    maxWidthScale 
                    : 
                    (
                        minWidthScale > (maxValue * widthScaleMultiplier) 
                        ? 
                        minWidthScale
                        : 
                        (maxValue * widthScaleMultiplier)
                    )
                );
                rectTransform.sizeDelta = new Vector2(newWidth, rectTransform.sizeDelta.y);
                
                //Reset position of the bar base on layout group setting
                PlayerUIManager.instance.playerUIHudManager.RefreshHUD();
            }
        }
    }
}
