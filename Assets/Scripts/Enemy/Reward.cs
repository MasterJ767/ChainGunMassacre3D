using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Enemy
{
    public class Reward : MonoBehaviour
    {
        public int minimumQuantity;
        public int maximumQuantity;
        [Range(0f, 1f)]
        public float chancePerRole;
        public WeightedReward[] rewards;

        public void DropItem()
        {
            int difference = maximumQuantity - minimumQuantity;
            int quantity = minimumQuantity;

            for (int i = 0; i < difference; i++)
            {
                if (Random.value <= chancePerRole)
                {
                    quantity++;
                }
            }

            if (quantity == 0)
            {
                return;
            }

            List<WeightedReward> rewardsToDrop = new List<WeightedReward>();

            while (rewardsToDrop.Count < quantity)
            {
                rewardsToDrop.Add(rewards[EvaluateItems()]);
            }

            if (rewardsToDrop.Count == 1)
            {
                GameObject item = Instantiate(rewards[0].prefab, transform.position, Quaternion.identity);
                Destroy(item, 20f);
            }
            else
            {
                Vector3 centre = transform.position;
                float angle = (2 * Mathf.PI / rewardsToDrop.Count);

                for (int i = 0; i < rewardsToDrop.Count; i++)
                {
                    Vector3 direction = new Vector3(Mathf.Cos(i * angle), 1f, Mathf.Sin(i * angle));
                    Vector3 spawnPoint = centre + (direction * 0.6f);

                    GameObject item = Instantiate(rewardsToDrop[i].prefab, spawnPoint, Quaternion.identity);
                    Destroy(item, 20f);
                }
            }
        }

        public int EvaluateItems()
        {
            float weightSum = 0f;
            for (int i = 0; i < rewards.Length; i++)
            {
                weightSum += rewards[i].weight;
            }
        
            float r = Random.value;
            float s = 0f;
        
            for (int i = 0; i < rewards.Length; i++)
            {
                s += rewards[i].weight / weightSum;
                if (s >= r)
                {
                    return i;
                }
            }

            return 0;
        }
    }

    [Serializable]
    public struct WeightedReward
    {
        public GameObject prefab;
        public float weight;
    }
}
