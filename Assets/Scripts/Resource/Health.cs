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
            if (timeSinceHit > regenDelay)
            {
                Heal(regenPerSecond * Time.deltaTime);
            }
        }

        private void SetHealthMax()
        {
            slider.maxValue = maxHealth;
            currentHealth = maxHealth;
            SetHealthSlider();
        }

        private void SetHealthSlider()
        {
            slider.value = currentHealth;
            healthFill.color = healthColour.Evaluate(slider.normalizedValue);
        }

        public void TakeDamage(float value)
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

        public void Death()
        {
            isDead = true;
        }
    }
}
