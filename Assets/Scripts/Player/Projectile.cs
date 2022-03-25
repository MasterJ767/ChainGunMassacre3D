using System;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class Projectile : MonoBehaviour
    {
        public GameObject projectileCoating;
        public GameObject electricEffectPrefab;
        
        private Weapon weapon;

        private ElementalParameters elementalParameters;
        private ElementalEffect elementalEffect;
        private float damage;
        private float knockback;
        private int pierceCount;

        private Rigidbody rb;
        private List<GameObject> hitEnemies = new List<GameObject>();

        private void Start()
        {
            rb = GetComponent<Rigidbody>();
        }

        public void Initialise(Weapon Weapon, ElementalParameters ElementalParameters, ElementalEffect ElementalEffect, float Damage, float Knockback, int PierceCount)
        {
            weapon = Weapon;
            
            elementalParameters = ElementalParameters;
            elementalEffect = ElementalEffect;
            damage = Damage;
            knockback = Knockback;
            pierceCount = PierceCount;

            projectileCoating.GetComponent<MeshRenderer>().material.color = elementalParameters.bulletColour;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Enemy"))
            {
                hitEnemies.Add(other.gameObject);

                Rigidbody enemyRigidbody = other.gameObject.GetComponent<Rigidbody>();
                enemyRigidbody.AddForce(rb.velocity.normalized * knockback, ForceMode.Impulse);

                Resource.Health enemyHealth = other.gameObject.GetComponent<Resource.Health>();
                enemyHealth.Damage(damage, elementalParameters.bulletColour);
                
                Enemy.Effects enemyEffects = other.gameObject.GetComponent<Enemy.Effects>();

                if (other.gameObject != null && !enemyHealth.isDead)
                {
                    switch (elementalEffect)
                    {
                        case ElementalEffect.NONE:
                            break;
                        case ElementalEffect.ELECTRIC:
                            GameObject electricEffectGameObject = Instantiate(electricEffectPrefab,other.transform.position, Quaternion.identity);
                            electricEffectGameObject.GetComponent<EffectElectric>().StartElectricAttack(other, (ElectricParameters)elementalParameters, weapon);
                            break;
                        case ElementalEffect.FIRE:
                            enemyEffects.StartFireAttack((FireParameters)elementalParameters, weapon);
                            break;
                        case ElementalEffect.ICE:
                            enemyEffects.StartIceAttack((IceParameters)elementalParameters, weapon);
                            break;
                        case ElementalEffect.POISON:
                            //
                            break;
                        case ElementalEffect.EARTH:
                            //
                            break;
                        case ElementalEffect.AIR:
                            //
                            break;
                    }
                }

                if (pierceCount <= 0)
                {
                    Destroy(gameObject);
                }
                else
                {
                    pierceCount--;
                }
            }
            else if (other.gameObject.CompareTag("Ground"))
            {
                Destroy(gameObject);
            }
        }
    }
}
