using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ProceduralTerrainGenerator))]
public class ProceduralTerrainGeneratorEditor : Editor
{
    ProceduralTerrainGenerator generator;

    protected void OnEnable()
    {
        generator = target as ProceduralTerrainGenerator;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        //if (GUILayout.Button("Generate Terrain"))
        //{
        //    generator.GenerateTerrain();
        //}

        GUILayout.Label("Ground Noise Map:");
        DrawNoiseMap(generator.terrainWidth, generator.terrainHeight, generator.groundScale, new Vector2(generator.groundOffsetX, generator.groundOffsetX));

        GUILayout.Label("Mountain Noise Map:");
        DrawNoiseMap(generator.terrainWidth, generator.terrainHeight, generator.mountainScale, new Vector2(generator.mountainOffsetX, generator.mountainOffsetY));
    }

    private void DrawNoiseMap(int width, int length, float scale, Vector2 offset)
    {
        Texture2D noiseTex = new Texture2D(width, length);
        Color[] pixels = new Color[width * length];

        for (int y = 0; y < length; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float xCoord = (float)x / width * scale + offset.x;
                float yCoord = (float)y / length * scale + offset.y;
                float sample = Mathf.PerlinNoise(xCoord, yCoord);
                pixels[y * width + x] = new Color(sample, sample, sample);
            }
        }

        noiseTex.SetPixels(pixels);
        noiseTex.Apply();

        GUILayout.Label(noiseTex);
    }
}
