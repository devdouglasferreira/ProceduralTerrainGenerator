using UnityEngine;

[ExecuteInEditMode]
public class FoliageGenerator : MonoBehaviour
{
    public Terrain terrain;
    public GameObject[] trees;
    public GameObject[] bushes;
    public int treeCount = 100;
    public int bushCount = 100;

    void OnValidate()
    {
        GenerateFoliage();
    }

    void GenerateFoliage()
    {
        RemoveFoliage();

        for (int i = 0; i < treeCount; i++)
        {
            PlaceObject(trees[Random.Range(0, trees.Length)]);
        }

        for (int i = 0; i < bushCount; i++)
        {
            PlaceObject(bushes[Random.Range(0, bushes.Length)]);
        }
    }

    void RemoveFoliage()
    {
        foreach (Transform child in transform)
        {
            DestroyImmediate(child.gameObject);
        }
    }

    void PlaceObject(GameObject obj)
    {
        float x = Random.Range(0, terrain.terrainData.size.x);
        float z = Random.Range(0, terrain.terrainData.size.z);
        float y = terrain.SampleHeight(new Vector3(x, 0, z));
        Vector3 position = new Vector3(x, y, z);

        if (y > 0.2f)
        {
            Instantiate(obj, position, Quaternion.identity, transform);
        }
    }
}