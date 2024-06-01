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
        noiseTexture = generator.GenerateNoiseTexture();

        if (noiseTexture != null)
        {
            GUILayout.Label(new GUIContent(noiseTexture), GUILayout.Width(generator.width), GUILayout.Height(generator.height));
        }
    }
}