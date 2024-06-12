using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace AN
{
    public class QuickSlotItem : MonoBehaviour
    {
        [SerializeField] public Image backGroundImage;
        [SerializeField] public Image itemIcon;
        [SerializeField] public bool isCooldown = false;
        [SerializeField] protected GameObject cooldownObject;
        [SerializeField] protected float currentCooldownTime = 0;
        [SerializeField] protected Slider cooldownSlider;
        [SerializeField] protected TextMeshProUGUI cooldownText;

        private void Awake() {
            cooldownSlider = cooldownObject.GetComponentInChildren<Slider>();
            cooldownText = cooldownObject.GetComponentInChildren<TextMeshProUGUI>();
            cooldownObject.SetActive(false);
        }

        private void Update() {
            CoolingDown();
        }

        public virtual void StartCooldown(float characterWeaponSwitchCooldownTime)
        {
            cooldownObject.SetActive(true);
            isCooldown = true;
            cooldownSlider.maxValue = characterWeaponSwitchCooldownTime;
            cooldownSlider.value = characterWeaponSwitchCooldownTime;
            currentCooldownTime = characterWeaponSwitchCooldownTime;
        }

        public void CoolingDown()
        {
            if(currentCooldownTime > 0)
            {
                currentCooldownTime -= Time.deltaTime;
                cooldownText.text = Mathf.RoundToInt(currentCooldownTime) + "";
                cooldownSlider.value = currentCooldownTime;
            }
            else
            {
                cooldownObject.SetActive(false);
                isCooldown = false;
                currentCooldownTime = 0;
            }
        }
    }
}
