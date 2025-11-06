using System;
using System.Collections.Generic;
using System.Linq;
using Mikalai2006.Voxel;
using UnityEngine;

// [RequireComponent(typeof(VoxelMeshRender))]
public class Tile3D : MonoBehaviour
{
    public string UID;
    // public float VoxelSize = 0.1f;
    public int TileSideVoxels = 5;

    [Range(1, 100)]
    public int Weight = 50;

    public RotationType Rotation;
    [HideInInspector] public VoxelMeshRender voxelMeshRender;

    public bool isEmpty;

    public enum RotationType
    {
        OnlyRotation,
        TwoRotations,
        FourRotations
    }

    [HideInInspector]public Voxel[] ColorsRight;
    [HideInInspector] public Voxel[] ColorsForward;
    [HideInInspector] public Voxel[] ColorsLeft;
    [HideInInspector] public Voxel[] ColorsBack;
    [HideInInspector] public Voxel[] ColorsTop;
    [HideInInspector] public Voxel[] ColorsBottom;
    
    
    [HideInInspector] public MeshConfig meshConfig;

    public Tile3D[] tileOptions;
    public bool isCollapsed;
    public void CreateNode(bool collapseState, List<Tile3D> tiles)
    {
        isCollapsed = collapseState;
        tileOptions = tiles.ToArray();
    }

    public void RecreateNode(Tile3D[] tiles)
    {
        tileOptions = tiles;
    }

    public void Start()
    {
        voxelMeshRender = transform.GetComponentInChildren<VoxelMeshRender>();

        if (voxelMeshRender == null)
        {
            Debug.Log($"<color=red>Ошибка тайла {gameObject.name}: Не найден компонент VoxelMeshRender</color>");
            return;
        }

        //voxelMeshRender.OnSetData += OnStart;
    }

    void OnDestroy()
    {
        //voxelMeshRender.OnSetData -= OnStart;
    }

    public void OnStart()
    {
        meshConfig = voxelMeshRender.Config;

        if (!voxelMeshRender.Config.isOneMesh)
        {
            Debug.Log($"<color=red>Tile3D требует одного меша: В компоненте {gameObject.name} не установлена опция - isOneMesh</color>");
            return;
        }

        TileSideVoxels = meshConfig.sOVoxelData.Bounds.x;

        if (!(meshConfig.sOVoxelData.Bounds.x == meshConfig.sOVoxelData.Bounds.y
            && meshConfig.sOVoxelData.Bounds.y == meshConfig.sOVoxelData.Bounds.z))
        {
            // Debug.Log($"<color=yellow>Предупреждение: стороны тайла {gameObject.name} должны быть равными по трем измерениям! Текущее значение: {meshConfig.sOVoxelData.Bounds}</color>");
            TileSideVoxels = Mathf.Max(meshConfig.sOVoxelData.Bounds.x, meshConfig.sOVoxelData.Bounds.y, meshConfig.sOVoxelData.Bounds.z);
        }

        // ColorsRight = new Voxel[TileSideVoxels * TileSideVoxels];
        // ColorsForward = new Voxel[TileSideVoxels * TileSideVoxels];
        // ColorsLeft = new Voxel[TileSideVoxels * TileSideVoxels];
        // ColorsBack = new Voxel[TileSideVoxels * TileSideVoxels];
        // ColorsTop = new Voxel[TileSideVoxels * TileSideVoxels];
        // ColorsBottom = new Voxel[TileSideVoxels * TileSideVoxels];
        
        if (isEmpty)
        {
            ColorsRight = Enumerable.Repeat(new Voxel() { color = Color.clear }, TileSideVoxels * TileSideVoxels).ToArray();
            ColorsForward = Enumerable.Repeat(new Voxel() { color = Color.clear }, TileSideVoxels * TileSideVoxels).ToArray();
            ColorsLeft = Enumerable.Repeat(new Voxel() { color = Color.clear }, TileSideVoxels * TileSideVoxels).ToArray();
            ColorsBack = Enumerable.Repeat(new Voxel(){color=Color.clear}, TileSideVoxels*TileSideVoxels).ToArray();
        } else
        {
            ColorsRight = voxelMeshRender.Config.sOVoxelData.ColorsRight;
            ColorsForward = voxelMeshRender.Config.sOVoxelData.ColorsForward;
            ColorsLeft = voxelMeshRender.Config.sOVoxelData.ColorsLeft;
            ColorsBack = voxelMeshRender.Config.sOVoxelData.ColorsBack;
        }

        // CalculateSidesColors();
    }

    public void CalculateSidesColors()
    {
        for (int i = 0; i < TileSideVoxels; i++)
        {
            for (int y = 0; y < TileSideVoxels; y++)
            {
                ColorsForward[i * TileSideVoxels + y] = GetVoxelColor(y, i, DirectionSideTile.Forward);
                ColorsRight[i * TileSideVoxels + y] = GetVoxelColor(y, i, DirectionSideTile.Right);
                ColorsLeft[i * TileSideVoxels + y] = GetVoxelColor(y, i, DirectionSideTile.Left);
                ColorsBack[i * TileSideVoxels + y] = GetVoxelColor(y, i, DirectionSideTile.Back);
                // ColorsTop[i * TileSideVoxels + y] = GetVoxelColor(y, i, DirectionSideTile.Top);
                // ColorsBottom[i * TileSideVoxels + y] = GetVoxelColor(y, i, DirectionSideTile.Bottom);
            }
        }
    }

