using UnityEngine;

[ExecuteInEditMode]
public class VegetationPlacer : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject[] treePrefabs;
    public GameObject[] bushPrefabs;

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
            PlacePrefabRandomly(treePrefabs);
        }

        for (int i = 0; i < numberOfBushes; i++)
        {
            PlacePrefabRandomly(bushPrefabs);
        }
    }

    private void PlacePrefabRandomly(GameObject[] prefabs)
    {
        bool placed = false;

        while (!placed)
        {
            float randomX = Random.Range(0, terrainWidth);
            float randomZ = Random.Range(0, terrainHeight);
            int terrainX = Mathf.RoundToInt(randomX);
            int terrainZ = Mathf.RoundToInt(randomZ);

            if (terrainGenerator.resultantHeightMapColor[terrainX, terrainZ] == 1 || terrainGenerator.resultantHeightMapColor[terrainX, terrainZ] == 2) // 1 indicates ground layer
            {
                float terrainY = terrainGenerator.resultantHeightMap[terrainX, terrainZ] * terrainGenerator.terrainDepth;
                Vector3 position = new Vector3(randomX, terrainY, randomZ);
                GameObject prefab = prefabs[Random.Range(0, prefabs.Length)];
                Instantiate(prefab, position, Quaternion.identity, transform); // Set the parent to the VegetationPlacer GameObject
                placed = true;
            }
        }
    }

    public void ClearVegetation()
    {
        foreach (Transform child in transform)
        {
            DestroyImmediate(child.gameObject);
        }
    }
}
