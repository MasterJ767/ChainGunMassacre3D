using System;
using System.Drawing;
using UnityEngine;
using UnityEngine.UI;

namespace Resource
{
    public class Stamina : MonoBehaviour
    {
        public float maxStamina;
        [HideInInspector]
        public float currentStamina;

        public float regenPerSecond;
        public float regenDelay;

        private float timeSinceUse;

        public bool hasStaminaBar;
        public Slider slider;
        public Image staminaFill;
        public Gradient staminaColour;

        private void Start()
        {
            SetStaminaMax();
        }
        
        private void Update()
        {
            timeSinceUse += Time.deltaTime;
            if (currentStamina < maxStamina && timeSinceUse > regenDelay)
            {
                Recover(regenPerSecond * Time.deltaTime);
            }
        }

        private void SetStaminaMax()
        {
            if (hasStaminaBar)
            {
                slider.maxValue = maxStamina;
            }
            currentStamina = maxStamina;
            SetStaminaSlider();
        }

        private void SetStaminaSlider()
        {
            if (hasStaminaBar)
            {
                slider.value = currentStamina;
                staminaFill.color = staminaColour.Evaluate(slider.normalizedValue);
            }
        }

        public bool Expend(float value)
        {
            float staminaDifference = currentStamina - value;
            if (staminaDifference < 0)
            {
                return false;
            }
            
            timeSinceUse = 0;
            currentStamina = staminaDifference;
            SetStaminaSlider();
            return true;
        }

        public void Recover(float value)
        {
            currentStamina = Mathf.Min(maxStamina, currentStamina + value);
            SetStaminaSlider();
        }
    }
}
