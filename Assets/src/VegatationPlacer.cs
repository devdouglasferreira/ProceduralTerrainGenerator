using UnityEngine;

[ExecuteInEditMode]
public class VegetationPlacer : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject treePrefab;
    public GameObject bushPrefab;

    [Header("Placement Settings")]
    public int numberOfTrees = 100;
    public int numberOfBushes = 100;
    public float terrainWidth;
    public float terrainHeight;

    private TerrainGenerator terrainGenerator;

    private void OnValidate()
    {
        terrainGenerator = FindObjectOfType<TerrainGenerator>();

        if (terrainGenerator != null)
        {
            ClearVegetation();
            PlaceVegetation();
        }
        else
        {
            Debug.LogError("TerrainGenerator not found in the scene.");
        }
    }

    public void RegenerateVegetation()
    {
        if (terrainGenerator != null)
        {
            ClearVegetation();
            PlaceVegetation();
        }
        else
        {
            Debug.LogError("TerrainGenerator not found in the scene.");
        }
    }

    private void PlaceVegetation()
    {
        for (int i = 0; i < numberOfTrees; i++)
        {
            PlacePrefabRandomly(treePrefab);
        }

        for (int i = 0; i < numberOfBushes; i++)
        {
            PlacePrefabRandomly(bushPrefab);
        }
    }

    private void PlacePrefabRandomly(GameObject prefab)
    {
        bool placed = false;

        while (!placed)
        {
            float randomX = Random.Range(0, terrainWidth);
            float randomZ = Random.Range(0, terrainHeight);
            int terrainX = Mathf.RoundToInt(randomX);
            int terrainZ = Mathf.RoundToInt(randomZ);

            if (terrainGenerator.resultantHeightMapColor[terrainX, terrainZ] == 1) // 1 indicates ground layer
            {
                float terrainY = terrainGenerator.resultantHeightMap[terrainX, terrainZ] * terrainGenerator.terrainDepth;
                Vector3 position = new Vector3(randomX, terrainY, randomZ);
                Instantiate(prefab, position, Quaternion.identity, transform); // Set the parent to the VegetationPlacer GameObject
                placed = true;
            }
        }
    }

    private void ClearVegetation()
    {
        foreach (Transform child in transform)
        {
            DestroyImmediate(child.gameObject);
        }
    }
}
