using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Cell3D // : MonoBehaviour
{
    public bool collapsed;
    public Tile3D[] tileOptions;
    public Vector3Int position;
    public Vector3 rotation;
    public bool isTop;

    public void CreateCell(bool collapseState, List<Tile3D> tiles, Vector3Int _position, Vector3 _rotation)
    {
        collapsed = collapseState;
        tileOptions = tiles.ToArray();
        position = _position;
        rotation = _rotation;
    }

    public void RecreateCell(Tile3D[] tiles)
    {
        tileOptions = tiles;
    }
}


[System.Serializable]
public struct Cell3DData
{
    public string uid;
    public Vector3Int position;
    public float RotationY;
    public TypeEntity typeCell;
    // public int stateNode;
    // public int top; // 0 - false, 1 - true
}

[System.Serializable]
public struct Cell3DItemForCreate
{
    public GameObject wrapper;
    public Cell3D cell3D;
    public Tile3DGroup tile3DGroup;
}
