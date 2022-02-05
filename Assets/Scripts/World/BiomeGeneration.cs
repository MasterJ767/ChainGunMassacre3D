using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace World
{
    public class BiomeGeneration : MonoBehaviour
    {
        public Transform player;

        public GameObject chunkPrefab;
        public Material material;

        public Biome[] biomes;

        private List<Chunk> activeChunks;
        private List<Chunk> unactiveChunks;

        public void Start()
        {
            Initialise();
        }

        private void Initialise()
        {
            for (int x = -Config.ViewDistance; x <= Config.ViewDistance; x++)
            {
                for (int z = -Config.ViewDistance; z <= Config.ViewDistance; z++)
                {
                    int scaledX = x * Config.ChunkWidth;
                    int scaledZ = z * Config.ChunkWidth;

                    GameObject chunk = Instantiate(chunkPrefab, new Vector3(scaledX, 0, scaledZ), Quaternion.identity, transform);
                    chunk.GetComponent<Chunk>().Initialise(this, scaledX, scaledZ, material);
                }
            }
        }

        public int GetBiomeId(int x, int z)
        {
            int biomeId = -1;
            float distance = new Vector2(x, z).magnitude;

            for (int i = 0; i < biomes.Length; i++)
            {
                if (biomes[i].maxExtent > distance)
                {
                    biomeId = i;
                }
            }

            if (biomeId == -1)
            {
                biomeId = biomes.Length - 1;
            }
            
            return biomeId;
        }

        public int GetFeature(int x, int z, int biomeId)
        {
            float weightSum = biomes[biomeId].blankFeatureWeight;
            for (int i = 0; i < biomes[biomeId].features.Length; i++)
            {
                weightSum += biomes[biomeId].features[i].weight;
            }

            Random.InitState(new Vector3(x + 1000, z - 1000).GetHashCode() * x ^ z / 2);
            float r = Random.value;
            float s = 0f;

            for (int j = 0; j < biomes[biomeId].features.Length; j++)
            {
                s += biomes[biomeId].features[j].weight / weightSum;
                if (s >= r)
                {
                    return j;
                }
            }

            return -1;
        }
    }
}

[Serializable]
public struct Biome
{
    public string biomeName;
    public float blankFeatureWeight;
    public WeightedSpawn[] features;
    public WeightedSpawn[] enemies;
    public WeightedSpawn[] bossEnemies;
    public float maxExtent;
}

[Serializable]
public struct WeightedSpawn
{
    public GameObject prefab;
    public float weight;
}