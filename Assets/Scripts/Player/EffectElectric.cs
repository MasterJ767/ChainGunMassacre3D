using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Player
{
    public class EffectElectric : MonoBehaviour
    {
        public GameObject electricLine;
        public LayerMask enemyLayer;
        
        public void StartElectricAttack(Collider enemy, ElectricParameters parameters, Weapon weapon)
        {
            StartCoroutine(ElectricAttack(enemy, parameters, weapon, parameters.repeat));
        }

        private IEnumerator ElectricAttack(Collider enemy, ElectricParameters parameters, Weapon weapon, int repeats)
        {
            Vector3 startPosition = enemy.transform.position;
            startPosition.y += enemy.bounds.extents.y;

            Collider[] allTargets = Physics.OverlapSphere(startPosition, parameters.range, enemyLayer);
            List<GameObject> validTargets = new List<GameObject>();

            foreach (Collider target in allTargets)
            {
                if (target.gameObject == enemy.gameObject)
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
                    
                validTargets.Add(target.gameObject);
            }

            if (validTargets.Count == 0)
            {
                Destroy(gameObject);
            }

            int targetCount = Mathf.Min(parameters.targets, validTargets.Count);
            while (validTargets.Count > targetCount)
            {
                validTargets.RemoveAt(Random.Range(0, validTargets.Count - 1));
            }

            LineRenderer[] lineRenderers = new LineRenderer[targetCount];
            for (int i = 0; i < lineRenderers.Length; i++)
            {
                GameObject lineGameObject = Instantiate(electricLine, startPosition, Quaternion.identity, transform);
                lineRenderers[i] = lineGameObject.GetComponent<LineRenderer>();
                lineRenderers[i].SetPosition(0, startPosition);
            }

            int steps = 5;

            for (int i = 1; i < steps; i++)
            {
                for (int j = 0; j < targetCount; j++)
                {
                    lineRenderers[j].positionCount++;
                    
                    if (validTargets[j] == null)
                    {
                        continue;
                    }
                    
                    if (i == steps - 1)
                    {
                        Vector3 targetPosition = validTargets[j].transform.position + new Vector3(0, validTargets[j].GetComponent<Collider>().bounds.extents.y, 0);
                        lineRenderers[j].SetPosition(i, targetPosition);
                    }
                    else
                    {
                        Vector3 lastPosition = lineRenderers[j].GetPosition(i-1);
                        Vector3 lineDirection = (validTargets[j].transform.position - lastPosition);
                        Vector3 nextPosition = lastPosition + (lineDirection / (steps - i));
                        nextPosition += new Vector3(lineDirection.y, -lineDirection.z, lineDirection.x) * Random.Range(0f, 0.125f);
                        nextPosition += new Vector3(-lineDirection.z, lineDirection.x, -lineDirection.y) * Random.Range(0f, 0.125f);
                        nextPosition += new Vector3(-lineDirection.y, lineDirection.z, -lineDirection.x) * Random.Range(0f, 0.125f);
                        nextPosition += new Vector3(lineDirection.z, -lineDirection.x, lineDirection.y) * Random.Range(0f, 0.125f);
                        lineRenderers[j].SetPosition(i, nextPosition);
                    }
                }

                yield return new WaitForSeconds(0.02f);
            }

            for (int i = 0; i < validTargets.Count; i++)
            {
                if (validTargets[i] == null)
                {
                    continue;
                }
                
                Resource.Health targetHealth = validTargets[i].GetComponent<Resource.Health>();
                
                if (targetHealth == null)
                {
                    continue;
                }
                
                targetHealth.Damage(parameters.damage, parameters.bulletColour);
            }

            yield return new WaitForSeconds(parameters.delay);

            repeats--;

            if (repeats >= 0)
            {
                GameObject nextChain = validTargets[Random.Range(0, validTargets.Count)];
                if (nextChain)
                {
                    StartCoroutine(ElectricAttack(nextChain.GetComponent<Collider>(), parameters, weapon, repeats));
                }
                else
                {
                    Destroy(gameObject);
                }
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
