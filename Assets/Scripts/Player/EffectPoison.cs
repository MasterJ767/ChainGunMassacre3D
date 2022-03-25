using System;
using UnityEngine;

namespace Player
{
    public class EffectPoison : MonoBehaviour
    {
        private PoisonParameters parameters;
        private Weapon weapon;

        public void Initialise(PoisonParameters Parameters, Weapon Weapon)
        {
            parameters = Parameters;
            weapon = Weapon;
            
            transform.localScale = Vector3.one * parameters.cloudSize;

            ParticleSystem cloud = GetComponent<ParticleSystem>();
            ParticleSystem.MainModule settings = cloud.main;
            settings.duration = parameters.cloudDuration;
            cloud.Play();
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Enemy"))
            {
                Resource.Health enemyHealth = other.gameObject.GetComponent<Resource.Health>();
                Enemy.Effects enemyEffects = other.gameObject.GetComponent<Enemy.Effects>();

                if (other.gameObject != null && !enemyHealth.isDead)
                {
                    enemyEffects.StartPoisonAttack(parameters, weapon);
                }
            }
        }
    }
}
