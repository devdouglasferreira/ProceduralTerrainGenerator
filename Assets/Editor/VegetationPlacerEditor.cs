using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(VegetationPlacer))]
public class VegetationPlacerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        VegetationPlacer vegetationPlacer = (VegetationPlacer)target;
        if (GUILayout.Button("Regenerate Vegetation"))
        {
            vegetationPlacer.RegenerateVegetation();
        }
    }
}
