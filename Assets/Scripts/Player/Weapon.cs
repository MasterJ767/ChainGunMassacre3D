using System;
using System.Collections;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Player
{
    public class Weapon : MonoBehaviour
    {
        [Header("Basic Information")]
        public string weaponName;
        public Transform attackPoint;
        public GameObject projectilePrefab;
        public bool isMelee;
        
        [Header("Stats")]
        public float damage;
        public int maxAmmo;
        [HideInInspector]
        public int currentAmmo;
        public float shotSpeed;
        public float range;
        public float knockback;
        public int burstCount;
        public float burstDelay;
        public int pierceCount;
        public float autoDelay;
        public SpreadType spreadType;
        public int spreadCount;
        public float spreadAngle;
        public ElementalEffect elementalEffect;
        public ElementalParameters elementalParameters;
        public int elementalFrequency;
        
        [HideInInspector]
        public int killCount;
        [HideInInspector]
        public int level;

        [Header("UI")] 
        public TextMeshProUGUI ammoText;
        
        private float fireCooldown = 0f;

        private void Start()
        {
            SetAmmoText();
        }

        private void Update()
        {
            fireCooldown -= Time.deltaTime;
            
            if (Input.GetButtonDown("Fire1"))
            {
                if (fireCooldown <= 0f && currentAmmo > 0)
                {
                    StartCoroutine(Shoot());
                    fireCooldown -= autoDelay;
                }
            }
        }

        private void SetAmmoText()
        {
            ammoText.text = currentAmmo.ToString("0");
        }

        public void Refill(int value)
        {
            currentAmmo = Mathf.Min(maxAmmo, currentAmmo + value);
            SetAmmoText();
        }

        private IEnumerator Shoot()
        {
            currentAmmo--;
            
            for (int i = 0; i <= burstCount; i++)
            {
                Quaternion projectileRotation = Quaternion.AngleAxis(spreadAngle, Vector3.forward);

                Vector3 spreadStart = Quaternion.Euler(0, 0, spreadCount * spreadAngle / 2f) * Vector3.forward;

                for (int j = 0; j <= spreadCount; j++)
                {
                    GameObject bulletGameObject = Instantiate(projectilePrefab, attackPoint.position, projectileRotation, transform);
                    Destroy(bulletGameObject, range / shotSpeed);

                    Vector3 bulletDirection;

                    if (spreadType == SpreadType.RANDOM)
                    {
                        bulletDirection = Quaternion.Euler(0, 0, Random.Range(spreadAngle, -spreadAngle)) * Vector3.forward;
                    }
                    else if (spreadType == SpreadType.FIXED)
                    {
                        bulletDirection = Quaternion.Euler(0, 0, -spreadAngle * j) * spreadStart;
                    }
                    else
                    {
                        bulletDirection = Vector3.zero;
                    }

                    bulletGameObject.GetComponent<Rigidbody>().velocity = bulletDirection * shotSpeed;
                    
                    Projectile bullet = bulletGameObject.GetComponent<Projectile>();
                    if (Random.Range(0f, 1f) <= (1f / elementalFrequency))
                    {
                        bullet.Initialise(elementalParameters, elementalEffect, damage, knockback, pierceCount);
                    }
                    else
                    {
                        NoneParameters noParameters = new NoneParameters(new Color(1, 1, 0, 0.5f));
                        bullet.Initialise(noParameters, ElementalEffect.NONE, damage, knockback, pierceCount);
                    }
                    
                }
                
                yield return new WaitForSecondsRealtime(burstDelay);
            }
        }

        private void AddElectricEffect()
        {
            if (elementalEffect == ElementalEffect.NONE)
            {
                elementalEffect = ElementalEffect.ELECTRIC;
                elementalParameters = new ElectricParameters(new Color(0.25f, 0.5f, 1f, 0.5f), 2f, 2, 0, 0.1f, 3f);
            }
        }
        
        private void AddFireEffect()
        {
            if (elementalEffect == ElementalEffect.NONE)
            {
                elementalEffect = ElementalEffect.FIRE;
                elementalParameters = new FireParameters(new Color(1, 0.3f, 0, 0.5f), 1f, 3f, 3);
            }
        }
        
        private void AddIceEffect()
        {
            if (elementalEffect == ElementalEffect.NONE)
            {
                elementalEffect = ElementalEffect.ICE;
                elementalParameters = new IceParameters(new Color(0, 0.75f, 1f, 0.5f), 3f, 3);
            }
        }

        private void AddPoisonEffect()
        {
            if (elementalEffect == ElementalEffect.NONE)
            {
                elementalEffect = ElementalEffect.POISON;
                elementalParameters = new PoisonParameters(new Color(0, 1, 0, 0.5f), 2f, 3f, 1f, 7f);
            }
        }

        private void AddEarthEffect()
        {
            if (elementalEffect == ElementalEffect.NONE)
            {
                elementalEffect = ElementalEffect.EARTH;
                elementalParameters = new EarthParameters(new Color(0.5f, 0.25f, 0, 0.5f), 2f, 3f, 0, 0.6f, 3f);
            }
        }

        private void AddAirEffect()
        {
            if (elementalEffect == ElementalEffect.AIR)
            {
                elementalEffect = ElementalEffect.AIR;
                elementalParameters = new AirParameters(new Color(0.75f, 0.75f, 1, 0.5f), 1f, 2f, 0, 3f);
            }
        }
    }
    
    public enum ElementalEffect
    {
        NONE,
        ELECTRIC,
        FIRE,
        ICE,
        POISON,
        EARTH,
        AIR
    }

    public enum SpreadType
    {
        NONE,
        RANDOM,
        FIXED
    }
}
