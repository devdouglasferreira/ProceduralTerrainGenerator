using UnityEngine;

[ExecuteInEditMode]
public class ProceduralTerrainGenerator : MonoBehaviour
{
    [Header("Terrain Settings")]
    public int terrainWidth = 256;
    public int terrainHeight = 256;
    public int terrainDepth = 20;

    [Header("Water Layer Settings")]
    public float waterHeight = 5.0f;
    public Texture2D waterTexture;

    [Header("Ground Layer Settings")]
    public float groundScale = 20.0f;
    public float groundHeight = 10.0f;
    public float groundOffsetX = 0.0f;
    public float groundOffsetY = 0.0f;
    public float groundFrequency = 1.0f;
    public Texture2D groundTexture;

    [Header("Mountain Layer Settings")]
    public float mountainScale = 50.0f;
    public float mountainHeight = 15.0f;
    public float mountainOffsetX = 100.0f;
    public float mountainOffsetY = 100.0f;
    public float mountainFrequency = 0.5f;
    public Texture2D mountainTexture;

    [HideInInspector]
    public Texture2D groundNoiseTexture;
    [HideInInspector]
    public Texture2D mountainNoiseTexture;

    private Terrain terrain;
    private TerrainData terrainData;

    private void OnValidate()
    {
        GenerateTerrain();
    }

    public void GenerateTerrain()
    {
        if (terrain == null)
        {
            terrain = GetComponent<Terrain>();
            if (terrain == null)
            {
                terrain = gameObject.AddComponent<Terrain>();
            }
        }

        if (terrainData == null)
        {
            terrainData = new TerrainData();
            terrain.terrainData = terrainData;
        }

        terrainData.heightmapResolution = terrainWidth + 1;
        terrainData.size = new Vector3(terrainWidth, terrainDepth, terrainHeight);

        float[,] heights = GenerateHeights();
        terrainData.SetHeights(0, 0, heights);

        // Generate noise maps for visualization
        groundNoiseTexture = GenerateNoiseTexture(groundScale, groundOffsetX, groundOffsetY, groundFrequency);
        mountainNoiseTexture = GenerateNoiseTexture(mountainScale, mountainOffsetX, mountainOffsetY, mountainFrequency);

        // Apply textures
        ApplyTextures();
    }

    private float[,] GenerateHeights()
    {
        float[,] heights = new float[terrainWidth, terrainHeight];

        // Water layer (flat)
        for (int x = 0; x < terrainWidth; x++)
        {
            for (int y = 0; y < terrainHeight; y++)
            {
                heights[x, y] = waterHeight / terrainDepth;
            }
        }

        // Ground layer
        ApplyPerlinNoiseLayer(ref heights, groundScale, groundHeight, groundOffsetX, groundOffsetY, groundFrequency);

        // Mountain layer
        ApplyPerlinNoiseLayer(ref heights, mountainScale, mountainHeight, mountainOffsetX, mountainOffsetY, mountainFrequency);

        return heights;
    }

    private void ApplyPerlinNoiseLayer(ref float[,] heights, float scale, float height, float offsetX, float offsetY, float frequency)
    {
        for (int x = 0; x < terrainWidth; x++)
        {
            for (int y = 0; y < terrainHeight; y++)
            {
                float xCoord = (float)x / terrainWidth * scale + offsetX;
                float yCoord = (float)y / terrainHeight * scale + offsetY;
                float sample = Mathf.PerlinNoise(xCoord * frequency, yCoord * frequency) * height / terrainDepth;
                heights[x, y] += sample;
            }
        }
    }

    private Texture2D GenerateNoiseTexture(float scale, float offsetX, float offsetY, float frequency)
    {
        Texture2D texture = new Texture2D(terrainWidth, terrainHeight);

        for (int x = 0; x < terrainWidth; x++)
        {
            for (int y = 0; y < terrainHeight; y++)
            {
                float xCoord = (float)x / terrainWidth * scale + offsetX;
                float yCoord = (float)y / terrainHeight * scale + offsetY;
                float sample = Mathf.PerlinNoise(xCoord * frequency, yCoord * frequency);
                texture.SetPixel(x, y, Color.Lerp(Color.black, Color.white, sample));
            }
        }

        texture.Apply();
        return texture;
    }

    private void ApplyTextures()
    {
        // Create TerrainLayers
        TerrainLayer waterLayer = new TerrainLayer();
        waterLayer.diffuseTexture = waterTexture;
        waterLayer.tileSize = new Vector2(terrainWidth, terrainHeight);

        TerrainLayer groundLayer = new TerrainLayer();
        groundLayer.diffuseTexture = groundTexture;
        groundLayer.tileSize = new Vector2(terrainWidth, terrainHeight);

        TerrainLayer mountainLayer = new TerrainLayer();
        mountainLayer.diffuseTexture = mountainTexture;
        mountainLayer.tileSize = new Vector2(terrainWidth, terrainHeight);

        terrainData.terrainLayers = new TerrainLayer[] { waterLayer, groundLayer, mountainLayer };

        // Assign textures to the splatmap
        float[,,] splatmapData = new float[terrainData.alphamapWidth, terrainData.alphamapHeight, 3];

        for (int y = 0; y < terrainData.alphamapHeight; y++)
        {
            for (int x = 0; x < terrainData.alphamapWidth; x++)
            {
                float height = terrainData.GetHeight(x, y);

                // Normalize height to a range of 0 to 1
                float normHeight = height / terrainDepth;

                // Determine the layer based on height
                if (normHeight <= waterHeight / terrainDepth)
                {
                    splatmapData[x, y, 0] = 1; // Water
                }
                else if (normHeight <= (waterHeight + groundHeight) / terrainDepth)
                {
                    splatmapData[x, y, 1] = 1; // Ground
                }
                else
                {
                    splatmapData[x, y, 2] = 1; // Mountains
                }
            }
        }

        terrainData.SetAlphamaps(0, 0, splatmapData);
    }
}
