using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class VoxelTest : MonoBehaviour
{
    public int width = 10;
    public int height = 10;
    public int depth = 10;
    public float voxelSize = 1f;
    public List<Material> _materials;

    private GameObject[,,] voxels;
    private GameObject[] voxelsList;

    void Start()
    {
        voxels = new GameObject[width, height, depth];
        voxelsList = new GameObject[1000];
        GenerateVoxels();
    }

    void GenerateVoxels()
    {
        int f = 0;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < depth; z++)
                {
                    // Создаем куб для каждого вокселя
                    GameObject voxel = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    voxel.transform.localScale = Vector3.one * voxelSize;
                    voxel.transform.position = new Vector3(x * voxelSize, y * voxelSize, z * voxelSize);
                    voxel.transform.parent = this.transform;
                    voxel.gameObject.isStatic = true;
                    // Добавляем материал
                    voxel.GetComponent<Renderer>().material = _materials[UnityEngine.Random.Range(0, _materials.Count)];
                    // voxel.GetComponent<Renderer>().sharedMaterial.color = Random.ColorHSV();

                    // // Create a new MaterialPropertyBlock
                    // MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();

                    // // Set a random color in the MaterialPropertyBlock
                    // propertyBlock.SetColor("_Color", Random.ColorHSV());

                    // // Apply the MaterialPropertyBlock to the GameObject
                    // voxel.GetComponent<MeshRenderer>().SetPropertyBlock(propertyBlock);

                    Mesh mesh = voxel.GetComponent<MeshFilter>().sharedMesh;
                    Vector3[] vertices = mesh.vertices;

                    // create new colors array where the colors will be created.
                    Color[] colors = new Color[vertices.Length];

                    for (int i = 0; i < vertices.Length; i++)
                        colors[i] = Random.ColorHSV(); //Color.Lerp(Color.red, Color.green, vertices[i].y);

                    // assign the array of colors to the Mesh.
                    // Debug.Log($"vertices.Length={vertices.Length}, mesh.colors {string.Join(",", mesh.colors.ToArray())}, colors {string.Join(",", colors.ToArray())}");
                    mesh.colors = colors;
                    voxel.GetComponent<MeshFilter>().sharedMesh = mesh;

                    voxels[x, y, z] = voxel;


                    voxelsList[f] = voxel.gameObject;
                f++;
                }
            }
        }

        StaticBatchingUtility.Combine(voxelsList, this.gameObject);
    }
}