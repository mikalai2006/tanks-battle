using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class VoxelChunkRenderer : MonoBehaviour
{
    [SerializeField] private int ChunkWidth = 32;
    [SerializeField] private int ChunkHeight = 32;

    public int[,,] Blocks;

    private List<Vector3> vertices = new List<Vector3>();
    private List<int> triangles = new List<int>();

    [SerializeField] private SOVoxelData sOVoxelData;

    void Start()
    {
        float startTime = Time.realtimeSinceStartup;

        Blocks = new int[ChunkWidth, ChunkHeight, ChunkWidth];
        for (int j = 0; j < sOVoxelData.voxels.Count; j++)
        {
            Vector3Int voxPos = sOVoxelData.voxels[j];
            Blocks[voxPos.x, voxPos.y, voxPos.z] = 1;
        }

        Mesh chunkMesh = new Mesh();

        // Blocks[0, 0, 0] = 1;
        // Blocks[0, 0, 1] = 1;
        // Blocks[0, 1, 0] = 1;

        for (int y = 0; y < ChunkHeight; y++)
        {
            for (int x = 0; x < ChunkWidth; x++)
            {
                for (int z = 0; z < ChunkWidth; z++)
                {
                    GenerateBlock(x, y, z);
                }
            }
        }

        chunkMesh.vertices = vertices.AsParallel().ToArray();
        chunkMesh.triangles = triangles.AsParallel().ToArray();

        chunkMesh.RecalculateBounds();
        chunkMesh.RecalculateNormals();

        GetComponent<MeshFilter>().mesh = chunkMesh;

        Debug.Log($"Time generate mesh: {(Time.realtimeSinceStartup - startTime) * 1000f} ms");
    }

    private void GenerateBlock(int x, int y, int z)
    {
        Vector3Int blockPosition = new Vector3Int(x, y, z);

        if (GetBlockAtPosition(blockPosition) == 0)
        {
            return;
        }

        if (GetBlockAtPosition(blockPosition + Vector3Int.right) == 0) GenerateRightSide(blockPosition);
        if (GetBlockAtPosition(blockPosition + Vector3Int.left) == 0) GenerateLeftSide(blockPosition);
        if (GetBlockAtPosition(blockPosition + Vector3Int.forward) == 0) GenerateFrontSide(blockPosition);
        if (GetBlockAtPosition(blockPosition + Vector3Int.back) == 0) GenerateBackSide(blockPosition);
        if (GetBlockAtPosition(blockPosition + Vector3Int.up) == 0) GenerateTopSide(blockPosition);
        if (GetBlockAtPosition(blockPosition + Vector3Int.down) == 0) GenerateBottomSide(blockPosition);
    }

    private int GetBlockAtPosition(Vector3Int blockPosition)
    {
        if (blockPosition.x >= 0 && blockPosition.x < ChunkWidth &&
            blockPosition.y >= 0 && blockPosition.y < ChunkHeight &&
            blockPosition.z >= 0 && blockPosition.z < ChunkWidth)
        {
            return Blocks[blockPosition.x, blockPosition.y, blockPosition.z];
        } else {
            return 0;
        }
    }
    private void GenerateRightSide(Vector3Int blockPosition)
    {
        vertices.Add(new Vector3(1, 0, 0) + blockPosition);
        vertices.Add(new Vector3(1, 1, 0) + blockPosition);
        vertices.Add(new Vector3(1, 0, 1) + blockPosition);
        vertices.Add(new Vector3(1, 1, 1) + blockPosition);

        AddLastVerticesSquare();
    }
    private void GenerateLeftSide(Vector3Int blockPosition)
    {
        vertices.Add(new Vector3(0, 0, 0) + blockPosition);
        vertices.Add(new Vector3(0, 0, 1) + blockPosition);
        vertices.Add(new Vector3(0, 1, 0) + blockPosition);
        vertices.Add(new Vector3(0, 1, 1) + blockPosition);
        AddLastVerticesSquare();
    }
    private void GenerateFrontSide(Vector3Int blockPosition)
    {
        vertices.Add(new Vector3(0, 0, 1) + blockPosition);
        vertices.Add(new Vector3(1, 0, 1) + blockPosition);
        vertices.Add(new Vector3(0, 1, 1) + blockPosition);
        vertices.Add(new Vector3(1, 1, 1) + blockPosition);
        AddLastVerticesSquare();
    }
    private void GenerateBackSide(Vector3Int blockPosition)
    {
        vertices.Add(new Vector3(0, 0, 0) + blockPosition);
        vertices.Add(new Vector3(0, 1, 0) + blockPosition);
        vertices.Add(new Vector3(1, 0, 0) + blockPosition);
        vertices.Add(new Vector3(1, 1, 0) + blockPosition);
        AddLastVerticesSquare();
    }
    private void GenerateTopSide(Vector3Int blockPosition)
    {
        vertices.Add(new Vector3(0, 1, 0) + blockPosition);
        vertices.Add(new Vector3(0, 1, 1) + blockPosition);
        vertices.Add(new Vector3(1, 1, 0) + blockPosition);
        vertices.Add(new Vector3(1, 1, 1) + blockPosition);

        AddLastVerticesSquare();
    }
    private void GenerateBottomSide(Vector3Int blockPosition)
    {
        vertices.Add(new Vector3(0, 0, 0) + blockPosition);
        vertices.Add(new Vector3(1, 0, 0) + blockPosition);
        vertices.Add(new Vector3(0, 0, 1) + blockPosition);
        vertices.Add(new Vector3(1, 0, 1) + blockPosition);

        AddLastVerticesSquare();
    }
    private void AddLastVerticesSquare()
    {
        triangles.Add(vertices.Count - 4);
        triangles.Add(vertices.Count - 3);
        triangles.Add(vertices.Count - 2);

        triangles.Add(vertices.Count - 3);
        triangles.Add(vertices.Count - 1);
        triangles.Add(vertices.Count - 2);
    }
}
