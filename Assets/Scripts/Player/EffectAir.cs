using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class EffectAir : MonoBehaviour
    {
        public GameObject airEffectPrefab;
        public GameObject airWave;
        public LayerMask enemyLayer;
        
        public void StartAirAttack(Collider enemy, AirParameters parameters, Weapon weapon, int repeats)
        {
            StartCoroutine(AirAttack(enemy, parameters, weapon, repeats));
        }

        private IEnumerator AirAttack(Collider enemy, AirParameters parameters, Weapon weapon, int repeats)
        {
            Vector3 startPosition = enemy.transform.position;
            startPosition.y += enemy.bounds.extents.y;
            
            GameObject airWaveGO = Instantiate(airWave, startPosition, Quaternion.identity, enemy.gameObject.GetComponent<Resource.Health>().effectParent);
            airWaveGO.transform.localScale = Vector3.one * parameters.range * 2;
            airWaveGO.GetComponent<ParticleSystem>().Play();
            Destroy(airWaveGO, 2f);

            Collider[] allTargets = Physics.OverlapSphere(startPosition, parameters.range, enemyLayer);
            List<GameObject> validTargets = new List<GameObject>();

            foreach (Collider target in allTargets)
            {
                if (target.gameObject == enemy.gameObject)
                {
                    continue;
                }

                if (target.gameObject == null)
                {
                    continue;
                }

                if (validTargets.Contains(target.gameObject))
                {
                    continue;
                }

                if (target.gameObject.activeSelf == false)
                {
                    continue;
                }
                
                if (!target.gameObject.CompareTag("Enemy"))
                {
                    continue;
                }

                validTargets.Add(target.gameObject);
            }
            
            yield return new WaitForSeconds(0.02f);
            
            if (validTargets.Count == 0)
            {
                Destroy(gameObject);
            }

            List<GameObject> nextChain = new List<GameObject>();

            foreach (GameObject target in validTargets)
            {
                if (target.gameObject != null)
                {
                    Vector3 direction = target.transform.position - startPosition;
                    direction.y = 0;
                    
                    direction = direction.normalized * parameters.force;
                    
                    target.GetComponent<Enemy.Movement>().Knockback(direction * parameters.force);
                    Resource.Health targetHealth = target.GetComponent<Resource.Health>();
                    
                    if (repeats > 0 && targetHealth.DamageQuery(parameters.damage) == 0)
                    {
                        nextChain.Add(target);
                    }
                }
            }

            foreach (GameObject target in nextChain)
            {
                if (target.gameObject != null)
                {
                    GameObject airEffectGameObject = Instantiate(airEffectPrefab, target.transform.position, Quaternion.identity, target.GetComponent<Resource.Health>().effectParent);
                    airEffectGameObject.GetComponent<EffectAir>().StartAirAttack(target.GetComponent<Collider>(), parameters, weapon, repeats - 1);
                }
            }
            
            yield return new WaitForSeconds(0.02f);

            foreach (GameObject target in validTargets)
            {
                if (target.gameObject != null)
                {
                    Resource.Health targetHealth = target.GetComponent<Resource.Health>();
                    targetHealth.Damage(parameters.damage, parameters.bulletColour, DamageType.AIR);
                }
            }
            
            Destroy(gameObject);
        }
    }
}
