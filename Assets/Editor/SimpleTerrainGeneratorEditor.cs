using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SimpleTerrainGenerator))]
public class SimpleTerrainGeneratorEditor : Editor
{
    private Texture2D noiseTexture;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SimpleTerrainGenerator generator = (SimpleTerrainGenerator)target;
        noiseTexture = GenerateNoiseTexture(generator.noiseMap);

        if (noiseTexture != null)
        {
            GUILayout.Label(new GUIContent(noiseTexture), GUILayout.Width(generator.width), GUILayout.Height(generator.height));
        }
    }

    public Texture2D GenerateNoiseTexture(float[,] noiseMap)
    {
        Texture2D noiseTexture = new Texture2D(noiseMap.GetLength(0), noiseMap.GetLength(1));
        for (int x = 0; x < noiseMap.GetLength(0); x++)
        {
            for (int y = 0; y < noiseMap.GetLength(1); y++)
            {
                float sample = noiseMap[x, y];
                noiseTexture.SetPixel(x, y, new Color(sample, sample, sample));
            }
        }
        noiseTexture.Apply();
        return noiseTexture;
    }
}