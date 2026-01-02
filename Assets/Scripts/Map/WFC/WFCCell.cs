using UnityEngine;

public class WFCCell
{
    public bool collapsed;
    public Tile3D[] tileOptions;
    public Vector3Int position;
    public Tile3D MBObject;

    public WFCCell(bool collapseState, Tile3D[] tiles, Vector3Int pos)
    {
        collapsed = collapseState;
        tileOptions = tiles;
        position = pos;
    }

    public void RecreateCell(Tile3D[] tiles)
    {
        tileOptions = tiles;
    }

    public void SetMBObject(Tile3D tile3D)
    {
        MBObject = tile3D;
    }
}
