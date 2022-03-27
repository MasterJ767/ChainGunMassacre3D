using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Manager
{
    public class SpawnManager : MonoBehaviour
    {
        public World.Generation world;
        public Transform player;
        
        public int wave = 0;
        public int difficulty = 1;
        public float incrementTime = 5f;
        private int enemyCount = 0;
        private int enemyBossCount = 0;

        private float minimumSpawnDistance = 17f;
        private float additionalSpawnDistance = 10f;

        [NonSerialized] 
        public Dictionary<GameObject, int> spawnedEnemies;
        [NonSerialized] 
        public Dictionary<GameObject, int> spawnedBossEnemies;

        private static SpawnManager instance;

        public static SpawnManager GetInstance()
        {
            if (instance == null)
            {
                instance = FindObjectOfType<SpawnManager>();
                if (instance == null)
                {
                    Debug.LogWarning("NO EFFECT MANAGER IN SCENE");
                }
            }

            return instance;
        }

        private void Start()
        {
            spawnedEnemies = new Dictionary<GameObject, int>();
            spawnedBossEnemies = new Dictionary<GameObject, int>();

            StartCoroutine(Increment());
        }

        private void Update()
        {
            SpawnEnemies();
            CheckDistance();
        }

        private int GetBiomeId(Vector3 position)
        {
            return world.GetBiomeId(Mathf.FloorToInt(position.x), Mathf.FloorToInt(position.z));
        }

        private IEnumerator Increment()
        {
            while (Time.timeScale > 0)
            {
                enemyCount = DetermineEnemyCount();
                enemyBossCount = (difficulty - 1);

                yield return new WaitForSeconds(incrementTime);

                wave++;

                if (wave % 8 == 0)
                {
                    difficulty++;
                }
            }
        }

        private int DetermineEnemyCount()
        {
            int n = difficulty;
            int d = (2 * difficulty) - 1;
            return Mathf.Abs(Mathf.FloorToInt(n * Mathf.Log((d * Mathf.Pow(wave, 2f)) + 0.4f)) + 12) + (GetBiomeId(player.position) * difficulty);
        }

        private void SpawnEnemies()
        {
            if (spawnedEnemies.Count < enemyCount)
            {
                SpawnRegular();
            }

            if (spawnedBossEnemies.Count < enemyBossCount)
            {
                SpawnBoss();
            }
        }

        private void SpawnRegular()
        {
            Vector3 centre = player.position;
            float angle = ((Mathf.PI / 2) / enemyCount) + (Random.Range(-1, 1) * (Mathf.PI / 2) * Random.value);
            float offset = (Mathf.PI / 2) * Random.value;

            int biomeId = GetBiomeId(centre);

            for (int i = (spawnedEnemies.Count - 1); i < enemyCount; i++)
            {
                int enemyId = EvaluateEnemies(biomeId, false);
                Spawn(i, angle, offset, centre, spawnedEnemies, biomeId, enemyId, false);
            }
        }

        private void SpawnBoss()
        {
            Vector3 centre = player.position;
            float angle = ((Mathf.PI / 2) / enemyCount) + (Random.Range(-1, 1) * (Mathf.PI / 2) * Random.value);
            float offset = (Mathf.PI / 2) * Random.value;

            int biomeId = world.GetBiomeId(Mathf.FloorToInt(centre.x), Mathf.FloorToInt(centre.z));
            
            for (int i = (spawnedBossEnemies.Count - 1); i < enemyBossCount; i++)
            {
                int enemyId = EvaluateEnemies(biomeId, true);
                Spawn(i, angle, offset, centre, spawnedBossEnemies, biomeId, enemyId, true);
            }
        }

        private int EvaluateEnemies(int biomeId, bool boss)
        {
            float weightSum = 0f;
            if (boss)
            {
                for (int i = 0; i < world.biomes[biomeId].bossEnemies.Length; i++)
                {
                    weightSum += world.biomes[biomeId].bossEnemies[i].weight;
                }
            }
            else
            {
                for (int i = 0; i < world.biomes[biomeId].enemies.Length; i++)
                {
                    weightSum += world.biomes[biomeId].enemies[i].weight;
                }
            }
            
            float r = Random.value;
            float s = 0f;

            if (boss)
            {
                for (int i = 0; i < world.biomes[biomeId].bossEnemies.Length; i++)
                {
                    s += world.biomes[biomeId].bossEnemies[i].weight / weightSum;
                    if (s >= r)
                    {
                        return i;
                    }
                }
            }
            else
            {
                for (int i = 0; i < world.biomes[biomeId].enemies.Length; i++)
                {
                    s += world.biomes[biomeId].enemies[i].weight / weightSum;
                    if (s >= r)
                    {
                        return i;
                    }
                }
            }

            return 0;
        }

        private float ClampAngle(int i, float angle, float offset)
        {
            angle += (Mathf.PI / 4);
            
            float upper = (3f / 4f) * Mathf.PI;
            float lower = (1f / 4f) * Mathf.PI;

            float extra = 0;
            
            float finalAngle = (i * angle) + offset;
            if (finalAngle > upper)
            {
                extra = finalAngle - upper;
                finalAngle = lower + extra;
            }
            else if (finalAngle < lower)
            {
                extra = lower - finalAngle;
                finalAngle = upper - extra;
            }

            return Mathf.Clamp(finalAngle, lower, upper);
        }

        private void Spawn(int i, float angle, float offset, Vector3 centre, Dictionary<GameObject, int> list, int biomeId, int enemyId, bool boss)
        {
            float finalAngle = ClampAngle(i, angle, offset);

            Vector3 direction = new Vector3(Mathf.Cos(finalAngle), 0, Mathf.Sin(finalAngle));
            Vector3 spawnPoint = centre + (direction * (minimumSpawnDistance + (additionalSpawnDistance * Random.value)));
            
            GameObject enemy;
            
            if (boss)
            {
                spawnPoint += world.biomes[biomeId].bossEnemies[enemyId].offset;
                enemy = Instantiate(world.biomes[biomeId].bossEnemies[enemyId].prefab, spawnPoint, Quaternion.identity, transform);
            }
            else
            {
                spawnPoint += world.biomes[biomeId].enemies[enemyId].offset;
                enemy = Instantiate(world.biomes[biomeId].enemies[enemyId].prefab, spawnPoint, Quaternion.identity, transform);
            }

            enemy.GetComponent<Enemy.Movement>().target = player;
            list.Add(enemy, enemyId);
        }

        private void CheckDistance()
        {
            Dictionary<GameObject, int> newEnemies = new Dictionary<GameObject, int>();
            List<GameObject> enemiesToDelete = new List<GameObject>();

            Vector3 centre = player.position;

            foreach (GameObject enemyObject in spawnedEnemies.Keys)
            {
                if (enemyObject == null)
                {
                    enemiesToDelete.Add(enemyObject);
                    continue;
                }

                Enemy.Movement enemyMovement = enemyObject.GetComponent<Enemy.Movement>();

                if (enemyMovement.distanceToTarget > (enemyMovement.viewDistance * 1.9f))
                {
                    float angle = ((Mathf.PI / 2) / enemyCount);
                    int n = Random.Range(0, enemyCount - 1);
                    
                    enemiesToDelete.Add(enemyObject);

                    int biomeId = GetBiomeId(centre);

                    int enemyId = EvaluateEnemies(biomeId, false);
                    Spawn(n, angle, 0, centre, newEnemies, biomeId, enemyId, false);
                }
            }
            
            for (int i = enemiesToDelete.Count - 1; i >= 0; i--)
            {
                GameObject enemy = enemiesToDelete[i];
                spawnedEnemies.Remove(enemy);       
                enemiesToDelete.Remove(enemy);
                Destroy(enemy);
            }
            
            spawnedEnemies.AddRange(newEnemies);
            
            foreach (GameObject enemyBossObject in spawnedBossEnemies.Keys)
            {
                if (enemyBossObject == null)
                {
                    enemiesToDelete.Add(enemyBossObject);
                    continue;
                }
                
                Enemy.Movement enemyBossMovement = enemyBossObject.GetComponent<Enemy.Movement>();

                if (enemyBossMovement.distanceToTarget > (enemyBossMovement.viewDistance * 1.9f))
                {
                    float angle = ((Mathf.PI / 2) / enemyBossCount);
                    int n = Random.Range(0, enemyBossCount - 1);
                    float offset = ((Mathf.PI / 2) * Random.value);
                    float finalAngle = ClampAngle(n, angle, offset);
                    Vector3 direction = new Vector3(Mathf.Cos(finalAngle), 0, Mathf.Sin(finalAngle));
                    enemyBossObject.transform.position = centre + (direction * (minimumSpawnDistance + (additionalSpawnDistance * Random.value)));
                }
            }
            
            for (int i = enemiesToDelete.Count - 1; i >= 0; i--)
            {
                GameObject enemy = enemiesToDelete[i];
                spawnedBossEnemies.Remove(enemy);       
                enemiesToDelete.Remove(enemy);
                Destroy(enemy);
            }
        }
        
        public void RemoveEnemy(GameObject enemy)
        {
            if (spawnedEnemies.ContainsKey(enemy))
            {
                spawnedEnemies.Remove(enemy);
            }
        }
        
        public void RemoveBossEnemy(GameObject enemy)
        {
            if (spawnedBossEnemies.ContainsKey(enemy))
            {
                spawnedBossEnemies.Remove(enemy);
            }
        }
    }
}
