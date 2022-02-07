using System;
using UnityEngine;
using UnityEngine.UI;

namespace Resource
{
    public class Health : MonoBehaviour
    {
        public float maxHealth;
        [HideInInspector] 
        public float currentHealth;

        public float regenPerSecond;
        public float regenDelay;

        private float timeSinceHit;

        public bool hasHealthBar;
        public Slider slider;
        public Image healthFill;
        public Gradient healthColour;

        private bool isDead = false;

        private void Start()
        {
            SetHealthMax();
        }

        private void Update()
        {
            timeSinceHit += Time.deltaTime;
            if (currentHealth < maxHealth && timeSinceHit > regenDelay)
            {
                Heal(regenPerSecond * Time.deltaTime);
            }
        }

        private void SetHealthMax()
        {
            if (hasHealthBar)
            {
                slider.maxValue = maxHealth;
            }
            currentHealth = maxHealth;
            SetHealthSlider();
        }

        private void SetHealthSlider()
        {
            if (hasHealthBar)
            {
                slider.value = currentHealth;
                healthFill.color = healthColour.Evaluate(slider.normalizedValue);
            }
        }

        public void Damage(float value)
        {
            timeSinceHit = 0;
            currentHealth = Mathf.Max(0, currentHealth - value);
            SetHealthSlider();
            if (currentHealth <= 0 && !isDead)
            {
                Death();
            }
        }

        public void Heal(float value)
        {
            currentHealth = Mathf.Min(maxHealth, currentHealth + value);
            SetHealthSlider();
        }

        private void Death()
        {
            isDead = true;
        }
    }
}
