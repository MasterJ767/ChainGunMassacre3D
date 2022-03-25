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

        [Header("Player")]
        public Attack playerAttackController;
        public Camera playerCameraController;

        [HideInInspector]
        public float fireCooldown = 0f;
        
        private void Start()
        {
            Refill(maxAmmo);
            
            elementalParameters = new NoneParameters(new Color(1, 1, 0, 0f));
            elementalEffect = ElementalEffect.NONE;
            
            AddPoisonEffect();
        }

        private void Update()
        {
            fireCooldown -= Time.deltaTime;
            
            if (Input.GetButton("Fire1") && fireCooldown <= 0f)
            {
                if (Fire(1))
                {
                    StartCoroutine(Shoot());
                    fireCooldown = autoDelay;
                }
            }
        }

        public void SetAmmoText()
        {
            ammoText.text = currentAmmo.ToString("0");
        }

        public bool Fire(int value)
        {
            if (isMelee)
            {
                return true;
            }
            
            float ammoDifference = currentAmmo - value;
            if (ammoDifference < 0)
            {
                return false;
            }
            
            currentAmmo = Mathf.Max(0, currentAmmo - value);
            SetAmmoText();
            return true;
        }

        public void Refill(int value)
        {
            currentAmmo = Mathf.Min(maxAmmo, currentAmmo + value);
            SetAmmoText();
        }

        private IEnumerator Shoot()
        {
            Transform cameraTransform = playerCameraController.transform;
            Vector3 forward = cameraTransform.forward;
            Vector3 startPosition = attackPoint.position;

            for (int i = 0; i < burstCount; i++)
            {
                float spreadSeparation = (2 * Mathf.PI / spreadCount);
                float spreadAngleRad = spreadAngle * Mathf.Deg2Rad;
                Vector3 targetCentre = attackPoint.position + (forward * range);
                float offset = Random.Range(0f, 2 * Mathf.PI);

                for (int j = 0; j < spreadCount; j++)
                {
                    GameObject bulletGameObject = Instantiate(projectilePrefab, startPosition, cameraTransform.rotation, transform);

                    Vector3 spreadDirection = (Mathf.Cos((j * spreadSeparation) + offset) * cameraTransform.right) + (Mathf.Sin((j * spreadSeparation) + offset) * cameraTransform.up);
                    float spreadDistance = range * Mathf.Tan(spreadAngleRad);
                    Vector3 impactLocation;
                    Vector3 bulletDirection;

                    if (spreadType == SpreadType.RANDOM)
                    {
                        impactLocation = targetCentre + (spreadDirection.normalized * Random.Range(spreadDistance, spreadDistance / 2));
                        bulletDirection = (impactLocation - startPosition).normalized;
                    }
                    else if (spreadType == SpreadType.FIXED)
                    {
                        impactLocation = targetCentre + (spreadDirection.normalized * spreadDistance);
                        bulletDirection = (impactLocation - startPosition).normalized;
                    }
                    else
                    {
                        bulletDirection = forward;
                    }

                    bulletGameObject.GetComponent<Rigidbody>().velocity = bulletDirection * shotSpeed;
                    
                    Projectile bullet = bulletGameObject.GetComponent<Projectile>();
                    if (elementalEffect != ElementalEffect.NONE && (Random.Range(0f, 1f) <= (1f / elementalFrequency) || elementalFrequency <= 0))
                    {
                        bullet.Initialise(playerAttackController.GetCurrentWeapon(), elementalParameters, elementalEffect, damage, knockback, pierceCount);
                    }
                    else
                    {
                        NoneParameters noParameters = new NoneParameters(new Color(1, 1, 0, 0f));
                        bullet.Initialise(playerAttackController.GetCurrentWeapon(), noParameters, ElementalEffect.NONE, damage, knockback, pierceCount);
                    }
                    
                    Destroy(bulletGameObject, range / shotSpeed);
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
                elementalParameters = new FireParameters(new Color(1, 0.25f, 0, 0.5f), 0.1f, 3f, 0.2f, 3);
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
                elementalParameters = new PoisonParameters(new Color(0, 1, 0, 0.5f), 0.2f, 3f, 0.2f,1f, 7f);
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

    public enum DamageType
    {
        NONE,
        WEAPON,
        ELECTRIC,
        FIRE,
        POISON,
        EARTH,
        AIR,
        ENEMY
    }
}
