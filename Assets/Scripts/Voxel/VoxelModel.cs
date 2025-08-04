// using UnityEngine;
// using System.Collections.Generic;
// using UnityEditor;
// using System.Globalization;

// namespace Mikalai2006.Voxel {
//     public class VoxelModel : MonoBehaviour
//     {
//         public GameObject cubePrefab;
//         public WorldManager WorldManager;
//         public TextAsset txtOBJFile;
//         public float scale = 1.0f; // Adjust scale as needed
//         public string path;


//         void Start()
//         {
//             // txtOBJFile = AssetDatabase.LoadAssetAtPath<TextAsset>("Resources/TestPanzer-6-muzzleOBJ.obj");//Resources.Load<TextAsset>("TestPanzer-6-muzzleOBJ"); // Assuming it's in Resources folder
//         //  if (txtOBJFile != null)
//         //  {
//         //      string objData = objTextAsset.text;
//         //      // Process the OBJ data (e.g., parse it using a library or custom code)
//         //  }

//             if (cubePrefab == null)
//             {
//                 Debug.LogError("Cube prefab is not assigned!");
//                 return;
//             }

//             if (txtOBJFile == null)
//             {
//                 Debug.LogError("OBJ file is not assigned!");
//                 return;
//             }

//             List<Vector3> vertices = new List<Vector3>();
//             List<int> triangles = new List<int>();
//             List<Vector2> uvs = new List<Vector2>();

//             // Parse the .obj file
//             string objText = txtOBJFile.text;
//             string[] lines = objText.Split('\n');

//             foreach (string line in lines)
//             {
//                 string[] parts = line.Trim('\r', '\n', ' ').Split(' ');

//                 if (parts.Length > 0)
//                 {
//                     switch (parts[0])
//                     {
//                         case "v": // Vertex data
//                 Debug.Log($"[{parts[1]}],[{parts[2]}],[{parts[3]}]");
//                             float x = float.Parse(parts[1], CultureInfo.InvariantCulture);
//                             float y = float.Parse(parts[2], CultureInfo.InvariantCulture);
//                             float z = float.Parse(parts[3], CultureInfo.InvariantCulture);
//                             vertices.Add(new Vector3(x, y, z) * scale);
//                             break;
//                         // case "vt": // UV data (optional, you can ignore if not needed)
//                         //            float u = float.Parse(parts[1], CultureInfo.InvariantCulture);
//                         //            float v = float.Parse(parts[2], CultureInfo.InvariantCulture);
//                         //            uvs.Add(new Vector2(u, v));
//                         //     break;
//                         // case "f": // Face data
//                         //     for (int i = 1; i < parts.Length; i++)
//                         //     {
//                         //         string[] indices = parts[i].Split('/');
//                         //         int vertexIndex = int.Parse(indices[0]) - 1;
//                         //         triangles.Add(vertexIndex);
//                         //     }
//                         //     break;
//                     }
//                 }
//             }

//             WorldManager.CreateContainer(vertices.ToArray(), scale);
//             //Instantiate cubes at each vertex
//             // GameObject[] voxelsList = new GameObject[vertices.Count];;
//             // int i = 0;
//             // foreach (Vector3 vertex in vertices)
//             // {
//             //     var a = Instantiate(cubePrefab, vertex, Quaternion.identity, transform);
//             //     a.isStatic = true;
//             //     voxelsList[i] = a;
//             //     i++;
//             // }

//             // StaticBatchingUtility.Combine(voxelsList, this.gameObject);

//             Debug.Log("Voxel model loaded and instantiated successfully!");
//         }
//     }
// }