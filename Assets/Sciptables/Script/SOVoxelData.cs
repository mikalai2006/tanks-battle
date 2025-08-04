using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SOVoxelData", menuName = "SO/VoxelData")]
public class SOVoxelData : ScriptableObject
{
    public List<SubmeshesData> groups;
    public List<Vector3> voxels;
    public List<Color> colors;
    public float sizeVoxel;
    public Vector3 GlobalSize;
    public Vector3 GlobalPosition;
    // public Vector3 GlobalRotation;
    public Vector3 LocalSize;
    public Vector3 LocalPosition;
    public UnityEngine.Rendering.ShadowCastingMode shadowCastingMode;
}


[Serializable]
public struct SubmeshesData
{
    public Color color;
    public List<Vector3> voxels;
}