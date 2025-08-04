using System.Collections.Generic;
using UnityEditor;
    using UnityEngine;

public static class MeshSaver
{
    [MenuItem("CONTEXT/MeshFilter/Save Mesh...")]
    private static void SaveMesh(MenuCommand command)
    {
        MeshFilter meshFilter = command.context as MeshFilter;
        Mesh filter = meshFilter.mesh;
        if (filter == null || filter == null) return;

        string path = EditorUtility.SaveFilePanelInProject("Save Mesh", filter.name, "asset", "Save the Mesh asset.");
        if (string.IsNullOrEmpty(path)) return;

        // filter.Optimize();
        // filter.OptimizeReorderVertexBuffer();

        // filter.RecalculateNormals();

        // filter.RecalculateBounds();

        // filter.UploadMeshData(false);

        // meshFilter.mesh = filter;

        // Mesh mesh = OptimizeMesh(meshFilter.sharedMesh);

        AssetDatabase.CreateAsset(meshFilter.sharedMesh, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    public static Mesh OptimizeMesh(Mesh mesh)
    {
        if (mesh == null)
        {
            Debug.LogError("Mesh is null!");
            return null;
        }

        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;
        Vector3[] normals = mesh.normals;
        Vector2[] uvs = mesh.uv;

        Dictionary<Vector3, int> uniqueVertices = new Dictionary<Vector3, int>();
        List<Vector3> newVertices = new List<Vector3>();
        List<int> newTriangles = new List<int>();
        List<Vector3> newNormals = new List<Vector3>();
        List<Vector2> newUVs = new List<Vector2>();


        for (int i = 0; i < triangles.Length; i++)
        {
            int index = triangles[i];
            Vector3 vertex = vertices[index];
            if (uniqueVertices.ContainsKey(vertex))
            {
                newTriangles.Add(uniqueVertices[vertex]);
            }
            else
            {
                newVertices.Add(vertex);
                newNormals.Add(normals[index]);
                newUVs.Add(uvs[index]);
                int newIndex = newVertices.Count - 1;
                uniqueVertices.Add(vertex, newIndex);
                newTriangles.Add(newIndex);
            }
        }

        mesh.Clear();
        mesh.vertices = newVertices.ToArray();
        mesh.triangles = newTriangles.ToArray();
        mesh.normals = newNormals.ToArray();
        mesh.uv = newUVs.ToArray();
        mesh.RecalculateBounds();
        // mesh.RecalculateNormals();
        // mesh.UploadMeshData(false);

        return mesh;
        }
    }