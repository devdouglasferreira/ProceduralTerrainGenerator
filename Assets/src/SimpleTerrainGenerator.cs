using UnityEngine;
using UnityEditor;

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

    private Terrain terrain;
    private TerrainData terrainData;

    private void OnValidate()
    {
        if (terrain == null)
        {
            terrain = GetComponent<Terrain>();
            if (terrain == null)
            {
                Debug.LogError("No Terrain component found.");
                return;
            }
        }

        if (terrainData == null)
        {
            terrainData = terrain.terrainData;
        }

        GenerateTerrain();
    }

    void GenerateTerrain()
    {
        terrainData.heightmapResolution = width + 1;
        terrainData.size = new Vector3(width, heightMultiplier, height);

        float[,] heights = new float[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                heights[x, y] = Mathf.PerlinNoise((x + offset.x) / scale, (y + offset.y) / scale);
            }
        }

        terrainData.SetHeights(0, 0, heights);

        ApplyTexture();
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

    public Texture2D GenerateNoiseTexture()
    {
        Texture2D noiseTexture = new Texture2D(width, height);
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float sample = Mathf.PerlinNoise((x + offset.x) / scale, (y + offset.y) / scale);
                noiseTexture.SetPixel(x, y, new Color(sample, sample, sample));
            }
        }
        noiseTexture.Apply();
        return noiseTexture;
    }
}