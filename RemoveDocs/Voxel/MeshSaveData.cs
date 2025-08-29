    // using UnityEngine;
    // using System.IO;
    // using System;

    // [Serializable]
    // public class MeshSaveData
    // {
    //     public Vector3[] vertices;
    //     public int[] triangles;
    //     public Vector3[] normals;
    //     // Add other mesh properties as needed (e.g., uvs, colors)

    //     public MeshSaveData(Mesh mesh)
    //     {
    //         this.vertices = mesh.vertices;
    //         this.triangles = mesh.triangles;
    //         this.normals = mesh.normals;
    //     }

    //     public Mesh ToMesh()
    //     {
    //         Mesh mesh = new Mesh();
    //         mesh.vertices = this.vertices;
    //         mesh.triangles = this.triangles;
    //         mesh.normals = this.normals;
    //         mesh.RecalculateBounds();
    //         return mesh;
    //     }
    // }

    // public static class MeshSaverRuntime
    // {
    //     public static void SaveMeshToJson(Mesh mesh, string filename)
    //     {
    //         MeshSaveData data = new MeshSaveData(mesh);
    //         string json = JsonUtility.ToJson(data);
    //         string path = Path.Combine(Application.persistentDataPath, filename + ".json");
    //         File.WriteAllText(path, json);
    //         Debug.Log("Mesh saved to: " + path);
    //     }

    //     public static Mesh LoadMeshFromJson(string filename)
    //     {
    //         string path = Path.Combine(Application.persistentDataPath, filename + ".json");
    //         if (File.Exists(path))
    //         {
    //             string json = File.ReadAllText(path);
    //             MeshSaveData data = JsonUtility.FromJson<MeshSaveData>(json);
    //             return data.ToMesh();
    //         }
    //         return null;
    //     }
    // }