using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

[System.Serializable]
public struct VoxelJobData
{
    public float3 position;
    // public NativeList<float3> vertices;
    // public NativeList<uint> triangles;
    // public NativeList<float2> uvs;
    // public NativeList<float2> uvs2;
    // public NativeList<Color> colors;

    public void Destroy()
    {
    //     vertices.Dispose();
    //     triangles.Dispose();
    //     uvs.Dispose();
    //     uvs2.Dispose();
    //     colors.Dispose();
    }
}
