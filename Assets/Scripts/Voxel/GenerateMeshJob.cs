using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

namespace Mikalai2006.Voxel {
    [BurstCompile]
    struct GenerateMeshDataJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<Voxel> voxels;
        // public int Resolution;
        [WriteOnly] public NativeQueue<float3>.ParallelWriter voxelJobDatas;
        // [WriteOnly] public NativeList<Vector3> Vertices;
        // [WriteOnly] public NativeList<Vector2> UVs;
        // [WriteOnly] public NativeList<Vector2> UVs2;
        // [WriteOnly] public NativeList<int> Triangles;
        // [WriteOnly] public NativeList<Color> Colors;
        // [ReadOnly] public NativeArray<Vector3> voxelFaceChecks;
        // [ReadOnly] public NativeArray<Vector2> voxelUVs;
        // [ReadOnly] public NativeArray<int> voxelTris;
        // [ReadOnly] public NativeArray<int> voxelVertexIndex;
        // [ReadOnly] public NativeArray<Vector3> voxelVertices;
        [ReadOnly] public int width;

        public int To1D(int x, int y, int width)
        {
            return y * width + x;
        }

        public void Execute(int index)
        {
            NativeArray<Vector3> voxelFaceChecks = new NativeArray<Vector3>(6, Allocator.Temp);
            voxelFaceChecks[0] = new Vector3(0, 0, -1);
            voxelFaceChecks[0] = new Vector3(0, 0, 1);
            voxelFaceChecks[0] = new Vector3(-1, 0, 0);
            voxelFaceChecks[0] = new Vector3(1, 0, 0);
            voxelFaceChecks[0] = new Vector3(0, -1, 0);
            voxelFaceChecks[0] = new Vector3(0, 1, 0);

            // // Calculate vertex position, normal, UV based on index 'i' and Resolution
            // // Example: Simple grid generation
            // int x = i % (Resolution + 1);
            // int y = i / (Resolution + 1);

            // Vertices[i] = new Vector3(x, 0, y); // Example: Flat plane
            // Normals[i] = Vector3.up;
            // UVs[i] = new Vector2((float)x / Resolution, (float)y / Resolution);

            // // Calculate triangles (only for first vertex of each quad)
            // if (x < Resolution && y < Resolution)
            // {
            //     int baseIndex = y * (Resolution + 1) + x;
            //     int triIndex = (y * Resolution + x) * 6;

            //     // First triangle of quad
            //     Triangles[triIndex + 0] = (uint)baseIndex;
            //     Triangles[triIndex + 1] = (uint)(baseIndex + Resolution + 1);
            //     Triangles[triIndex + 2] = (uint)(baseIndex + 1);

            //     // Second triangle of quad
            //     Triangles[triIndex + 3] = (uint)(baseIndex + 1);
            //     Triangles[triIndex + 4] = (uint)(baseIndex + Resolution + 1);
            //     Triangles[triIndex + 5] = (uint)(baseIndex + Resolution + 2);
            // }
            var voxel = voxels[index];
            // Проверяйте только сплошные блоки.
            if (voxel.isSolid)
            {
                // var data = new VoxelJobData()
                // {
                //     position = voxel.position,
                // };
                // NativeList<float3> vertices = new NativeList<float3>(Allocator.Temp);
                // NativeList<uint> triangles = new NativeList<uint>(Allocator.Temp);
                // NativeList<float2> uvs = new NativeList<float2>(Allocator.Temp);
                // NativeList<float2> uvs2 = new NativeList<float2>(Allocator.Temp);
                // NativeList<Color> colors = new NativeList<Color>(Allocator.Temp);


                var voxelPosition = voxel.position;
                // var block = voxel;

                // NativeArray<Vector3> faceVertices = new NativeArray<Vector3>(4, Allocator.Temp);
                // NativeArray<Vector2> faceUVs = new NativeArray<Vector2>(4, Allocator.Temp);
                // uint counter = 0;

                // // проверяем есть ли в словаре запись для индекса подсетки.
                // if (!meshData.triangles.ContainsKey(block.IndexSubMesh))
                // {
                //     meshData.triangles[block.IndexSubMesh] = new List<int>();
                // }

                // voxelColor = WorldManager.Instance.WorldColors[block.ID - 1];
                // voxelColorAlpha = voxelColor.color;
                // voxelColorAlpha.a = 1;
                // voxelSmoothness = new Vector2(voxelColor.metallic, voxelColor.smoothness);
                // Iterate over each face direction
                for (int i = 0; i < 6; i++)
                {
                    // Проверьте, есть ли сплошной блок напротив этой грани.
                    Vector3Int positionNeighbour =  Vector3Int.FloorToInt(voxelPosition + voxelFaceChecks[i]);
                    var pos = To1D(positionNeighbour.x, positionNeighbour.y, width);
                    if (pos < 0 || voxels[To1D(positionNeighbour.x, positionNeighbour.y, width)].isSolid)
                    {
                        continue;
                    }
                    voxelJobDatas.Enqueue(voxel.position);
                    // //Draw this face
                    // // Соберите соответствующие вершины из вершин по умолчанию и добавьте позицию блока.
                    // for (int j = 0; j < 4; j++)
                    // {
                    //     faceVertices[j] = voxelVertices[voxelVertexIndex[j + i * 4]] + blockPos;
                    //     faceUVs[j] = voxelUVs[j];
                    // }

                    // for (int j = 0; j < 6; j++)
                    // {
                    //     vertices.Add(faceVertices[voxelTris[j + i * 6]]);
                    //     uvs.Add(faceUVs[voxelTris[j + i * 6]]);
                    //     colors.Add(block.color); //voxelColorAlpha);
                    //     uvs2.Add(new Vector2(0.4f, 0.75f)); // voxelSmoothness

                    //     // if (!meshData.triangles.ContainsKey(block.IndexSubMesh))
                    //     // {
                    //     //     meshData.triangles[block.IndexSubMesh] = new List<int>();
                    //     // }
                    //     // meshData.triangles[block.IndexSubMesh].Add(counter++);
                    //     triangles.Add(counter++);
                    // }
                }


                // vertices.Dispose();
                // triangles.Dispose();
                // uvs.Dispose();
                // uvs2.Dispose();
                // colors.Dispose();
                // faceVertices.Dispose();
                // faceUVs.Dispose();
            }
        }
    }
}


// using Unity.Burst;
// using Unity.Collections;
// using Unity.Jobs;
// using UnityEngine;

// namespace Mikalai2006.Voxel
// {
//     [BurstCompile]
//     public struct GenerateMeshDataJob : IJobParallelFor
//     {
//         // Inputs
//         public int resolution; // Example: for a grid or terrain
//         public float scale;
//         public NativeArray<Vector3> vertices;
//         public NativeArray<int> triangles;
//         public NativeArray<Vector2> uvs;

//         public void Execute(int index)
//         {
//             // Calculate vertex positions, UVs, and triangle indices based on 'index'
//             // For a grid, 'index' could map to a specific quad or vertex.
//             // Example:
//             // int x = index % resolution;
//             // int y = index / resolution;
//             // vertices[index] = new Vector3(x * scale, 0, y * scale);
//             // ... calculate UVs and triangle indices for the corresponding quad
//         }
//     }
// }