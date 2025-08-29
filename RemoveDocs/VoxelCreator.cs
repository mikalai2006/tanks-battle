#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class VoxelCreator : MonoBehaviour
{
    public CubeVoxels CubeVoxels;
    // public MeshFilter inputMesh;
    public string nameFile;
    public TypeDetailVehicle typeDetailVehicle;
    public Button button;

    void Start()
    {
        // InitMesh();

        button.onClick.AddListener(GenerateVoxels);
    }

    // public void InitMesh()
    // {
    //     oMeshFilter = GetComponent<MeshFilter>();
    //     oMesh = oMeshFilter.sharedMesh; //1

    //     cMesh = new Mesh(); //2
    //     cMesh.name = "clone";
    //     cMesh.vertices = oMesh.vertices;
    //     cMesh.triangles = oMesh.triangles;
    //     cMesh.normals = oMesh.normals;
    //     cMesh.uv = oMesh.uv;
    //     oMeshFilter.mesh = cMesh;  //3

    //     vertices = cMesh.vertices; //4
    //     triangles = cMesh.triangles;
    //     isCloned = true;
    //     Debug.Log("Init & Cloned");
    // }

    public void GenerateVoxels()
    {
        Dictionary<Vector3Int, Color> points = new Dictionary<Vector3Int, Color>();
        // Debug.Log($"Count Vertices = {GetComponent<MeshFilter>().sharedMesh.vertexCount}/{GetComponent<MeshFilter>().sharedMesh.vertices.Length}/ cubes = {CubeVoxels.listCubesGameObject.Count}");
        foreach (GameObject obj in CubeVoxels.listCubesGameObject)
        {
            if (GetComponent<Collider>().bounds.Intersects(obj.GetComponent<Collider>().bounds))
            {


                // // OverlapSphere
                float radius = 1f / Mathf.Min(CubeVoxels.size.x, CubeVoxels.size.y, CubeVoxels.size.z) / 2.01f;

                // // Debug.Log($"radius = {radius}");

                // Collider[] hitColliders = Physics.OverlapSphere(obj.transform.position, radius);

                // if (hitColliders.Length > 0)
                // {
                //     // Handle the overlap events for each collider in hitColliders
                //     foreach (Collider collider in hitColliders)
                //     {
                //         // Check if the collider is a mesh collider and perform desired action
                //         if (collider.GetComponent<MeshCollider>() != null)
                //         {
                //             // Debug.Log("Mesh collider hit: " + collider.name);
                //             // Perform your specific logic here, e.g., destroy, damage, etc.
                //             obj.gameObject.SetActive(true);
                //         }
                //         else
                //         {
                //              obj.gameObject.SetActive(false);
                //         }
                //     }
                // }


                // Raycast
                Physics.queriesHitBackfaces = true;
                Vector3 directionRay = obj.transform.forward;
                Vector3 objPosition = obj.transform.position;
                Vector3 offsetPosition = Vector3.zero;//-Vector3.right * radius;
                Color colorRay = Color.magenta;
                if (objPosition.z < 0)
                {
                    directionRay = -directionRay;
                    colorRay = Color.blue;
                }

                if (objPosition.x < 0)
                {
                    offsetPosition = -offsetPosition;
                }

                if (objPosition.x <= -CubeVoxels.size.x / 5)
                {
                    directionRay = -obj.transform.right;
                    colorRay = Color.green;
                }
                else if (objPosition.x >= CubeVoxels.size.x / 5)
                {
                    directionRay = obj.transform.right;
                    colorRay = Color.yellow;
                }
                objPosition += offsetPosition;
                Ray ray = new Ray(objPosition, directionRay);
                RaycastHit hit;
                // if (hit.collider == null)
                // {
                //     ray = new Ray(objPosition + new Vector3(radius, radius, radius), directionRay);
                //     Physics.Raycast(ray, out hit, 2f, 1 << 8);
                // }

                // if (hit.collider == null)
                // {
                //     ray = new Ray(objPosition + new Vector3(-radius, -radius, -radius), directionRay);
                //     Physics.Raycast(ray, out hit, 2f, 1 << 8);
                // }

                if (Physics.Raycast(ray, out hit, CubeVoxels.size.x, 1 << 8))
                {
                    Debug.DrawLine(objPosition, hit.point, colorRay, 60);
                    // Debug.Log("Raycast hit mesh: " + hit.collider.name + " at point: " + hit.point + " " + hit);
                    obj.gameObject.SetActive(true);

                    Color colorVoxel = Color.red;

                    Texture2D texture = hit.transform.GetComponent<MeshRenderer>().material.mainTexture as Texture2D;
                    // Debug.Log($"texture = {texture.name}");
                    if (texture != null)
                    {
                        // Ensure texture is readable
                        // (Texture Import Settings -> Read/Write Enabled must be checked)
                        if (texture.isReadable)
                        {
                            Vector2 uv = hit.textureCoord;
                            Color pixelColor = texture.GetPixelBilinear(uv.x, uv.y);
                            // Debug.Log("Color at point: " + pixelColor.ToHexString());
                            colorVoxel = pixelColor;
                        }
                        else
                        {
                            Debug.LogError("Texture is not readable. Enable 'Read/Write Enabled' in Texture Import Settings.");
                        }
                    }

                    points.Add(Vector3Int.FloorToInt(obj.transform.position), colorVoxel);
                }
                else
                {
                    // Debug.DrawLine(objPosition, directionRay, Color.red, 60);
                    obj.gameObject.SetActive(false);
                }
                Physics.queriesHitBackfaces = false;

                // RaycastAll
                // Physics.queriesHitBackfaces = true;
                // RaycastHit[] hits;
                // float distance = 100.0f;
                // Color colorLine = Color.magenta;
                // colorLine.a = 0.2f;
                // Color colorNoHits = Color.red;
                // colorNoHits.a = 0.2f;
                // hits = Physics.RaycastAll(obj.transform.position - Vector3.up * radius, obj.transform.up, distance, 1 << 8);

                // Debug.Log($"hits count = {hits.Length} for {obj.transform.position - Vector3.up * radius}");
                // if (hits.Length > 0)
                // {
                //     if (hits.Length % 2 != 0)
                //     {
                //         Debug.DrawLine(obj.transform.position - Vector3.up * radius, hits[hits.Length - 1].point, colorLine, 30);
                //         obj.gameObject.SetActive(true);
                //     }
                //     else
                //     {
                //         Debug.DrawLine(obj.transform.position - Vector3.up * radius, hits[hits.Length - 1].point, colorNoHits, 30);
                //         obj.gameObject.SetActive(false);
                //     }
                // }
                // else
                // {
                //     Debug.DrawLine(obj.transform.position - Vector3.up * radius, obj.transform.up * distance, colorNoHits, 30);
                // }
                // Physics.queriesHitBackfaces = false;
                // for (int i = 0; i < hits.Length; i++)
                // {
                //     RaycastHit hit = hits[i];
                //     // Renderer rend = hit.transform.GetComponent<Renderer>();
                //     // if (rend)
                //     // {
                //     //     // Change the material of all hit colliders
                //     //     // to use a transparent shader.
                //     //     rend.material.shader = Shader.Find("Transparent/Diffuse");
                //     //     Color tempColor = rend.material.color;
                //     //     tempColor.a = 0.3F;
                //     //     rend.material.color = tempColor;
                //     // }
                // }

            }
            else
            {

                obj.gameObject.SetActive(false);
            }
        }

        Debug.Log($"Generate voxel by of {points.Count} voxel`s");

        SOVoxelData asset = ScriptableObject.CreateInstance<SOVoxelData>();

        asset.voxels = points.Keys.ToList();
        asset.colors = points.Values.ToList();
        asset.sizeVoxel = 1f / Mathf.Min(CubeVoxels.size.x, CubeVoxels.size.y, CubeVoxels.size.z);

        string path = AssetDatabase.GenerateUniqueAssetPath($"Assets/SO/{typeDetailVehicle.ToString()}/{nameFile}.asset");

        AssetDatabase.CreateAsset(asset, path);
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;


        // MeshFilter[] meshFilters = CubeVoxels.GetComponentsInChildren<MeshFilter>();
        // CombineInstance[] combine = new CombineInstance[meshFilters.Length];
        // int i = 0;
        // while (i < meshFilters.Length)
        // {
        //     combine[i].mesh = meshFilters[i].mesh;
        //     i++;
        // }
        // Mesh mesh = new Mesh();
        // mesh.CombineMeshes(combine);
        // string pathMesh = AssetDatabase.GenerateUniqueAssetPath($"Assets/SO/{typeDetailVehicle.ToString()}/{nameFile}_mesh.asset");
        // AssetDatabase.CreateAsset(mesh, pathMesh);
        // AssetDatabase.SaveAssets();
        
    }
}

[Serializable]
public enum TypeDetailVehicle
{
    Body = 0,
    Tower = 1,
    Caterpillars = 2,
    Building = 3,
}

#endif