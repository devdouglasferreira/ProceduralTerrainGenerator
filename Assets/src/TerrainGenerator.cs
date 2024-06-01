using System;
using UnityEngine;

[ExecuteInEditMode]
public class TerrainGenerator : MonoBehaviour
{
    [Header("Terrain Settings")]
    [Range(10, 1000)]
    public int terrainWidth = 256;
    [Range(10, 1000)]
    public int terrainHeight = 256;
    [Range(10, 1000)]
    public int terrainDepth = 100;


    [Header("Water Layer Settings")]
    public float waterScale = 10.0f;
    [Range(0, 1)]
    public float waterMapWeight = 0.5f;
    public float waterOffsetX = 0.0f;
    public float waterOffsetY = 0.0f;
    public Texture2D waterTexture;

    [Header("Ground Layer Settings")]
    //[Range(3, 200)]
    //public float minGroundHeight = 3f;
    [Range(0, 1)]
    public float groundMapWeight = 0.5f;
    public float groundMinHeight = 3.0f;
    public float groundMaxHeight = 15.0f;
    public float groundScale = 10.0f;
    public float groundOffsetX = 0.0f;
    public float groundOffsetY = 0.0f;
    public Texture2D groundTexture;

    [Header("Moutains Layer Settings")]
    ///Range(15, 1000)]
    //public float minMoutainHeight = 3f;
    [Range(0, 1)]
    public float mountainMapWeight = 0.5f;
    public float mountainMinHeight = 15.0f;
    public float mountainMaxHeight = 250;
    public float mountainScale = 10.0f;
    public float moutainOffsetX = 0.0f;
    public float moutainOffsetY = 0.0f;
    public Texture2D mountainTexture;

    [HideInInspector]
    public float[,] waterNoiseMap;

    [HideInInspector]
    public float[,] groundNoiseMap;

    [HideInInspector]
    public float[,] mountainNoiseMap;

    [HideInInspector]
    public float[,] resultantHeightMap;

    [HideInInspector]
    public int[,] resultantHeightMapColor;

    private Terrain terrain;
    private TerrainData terrainData;

    protected void OnValidate()
    {
        terrain = GetComponent<Terrain>() ?? gameObject.AddComponent<Terrain>();
        terrainData = terrain.terrainData ?? new TerrainData();

        waterNoiseMap = GeneratePelinNoiseMap(terrainWidth, terrainHeight, waterScale, waterOffsetX, waterOffsetY);
        groundNoiseMap = GeneratePelinNoiseMap(terrainWidth, terrainHeight, groundScale, groundOffsetX, groundOffsetY);
        mountainNoiseMap = GeneratePelinNoiseMap(terrainWidth, terrainHeight, mountainScale, moutainOffsetX, moutainOffsetY);
        (resultantHeightMap, resultantHeightMapColor) = CalculateResultantHeightMap();

        ApplyTerrainHeight();
        ApplyTerrainTextures();

    }

    private float[,] GeneratePelinNoiseMap(int width, int height, float scale, float offsetX, float offsetY)
    {
        float[,] noiseMap = new float[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float xCoord = (float)x / width * scale + offsetX;
                float yCoord = (float)y / height * scale + offsetY;
                float sample = Mathf.PerlinNoise(xCoord, yCoord);

                noiseMap[x, y] = Mathf.Max(0, sample);
                //Debug.Assert(noiseMap[x, y] >= 0 && noiseMap[x, y] <= 1);
            }
        }

        return noiseMap;
    }

    private (float[,], int[,]) CalculateResultantHeightMap()
    {
        float[,] resultantHeightMap = new float[terrainWidth, terrainHeight];
        int[,] resultantHeightMapColor = new int[terrainWidth, terrainHeight];

        for (int x = 0; x < terrainWidth; x++)
        {
            for (int y = 0; y < terrainHeight; y++)
            {
                var waterHeight = waterNoiseMap[x, y] * waterMapWeight;
                var groundHeight = groundNoiseMap[x, y] * groundMapWeight;
                var mountainHeight = mountainNoiseMap[x, y] * mountainMapWeight;

                float blendedHeight = 0;
                float waterBlend = Mathf.SmoothStep(0, 1, waterHeight);
                float groundBlend = Mathf.SmoothStep(0, 1, groundHeight);
                float mountainBlend = Mathf.SmoothStep(0, 1, mountainHeight);

                // Normalize the blend values so they sum up to 1
                float totalBlend = groundBlend + mountainBlend;
                groundBlend /= totalBlend;
                mountainBlend /= totalBlend;

                // Blend ground and mountain heights based on their respective blends
                blendedHeight += groundBlend * Map(groundHeight, 0, 1, groundMinHeight, groundMaxHeight);
                blendedHeight += mountainBlend * Map(mountainHeight, 0, 1, mountainMinHeight, mountainMaxHeight);

                // Apply the flat water layer height if water is the dominant layer
                if (waterBlend > groundBlend && waterBlend > mountainBlend)
                {
                    resultantHeightMap[x, y] = Map(waterHeight, 0, 1, 0, groundMinHeight) / terrainDepth;
                    resultantHeightMapColor[x, y] = 0;
                }
                else
                {
                    resultantHeightMap[x, y] = blendedHeight / terrainDepth;

                    // Determine the dominant layer for coloring
                    if (mountainBlend > groundBlend)
                    {
                        resultantHeightMapColor[x, y] = 2;
                    }
                    else
                    {
                        resultantHeightMapColor[x, y] = 1;
                    }
                }
            }
        }

        return (resultantHeightMap, resultantHeightMapColor);
    }


    private void ApplyTerrainHeight()
    {
        terrainData.heightmapResolution = terrainWidth + 1;
        terrainData.size = new Vector3(terrainWidth, terrainDepth, terrainHeight);

        float[,] heights = new float[terrainWidth, terrainHeight];
        for (int x = 0; x < terrainWidth; x++)
        {
            for (int y = 0; y < terrainHeight; y++)
            {
                if (resultantHeightMapColor[x, y] == 0)
                    heights[x, y] = resultantHeightMap[x, y];
                else
                    heights[x, y] = resultantHeightMap[x, y];
            }
        }
        terrainData.SetHeights(0, 0, heights);
    }


    private void ApplyTerrainTextures()
    {
        TerrainLayer waterLayer = new TerrainLayer
        {
            diffuseTexture = waterTexture,
            tileSize = new Vector2(terrainWidth / 10f, terrainHeight / 10f)
        };

        TerrainLayer groundLayer = new TerrainLayer
        {
            diffuseTexture = groundTexture,
            tileSize = new Vector2(terrainWidth / 10f, terrainHeight / 10f)
        };

        TerrainLayer mountainLayer = new TerrainLayer
        {
            diffuseTexture = mountainTexture,
            tileSize = new Vector2(terrainWidth / 10f, terrainHeight / 10f)
        };

        terrainData.terrainLayers = new TerrainLayer[] { waterLayer, groundLayer, mountainLayer };

        float[,,] splatmapData = new float[terrainData.alphamapWidth, terrainData.alphamapHeight, 3];

        for (int y = 0; y < terrainData.alphamapHeight; y++)
        {
            for (int x = 0; x < terrainData.alphamapWidth; x++)
            {
                if (x < terrainWidth && y < terrainHeight)
                {
                    int colorIndex = resultantHeightMapColor[x, y];
                    splatmapData[x, y, colorIndex] = 1;
                }
            }
        }

        terrainData.SetAlphamaps(0, 0, splatmapData);
    }

    private float Map(float value, float fromSource, float toSource, float fromTarget, float toTarget)
    {
        return (value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget) + fromTarget;
    }

}