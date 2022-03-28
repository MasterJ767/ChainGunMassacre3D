using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace World
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class Chunk : MonoBehaviour
    {
        public MeshFilter meshFilter;
        public MeshRenderer meshRenderer;
        public BoxCollider boxCollider;

        [HideInInspector]
        public bool isRendered = false;
        
        private Generation world;
        [HideInInspector]
        public Vector2Int position;
        private Material material;
        private int[,] biomeMap = new int[Config.ChunkWidth, Config.ChunkWidth];

        public void Initialise(Generation World, int X, int Z, Material Material)
        {
            world = World;
            position = new Vector2Int(X, Z);
            material = Material;

            Populate();

            boxCollider.center = new Vector3(Config.ChunkWidth / 2f, 0, Config.ChunkWidth / 2f);
            boxCollider.size = new Vector3(Config.ChunkWidth, 0, Config.ChunkWidth);
        }

        public void Populate()
        {
            for (int x = 0; x < Config.ChunkWidth; x++)
            {
                for (int z = 0; z < Config.ChunkWidth; z++)
                {
                    biomeMap[x, z] = world.GetBiomeId(x + position.x, z + position.y);
                }
            }
        }

        public void Render()
        {
            meshRenderer.material = material;
            
            int biomes = world.biomes.Length + 1;
            
            int index = 0;
            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();
            List<Vector2> uvs = new List<Vector2>();
            
            for (int x = 0; x < Config.ChunkWidth; x++)
            {
                for (int z = 0; z < Config.ChunkWidth; z++)
                {
                    vertices.Add(new Vector3(x, 0, z));
                    vertices.Add(new Vector3(x, 0, z+1));
                    vertices.Add(new Vector3(x+1, 0, z+1));
                    vertices.Add(new Vector3(x+1, 0, z));
                    
                    triangles.Add(index);
                    triangles.Add(index + 1);
                    triangles.Add(index + 2);
                    triangles.Add(index);
                    triangles.Add(index + 2);
                    triangles.Add(index + 3);

                    int biomeId = biomeMap[x, z];

                    float topY = (biomes - biomeId) / (float) biomes;
                    float bottomY = topY - (1 / (float)biomes);

                    if (biomeId == -1)
                    {
                        uvs.Add(new Vector2(0, 0));
                        uvs.Add(new Vector2(0, 1 / (float) biomes));
                        uvs.Add(new Vector2(1, 1 / (float) biomes));
                        uvs.Add(new Vector2(1, 0));
                    }
                    else
                    {
                        uvs.Add(new Vector2(0, bottomY));
                        uvs.Add(new Vector2(0, topY));
                        uvs.Add(new Vector2(1, topY));
                        uvs.Add(new Vector2(1, bottomY));
                    }

                    index += 4;
                }
            }

            Mesh mesh = new Mesh();
            mesh.SetVertices(vertices.ToArray());
            mesh.SetTriangles(triangles.ToArray(), 0);
            mesh.SetUVs(0, uvs.ToArray());

            meshFilter.sharedMesh = mesh;

            Decorate();

            isRendered = true;
        }

        private void Decorate()
        {
            for (int x = 0; x < Config.ChunkWidth; x++)
            {
                for (int z = 0; z < Config.ChunkWidth; z++)
                {
                    Vector3Int globalPosition = new Vector3Int(x + position.x, 0, z + position.y);
                    if (globalPosition.magnitude >= 20)
                    {
                        int biomeId = biomeMap[x, z];
                        int featureId = world.GetFeature(globalPosition.x, globalPosition.z, biomeId);
                        if (featureId >= 0)
                        {
                            Quaternion featureRotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
                            Vector3 featurePosition = globalPosition + world.biomes[biomeId].features[featureId].offset;
                            GameObject featureGO = Instantiate(world.biomes[biomeId].features[featureId].prefab, featurePosition, featureRotation, transform);
                            featureGO.transform.localScale = Vector3.one * Random.Range(0.75f, 1.5f);
                        }
                    }
                }
            }
        }
    }
}
