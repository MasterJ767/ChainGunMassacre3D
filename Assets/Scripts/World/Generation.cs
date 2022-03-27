using System;
using System.Collections.Generic;
using Packages.Rider.Editor.UnitTesting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace World
{
    public class Generation : MonoBehaviour
    {
        public Transform player;
        private Chunk currentChunk;
        private Chunk lastChunk;

        public GameObject chunkPrefab;
        public Material material;

        public Biome[] biomes;

        private Dictionary<Vector2Int, ChunkData> chunks = new Dictionary<Vector2Int, ChunkData>();

        public void Start()
        {
            Initialise();
            SpawnPlayer();
        }

        public void Update()
        {
            DeterminePlayer();
            DeactivateChunks();
        }
        
        private void Initialise()
        {
            for (int x = -Config.GenerationDistance; x < Config.GenerationDistance; x++)
            {
                for (int z = -Config.GenerationDistance; z < Config.GenerationDistance; z++)
                {
                    int scaledX = x * Config.ChunkWidth;
                    int scaledZ = z * Config.ChunkWidth;

                    GameObject chunkObject = Instantiate(chunkPrefab, new Vector3(scaledX, 0, scaledZ), Quaternion.identity, transform);
                    Chunk chunk = chunkObject.GetComponent<Chunk>();
                    chunk.Initialise(this, scaledX, scaledZ, material);
                    ChunkData chunkData = new ChunkData()
                    {
                        chunk = chunk,
                        active = true,
                    };
                    chunks.Add(new Vector2Int(scaledX, scaledZ), chunkData);

                    if (x >= -Config.ViewDistance && x < Config.ViewDistance && z >= -Config.ViewDistance && z < Config.ViewDistance)
                    {
                        chunk.Render();
                    }
                }
            }
        }

        private void SpawnPlayer()
        {
            player.position = new Vector3(0, 3, 0);
            currentChunk = chunks[new Vector2Int(0, 0)].chunk;
            lastChunk = currentChunk;
        }

        private void DeterminePlayer()
        {
            lastChunk = currentChunk;
            Vector3 position = player.position;
            Vector2Int chunkPosition = new Vector2Int(Mathf.FloorToInt(position.x / Config.ChunkWidth) * Config.ChunkWidth, Mathf.FloorToInt(position.z / Config.ChunkWidth) * Config.ChunkWidth);
            ChunkData chunkData = chunks[chunkPosition];
            currentChunk = chunkData.chunk;

            if (currentChunk != lastChunk)
            {
                GenerateAroundPlayer(position);
            }
        }

        private void GenerateAroundPlayer(Vector3 position)
        {
            int xMid = Mathf.FloorToInt(position.x / Config.ChunkWidth);
            int zMid = Mathf.FloorToInt(position.z / Config.ChunkWidth);

            for (int x = xMid - Config.GenerationDistance; x < xMid + Config.GenerationDistance; x++)
            {
                for (int z = zMid - Config.GenerationDistance; z < zMid + Config.GenerationDistance; z++)
                {
                    int scaledX = x * Config.ChunkWidth;
                    int scaledZ = z * Config.ChunkWidth;

                    if (!chunks.ContainsKey(new Vector2Int(scaledX, scaledZ)))
                    {
                        GameObject chunkObject = Instantiate(chunkPrefab, new Vector3(scaledX, 0, scaledZ), Quaternion.identity, transform);
                        Chunk chunk = chunkObject.GetComponent<Chunk>();
                        chunk.Initialise(this, scaledX, scaledZ, material);
                        ChunkData chunkData = new ChunkData()
                        {
                            chunk = chunk,
                            active = true,
                        };
                        chunks.Add(new Vector2Int(scaledX, scaledZ), chunkData);
                    }
                    
                    ChunkData chunkDataCheck = chunks[new Vector2Int(scaledX, scaledZ)];
                    chunkDataCheck.active = true;
                    chunkDataCheck.chunk.gameObject.SetActive(true);
                }
            }

            RenderAroundPlayer(xMid, zMid);
        }

        private void RenderAroundPlayer(int xMid, int zMid)
        {
            for (int x = xMid - Config.ViewDistance; x < xMid + Config.ViewDistance; x++)
            {
                for (int z = zMid - Config.ViewDistance; z < zMid + Config.ViewDistance; z++)
                {
                    int scaledX = x * Config.ChunkWidth;
                    int scaledZ = z * Config.ChunkWidth;
                    
                    ChunkData chunkData = chunks[new Vector2Int(scaledX, scaledZ)];
                    if (!chunkData.chunk.isRendered)
                    {
                        chunkData.chunk.Render();
                    }
                }
            }
        }

        private void DeactivateChunks()
        {
            foreach (Vector2Int key in chunks.Keys)
            {
                if (Vector2.Distance(key, currentChunk.position) > Config.GenerationDistance * Config.ChunkWidth)
                {
                    ChunkData chunkData = chunks[key];
                    chunkData.active = false;
                    chunkData.chunk.gameObject.SetActive(false);
                }
            }
        }

        public int GetBiomeId(int x, int z)
        {
            float distance = new Vector2(x, z).magnitude;

            for (int i = 0; i < biomes.Length; i++)
            {
                if (biomes[i].maxExtent > distance)
                {
                    return i;
                }
            }

            return -1;
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
        public Vector3 offset;
        public float weight;
    }

    [Serializable]
    public struct ChunkData
    {
        public Chunk chunk;
        public bool active;
    }
}