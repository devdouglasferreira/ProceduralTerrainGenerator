using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[CustomEditor(typeof(TerrainGenerator))]
public class ProceduralTerrainGeneratorEditor : Editor
{
    TerrainGenerator generator;

    protected void OnEnable()
    {
        generator = target as TerrainGenerator;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GUILayout.Label("Resultant Height Map:");
        GUILayout.Label(GenerateResultantTexture(generator.resultantHeightMap));

        GUILayout.Label("Water Noise Map:");
        GUILayout.Label(GenerateNoiseTexture(generator.waterNoiseMap));

        GUILayout.Label("Ground Noise Map:");
        GUILayout.Label(GenerateNoiseTexture(generator.groundNoiseMap));

        GUILayout.Label("Moutain Noise Map:");
        GUILayout.Label(GenerateNoiseTexture(generator.mountainNoiseMap));


    }

    public Texture2D GenerateNoiseTexture(float[,] noiseMap)
    {
        Texture2D texture = new Texture2D(generator.terrainWidth, generator.terrainHeight);

        for (int x = 0; x < generator.terrainWidth; x++)
        {
            for (int y = 0; y < generator.terrainHeight; y++)
            {
                float sample = noiseMap[x, y];
                texture.SetPixel(x, y, Color.Lerp(Color.black, Color.white, sample));
            }
        }

        texture.Apply();
        return texture;
    }

    public Texture2D GenerateResultantTexture(float[,] resultantHeights)
    {
        Texture2D texture = new Texture2D(generator.terrainWidth, generator.terrainHeight);

        for (int x = 0; x < generator.terrainWidth; x++)
        {
            for (int y = 0; y < generator.terrainHeight; y++)
            {
                float sample = resultantHeights[x, y];

                if (generator.resultantHeightMapColor[x, y] == 0)
                    texture.SetPixel(x, y, Color.blue);
                else if (generator.resultantHeightMapColor[x, y] == 1)
                    texture.SetPixel(x, y, Color.green);
                else
                    texture.SetPixel(x, y, Color.yellow);
            }
        }

        texture.Apply();
        return texture;
    }
}
