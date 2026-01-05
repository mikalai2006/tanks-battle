using System.Collections.Generic;
using UnityEngine;

public class Tile3DGroup : MonoBehaviour
{
    [SerializeField] List<Tile3D> tiles;
    public List<Tile3D> Tiles => tiles;

    void Awake()
    {
        tiles = new List<Tile3D>();
    }

    public void AddTile(Tile3D tile)
    {
        tiles.Add(tile);
    }

    public void RemoveTile(Tile3D tile)
    {
        tiles.Remove(tile);
    }
}