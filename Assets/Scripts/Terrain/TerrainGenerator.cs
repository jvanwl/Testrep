using UnityEngine;
using System.Collections.Generic;

public class TerrainGenerator : MonoBehaviour
{
    public static TerrainGenerator Instance { get; private set; }

    [Header("Terrain Settings")]
    [SerializeField] private int width = 256;
    [SerializeField] private int length = 256;
    [SerializeField] private int height = 20;
    [SerializeField] private float scale = 20f;
    
    [Header("Resource Generation")]
    [SerializeField] private float resourceDensity = 0.1f;
    [SerializeField] private GameObject[] resourcePrefabs;
    
    private Terrain terrain;
    private TerrainData terrainData;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        GenerateTerrain();
    }

    private void GenerateTerrain()
    {
        terrainData = new TerrainData();
        terrainData.heightmapResolution = width + 1;
        terrainData.size = new Vector3(width, height, length);
        
        terrain = gameObject.AddComponent<Terrain>();
        terrain.terrainData = terrainData;
        
        gameObject.AddComponent<TerrainCollider>().terrainData = terrainData;

        GenerateHeights();
        AddTextures();
        PlaceResources();
    }

    private void GenerateHeights()
    {
        float[,] heights = new float[width, length];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < length; y++)
            {
                heights[x, y] = CalculateHeight(x, y);
            }
        }
        terrainData.SetHeights(0, 0, heights);
    }

    private float CalculateHeight(int x, int y)
    {
        float xCoord = (float)x / width * scale;
        float yCoord = (float)y / length * scale;

        // Generate base terrain using Perlin noise
        float height = Mathf.PerlinNoise(xCoord, yCoord);
        
        // Add some variation
        height += Mathf.PerlinNoise(xCoord * 2, yCoord * 2) * 0.5f;
        height += Mathf.PerlinNoise(xCoord * 4, yCoord * 4) * 0.25f;

        return height;
    }

    private void AddTextures()
    {
        // Define splat prototypes (terrain textures)
        TerrainLayer[] terrainLayers = new TerrainLayer[3];
        
        // Grass
        terrainLayers[0] = new TerrainLayer();
        terrainLayers[0].diffuseTexture = Resources.Load<Texture2D>("TerrainTextures/Grass");
        terrainLayers[0].tileSize = new Vector2(15, 15);
        
        // Rock
        terrainLayers[1] = new TerrainLayer();
        terrainLayers[1].diffuseTexture = Resources.Load<Texture2D>("TerrainTextures/Rock");
        terrainLayers[1].tileSize = new Vector2(15, 15);
        
        // Sand
        terrainLayers[2] = new TerrainLayer();
        terrainLayers[2].diffuseTexture = Resources.Load<Texture2D>("TerrainTextures/Sand");
        terrainLayers[2].tileSize = new Vector2(15, 15);

        terrainData.terrainLayers = terrainLayers;
    }

    private void PlaceResources()
    {
        int resourceCount = Mathf.FloorToInt(width * length * resourceDensity);
        
        for (int i = 0; i < resourceCount; i++)
        {
            float x = Random.Range(0, width);
            float z = Random.Range(0, length);
            float y = terrain.SampleHeight(new Vector3(x, 0, z));

            Vector3 position = new Vector3(x, y, z);
            
            // Check if position is suitable (not too steep, not in water, etc.)
            if (IsSuitableForResource(position))
            {
                GameObject resourcePrefab = resourcePrefabs[Random.Range(0, resourcePrefabs.Length)];
                Instantiate(resourcePrefab, position, Quaternion.identity, transform);
            }
        }
    }

    private bool IsSuitableForResource(Vector3 position)
    {
        // Check slope
        float maxSlope = 30f;
        float slope = terrain.terrainData.GetSteepness(
            position.x / terrain.terrainData.size.x,
            position.z / terrain.terrainData.size.z
        );
        
        if (slope > maxSlope) return false;

        // Check height (not in water)
        if (position.y < 3f) return false;

        // Check distance from other resources
        Collider[] hitColliders = Physics.OverlapSphere(position, 5f);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject.CompareTag("Resource"))
            {
                return false;
            }
        }

        return true;
    }

    public Vector3 GetRandomSpawnPoint()
    {
        float x = Random.Range(0, width);
        float z = Random.Range(0, length);
        float y = terrain.SampleHeight(new Vector3(x, 0, z));
        
        return new Vector3(x, y, z);
    }

    public bool IsPositionBuildable(Vector3 position)
    {
        float slope = terrain.terrainData.GetSteepness(
            position.x / terrain.terrainData.size.x,
            position.z / terrain.terrainData.size.z
        );
        
        return slope < 15f && position.y > 1f;
    }
}
