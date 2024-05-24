using UnityEngine;

[ExecuteInEditMode]
public class TextureApplier : MonoBehaviour
{
    private Terrain _terrain;
    

    [SerializeField]
    private Texture2D[] _layerTextures;

    public float waterLevel = 0.2f;
    public float groundLevel = 0.4f;

    protected void Awake()
    {
        
    }

    protected void OnValidate()
    {
        _terrain = GetComponent<Terrain>();

        if (_layerTextures == null)
            _layerTextures = new Texture2D[0];

        ApplyTextures();
    }

    private void ApplyTextures()
    {
        TerrainLayer[] terrainLayers = new TerrainLayer[_layerTextures.Length];

        for (int i = 0; i < _layerTextures.Length; i++)
            terrainLayers[i] = new TerrainLayer { diffuseTexture = _layerTextures[i] };
        
        _terrain.terrainData.terrainLayers = terrainLayers;

        float[,,] alphaMaps = new float[_terrain.terrainData.alphamapWidth, _terrain.terrainData.alphamapHeight, 3];

        for (int y = 0; y < _terrain.terrainData.alphamapHeight; y++)
        {
            for (int x = 0; x < _terrain.terrainData.alphamapWidth; x++)
            {
                float normalizedX = (float)x / (_terrain.terrainData.alphamapWidth - 1);
                float normalizedY = (float)y / (_terrain.terrainData.alphamapHeight - 1);
                float height = _terrain.terrainData.GetHeight(Mathf.RoundToInt(normalizedX * _terrain.terrainData.heightmapResolution), Mathf.RoundToInt(normalizedY * _terrain.terrainData.heightmapResolution)) / _terrain.terrainData.size.y;

                float[] splatWeights = new float[3];

                if (height < waterLevel) // Water layer
                {
                    splatWeights[1] = 1;
                }
                else if (height < groundLevel) // Ground layer
                {
                    splatWeights[0] = 1;
                }
                else // Mountain layer
                {
                    splatWeights[2] = 1;
                }

                float total = splatWeights[0] + splatWeights[1] + splatWeights[2];
                splatWeights[0] /= total;
                splatWeights[1] /= total;
                splatWeights[2] /= total;

                for (int i = 0; i < 3; i++)
                {
                    alphaMaps[x, y, i] = splatWeights[i];
                }
            }
        }

        _terrain.terrainData.SetAlphamaps(0, 0, alphaMaps);
    }
}
