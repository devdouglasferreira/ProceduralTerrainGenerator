using UnityEngine;

[ExecuteInEditMode]
public class SimpleTerrainGenerator : MonoBehaviour
{
    [Header("Noise Settings")]
    public float scale = 20f;
    public float heightMultiplier = 5f;
    public Vector2 offset;

    [Header("Terrain Settings")]
    public int width = 256;
    public int height = 256;
    public Texture2D terrainTexture;
    
    [HideInInspector]
    public float[,] noiseMap;


    private Terrain terrain;
    private TerrainData terrainData;


    private void OnValidate()
    {
        terrain = GetComponent<Terrain>() ?? gameObject.AddComponent<Terrain>();
        terrainData = terrain.terrainData ?? new TerrainData();

        noiseMap = GenerateNoiseMap();

        GenerateTerrain(noiseMap);
        ApplyTexture();
    }

    private void GenerateTerrain(float[,] noiseMap)
    {
        float[,] heights = new float[width, height];

        terrainData.heightmapResolution = width + 1;
        terrainData.size = new Vector3(width, heightMultiplier, height);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++) 
                heights[x, y] = noiseMap[x, y];
           
        }
        terrainData.SetHeights(0, 0, heights);
    }

    private float[,] GenerateNoiseMap()
    {
        float[,] noiseMap = new float[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
                noiseMap[x, y] = Mathf.PerlinNoise((x + offset.x) / scale, (y + offset.y) / scale);
        }
        return noiseMap;
    }

    void ApplyTexture()
    {
        if (terrainTexture == null)
        {
            Debug.LogError("No terrain texture assigned.");
            return;
        }

        TerrainLayer[] terrainLayers = new TerrainLayer[1];
        terrainLayers[0] = new TerrainLayer
        {
            diffuseTexture = terrainTexture,
            tileSize = new Vector2(10, 10)
        };

        terrainData.terrainLayers = terrainLayers;

        float[,,] alphaMap = new float[terrainData.alphamapWidth, terrainData.alphamapHeight, 1];

        for (int y = 0; y < terrainData.alphamapHeight; y++)
        {
            for (int x = 0; x < terrainData.alphamapWidth; x++)
            {
                alphaMap[x, y, 0] = 1;
            }
        }

        terrainData.SetAlphamaps(0, 0, alphaMap);
    }


}
