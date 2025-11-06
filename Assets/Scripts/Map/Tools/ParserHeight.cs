using System;
using System.Collections.Generic;
using UnityEngine;

public class ParserHeight : MonoBehaviour
{
    public TextureHeightMapperSettings _settings;
    public Dictionary<Vector2Int, int> data;
    public Vector2Int gridSize;

    public void SetConfig(TextureHeightMapperSettings settings)
    {
        _settings = settings;
    }

    public void Init()
    {
        data = new Dictionary<Vector2Int, int>();

        gridSize = new Vector2Int(_settings.texture.width, _settings.texture.height);
        
        _settings.nameMap = _settings.texture.name;
    }

    // void Start()
    // {
    //     GenerateHeightMap();
    // }

    public Dictionary<Vector2Int, int> GenerateHeightMap()
    {

        for (var x = 0; x < gridSize.x; x++)
        {
            for (var y = 0; y < gridSize.y; y++)
            {
                var color = _settings.texture.GetPixel(x, y);
                var position = new Vector2Int(x, y);
                var height = Mathf.RoundToInt(color.r * _settings.heightSize);
                data[position] = Mathf.RoundToInt(height);
                // Debug.Log($"pos: {position}, height: {data[position]}[{color.r}]");
            }
        }

        return data;
    }

}

[System.Serializable]
public class TextureHeightMapperSettings
{
    public float heightSize;
    public Texture2D texture;
    public string nameMap;
}
