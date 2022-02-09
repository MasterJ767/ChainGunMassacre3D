using System;
using System.Collections;
using TMPro;
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
        public TextMeshProUGUI healthText;
        public Gradient healthColour;

        public bool hasDamageIndicator;
        public Canvas damageIndicator;
        public TextMeshProUGUI damageIndicatorText;

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
                healthText.text = Mathf.Floor(currentHealth) + " / " + maxHealth;
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
        
        public void Damage(float value, Color colour)
        {
            if (hasDamageIndicator)
            {
                StartCoroutine(IndicateDamage(value, colour));
            }
            Damage(value);
        }

        public void Heal(float value)
        {
            currentHealth = Mathf.Min(maxHealth, currentHealth + value);
            SetHealthSlider();
        }

        private void Death()
        {
            isDead = true;

            if (gameObject.CompareTag("Enemy"))
            {
                EnemyDeath();
            }
            else if (gameObject.CompareTag("Player"))
            {
                PlayerDeath();
            }
        }

        private void EnemyDeath()
        {
            gameObject.GetComponent<Enemy.Reward>().DropItem();
            Destroy(gameObject);
        }

        private void PlayerDeath()
        {
            Time.timeScale = 0;
            Debug.Log("YOU DIED!");
        }

        private IEnumerator IndicateDamage(float damage, Color colour)
        {
            damageIndicatorText.text = damage.ToString("0.0");
            damageIndicatorText.color = colour;
            damageIndicator.gameObject.SetActive(true);
            yield return new WaitForSeconds(1f);
            damageIndicator.gameObject.SetActive(false);
        }
    }
}