    public void Rotate90()
    {
        // var TileSideVoxels = meshConfig.sOVoxelData.Bounds.x;
        transform.Rotate(0, 90, 0);

        Voxel[] colorsRightNew = new Voxel[TileSideVoxels * TileSideVoxels];
        Voxel[] colorsForwardNew = new Voxel[TileSideVoxels * TileSideVoxels];
        Voxel[] colorsLeftNew = new Voxel[TileSideVoxels * TileSideVoxels];
        Voxel[] colorsBackNew = new Voxel[TileSideVoxels * TileSideVoxels];
        // Voxel[] colorsTopNew = new Voxel[TileSideVoxels * TileSideVoxels];
        // Voxel[] colorsBottomNew = new Voxel[TileSideVoxels * TileSideVoxels];

        for (int row = 0; row < TileSideVoxels; row++)
        {
            for (int column = 0; column < TileSideVoxels; column++)
            {
                // colorsRightNew[row * TileSideVoxels + column] = ColorsForward[row * TileSideVoxels + TileSideVoxels - column - 1];
                // colorsForwardNew[row * TileSideVoxels + column] = ColorsLeft[row * TileSideVoxels + column];
                // colorsLeftNew[row * TileSideVoxels + column] = ColorsBack[row * TileSideVoxels + TileSideVoxels - column - 1];
                // colorsBackNew[row * TileSideVoxels + column] = ColorsRight[row * TileSideVoxels + column];
                // // TODO
                colorsForwardNew[row * TileSideVoxels + column] = ColorsRight[row * TileSideVoxels + column];
                colorsRightNew[row * TileSideVoxels + column] = ColorsBack[row * TileSideVoxels + TileSideVoxels - column - 1];
                colorsBackNew[row * TileSideVoxels + column] = ColorsLeft[row * TileSideVoxels + column];
                colorsLeftNew[row * TileSideVoxels + column] = ColorsForward[row * TileSideVoxels + TileSideVoxels - column - 1];
            }
        }
        // colorsRightNew = ColorsForward;
        // colorsForwardNew = ColorsLeft;
        // colorsLeftNew = ColorsBack;
        // colorsBackNew = ColorsRight;

        ColorsRight = colorsRightNew;
        ColorsForward = colorsForwardNew;
        ColorsLeft = colorsLeftNew;
        ColorsBack = colorsBackNew;
    }

    public Voxel GetVoxelColor(int verticalLayer, int horizontalOffset, DirectionSideTile direction)
    {
        Voxel vox = default;

        if (direction == DirectionSideTile.Forward)
        {
            vox = voxelMeshRender.GetVoxel(0, new Vector3Int(horizontalOffset, verticalLayer, 0));
        }
        else if (direction == DirectionSideTile.Right)
        {
            vox = voxelMeshRender.GetVoxel(0, new Vector3Int(0, verticalLayer,  horizontalOffset));
        }
        else if (direction == DirectionSideTile.Back)
        {
            vox = voxelMeshRender.GetVoxel(0, new Vector3Int(horizontalOffset, verticalLayer, TileSideVoxels - 1)); // TileSideVoxels - horizontalOffset - 1
        }
        else if (direction == DirectionSideTile.Left)
        {
            vox = voxelMeshRender.GetVoxel(0, new Vector3Int(TileSideVoxels - 1, verticalLayer, horizontalOffset)); // TileSideVoxels - horizontalOffset - 1
        }
        // Color32 voxColor = (Color32)vox.color;
        // var meshCollider = GetComponentInChildren<MeshCollider>();

        // float vox = VoxelSize;
        // float half = VoxelSize / 2;

        // Vector3 rayStart;
        // Vector3 rayDir;
        // if (direction == Direction.Right)
        // {
        //     rayStart = meshCollider.bounds.min +
        //                new Vector3(-half, 0, half + horizontalOffset * vox);
        //     rayDir = Vector3.right;
        // }
        // else if (direction == Direction.Forward)
        // {
        //     rayStart = meshCollider.bounds.min +
        //                new Vector3(half + horizontalOffset * vox, 0, -half);
        //     rayDir = Vector3.forward;
        // }
        // else if (direction == Direction.Left)
        // {
        //     rayStart = meshCollider.bounds.max +
        //                new Vector3(half, 0, -half - (TileSideVoxels - horizontalOffset - 1) * vox);
        //     rayDir = Vector3.left;
        // }
        // else if (direction == Direction.Back)
        // {
        //     rayStart = meshCollider.bounds.max +
        //                new Vector3(-half - (TileSideVoxels - horizontalOffset - 1) * vox, 0, half);
        //     rayDir = Vector3.back;
        // }
        // else
        // {
        //     throw new ArgumentException("Wrong direction value, should be Direction.left/right/back/forward",
        //         nameof(direction));
        // }

        // rayStart.y = meshCollider.bounds.min.y + half + verticalLayer * vox;

        // //Debug.DrawRay(rayStart, direction * .1f, Color.blue, 2);

        // if (Physics.Raycast(new Ray(rayStart, rayDir), out RaycastHit hit, vox))
        // {
        //     byte colorIndex = (byte)(hit.textureCoord.x * 256);

        //     if (colorIndex == 0) Debug.LogWarning("Found color 0 in mesh palette, this can cause conflicts");

        //     return colorIndex;
        // }

        return vox;
    }
}

public enum DirectionSideTile
{
    Left,
    Right,
    Back,
    Forward,
    Top,
    Bottom
}