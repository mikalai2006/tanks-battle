using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SOVoxelData", menuName = "SO/VoxelData")]
public class SOVoxelData : ScriptableObject
{
    public List<SubmeshesData> groups;
    public List<Vector3Int> voxels;
    public List<Color> colors;
    public float sizeVoxel;
    public Vector3 Pivot;
    public Vector3Int Bounds;
    public Vector3Int GlobalSize;
    public Vector3 GlobalPosition;
    public Quaternion GlobalRotation;
    public Vector3Int LocalSize;
    public Vector3 LocalPosition;
    public Quaternion LocalRotation;
    
    [Tooltip("Меш для начальной загрузки (позволяет использовать все возможности GPU Instancing до начала разрушения)")]
    public Mesh startMesh;
}


[Serializable]
public struct SubmeshesData
{
    public Color color;
    public List<Vector3Int> voxels;
}