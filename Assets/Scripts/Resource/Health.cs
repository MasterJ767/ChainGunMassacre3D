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

        public bool hasInstantiatedEffects;
        public Transform effectParent;
        public GameObject damageIndicatorPrefab;
        public GameObject poisonCloud;

        [NonSerialized]
        public bool isDead = false;

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

        public void Damage(float value, Player.DamageType damageType)
        {
            timeSinceHit = 0;
            currentHealth = Mathf.Max(0, currentHealth - value);
            SetHealthSlider();
            if (currentHealth <= 0 && !isDead)
            {
                Death(damageType);
            }
        }
        
        public void Damage(float value, Color colour, Player.DamageType damageType)
        {
            if (hasInstantiatedEffects)
            {
                IndicateDamage(value, colour);
            }
            Damage(value, damageType);
        }

        public float DamageQuery(float value)
        {
            return Mathf.Max(0, currentHealth - value);
        }

        public void Heal(float value)
        {
            currentHealth = Mathf.Min(maxHealth, currentHealth + value);
            SetHealthSlider();
        }

        public float HealQuery(float value)
        {
            return Mathf.Min(maxHealth, currentHealth + value);
        }

        private void Death(Player.DamageType damageType)
        {
            isDead = true;

            if (gameObject.CompareTag("Enemy"))
            {
                EnemyDeath(damageType);
            }
            else if (gameObject.CompareTag("Player"))
            {
                PlayerDeath();
            }
        }

        private void EnemyDeath(Player.DamageType damageType)
        {
            if (damageType == Player.DamageType.POISON && hasInstantiatedEffects)
            {
                Enemy.Effects effects = GetComponent<Enemy.Effects>();
                Enemy.EffectInformation effectInformation = effects.GetEffectInformation(Player.ElementalEffect.POISON);
                GameObject poisonCloudGO = Instantiate(poisonCloud, transform.position, Quaternion.identity, effectParent);
                poisonCloudGO.GetComponent<Player.EffectPoison>().Initialise((Player.PoisonParameters)effectInformation.parameters, effectInformation.weapon);
                Destroy(poisonCloudGO, ((Player.PoisonParameters)effectInformation.parameters).cloudDuration);
            }
            gameObject.GetComponent<Enemy.Reward>().DropItem();
            gameObject.SetActive(false);
            Destroy(gameObject, 2f);
        }

        private void PlayerDeath()
        {
            Time.timeScale = 0;
            Debug.Log("YOU DIED!");
        }

        private void IndicateDamage(float damage, Color colour)
        {
            Collider colldier = gameObject.GetComponent<Collider>();
            GameObject damageIndicator = Instantiate(damageIndicatorPrefab, transform.position + new Vector3(0, 2 * colldier.bounds.size.y, 0), transform.rotation, effectParent);
            TextMeshProUGUI damageIndicatorText = damageIndicator.GetComponentInChildren<TextMeshProUGUI>();
            damageIndicatorText.text = damage.ToString("0.0");
            damageIndicatorText.color = new Color(colour.r, colour.g, colour.b, 0.5f);
            Destroy(damageIndicator, 1f);
        }
    }
}
