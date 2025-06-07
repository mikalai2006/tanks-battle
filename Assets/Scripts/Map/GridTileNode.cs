
using System;

using UnityEngine;

[Flags]
public enum StateNode
{
    Disable = 1 << 0,
    Empty = 1 << 1,
    Occupied = 1 << 2
}

[Serializable]
public class GridTileNode : IHeapItem<GridTileNode>
{
    [NonSerialized] private readonly GridTile<GridTileNode> _grid;
    [NonSerialized] private readonly GridTileHelper _gridHelper;
    [SerializeField] public StateNode StateNode = StateNode.Empty;
    public int X;
    public int Y;
    public TypeGround TypeGround = TypeGround.None;

    [NonSerialized] public int level;
    [NonSerialized] public Vector3Int position;

    [NonSerialized] private BaseMachine _ocuppiedUnit = null;
    public BaseMachine OccupiedUnit => _ocuppiedUnit;
    public bool IsAllowSpawn =>
        StateNode.HasFlag(StateNode.Empty)
        && !StateNode.HasFlag(StateNode.Occupied);
    // (StateNode.Empty | ~StateNode.Protected | ~StateNode.Occupied) == (StateNode.Empty | ~StateNode.Protected | ~StateNode.Occupied);

    private int heapIndex;
    public int HeapIndex
    {
        get
        {
            return heapIndex;
        }
        set
        {
            heapIndex = value;
        }
    }

    [NonSerialized] public int countRelatedNeighbors = 0;
    [NonSerialized] public GridTileNode cameFromNode;
    [NonSerialized] public int gCost;
    [NonSerialized] public int hCost;
    [NonSerialized] public float fCost;
    [NonSerialized] public int koofPath = 10;
    [NonSerialized] public int levelPath = 0;
    [NonSerialized] public bool isCreated = false;
    [NonSerialized] public bool isEdge = false;

    public GridTileNode(GridTile<GridTileNode> grid, GridTileHelper gridHelper, int x, int y)
    {
        position = new Vector3Int(x, y, 0);
        _gridHelper = gridHelper;
        _grid = grid;
        this.X = x;
        this.Y = y;
    }

    public int CompareTo(GridTileNode nodeToCompare)
    {
        int compare = fCost.CompareTo(nodeToCompare.fCost);
        if (compare == 0)
            compare = hCost.CompareTo(nodeToCompare.hCost);
        return -compare;
    }

    public void SetDisableNode()
    {
        StateNode = StateNode.Disable;
    }

    public void AddStateNode(StateNode state)
    {
        StateNode |= state;
    }

    public void RemoveStateNode(StateNode state)
    {
        StateNode ^= state;
    }

    /// <summary>
    /// Set occupied entity for node.
    /// </summary>
    /// <param name="entity">Entity or null</param>
    public void SetOcuppiedUnit(BaseMachine entity)
    {
        _ocuppiedUnit = entity;
        if (entity == null)
        {
            StateNode &= ~StateNode.Occupied;
            StateNode |= StateNode.Empty;
        }
        else
        {
            StateNode &= ~StateNode.Empty;
            StateNode |= StateNode.Occupied;
        }
    }

    public void CalculateFCost()
    {
        fCost = gCost + hCost;

    }


#if UNITY_EDITOR
    public override string ToString()
    {
        return "GridTileNode:::" +
            // "keyArea=" + KeyArea + ",\n" +
            "[x" + position.x + ",y" + position.y + "] \n" +
            "typeGround=" + TypeGround + ",\n" +
            "OccupiedUnit=" + OccupiedUnit?.ToString() + ",\n" +
            // "GuestedUnit=" + _guestedUnit?.ToString() + ",\n" +
            "StateNode=" + Convert.ToString((int)StateNode, 2) + ",\n" +
            // "ProtectedUnit=" + ProtectedUnit?.ToString() + ",\n" +
            "countNeighbours=" + countRelatedNeighbors + ",\n" +
            "(gCost=" + gCost + ") (hCost=" + hCost + ") (fCost=" + fCost + ")";
    }

#endif

}


[Flags]
public enum TypeGround
{
    None = 1 << 0,
    Dirt = 1 << 1,
    Grass = 1 << 2,
    Sand = 1 << 3,
    Water = 1 << 4,
}