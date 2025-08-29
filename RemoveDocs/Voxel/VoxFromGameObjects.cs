using System;
using System.Collections.Generic;
using UnityEngine;

public class VoxFromGameOBjects : MonoBehaviour
{
    public SOVoxelData input;
    public Material material;
    public Mesh mesh;
    private VoxelObjectData[] modelList;
    public void Start()
    {
        modelList = new VoxelObjectData[10];
        for (int i = 0; i < 10; i++)
        {
            modelList[i] = CreateGameObject(input.voxels, new Vector3(UnityEngine.Random.Range(i, i * 10), 0.5f, UnityEngine.Random.Range(i, i * 10)));
        }
    }

    #region Functions
    private VoxelObjectData CreateGameObject(List<Vector3Int> points, Vector3 startPosition, float size = 1)
    {
        GameObject[] listGameObjects = new GameObject[points.Count];
        for (int i = 0; i < points.Count; i++)
        {
            Vector3 position = input.voxels[i];
            // Создаем куб для каждого вокселя
            GameObject voxel = GameObject.CreatePrimitive(PrimitiveType.Cube);
            voxel.transform.localScale = Vector3.one * size;
            voxel.transform.position = new Vector3(position.x * size, position.y * size, position.z * size) + startPosition;
            voxel.transform.parent = this.transform;
            voxel.gameObject.isStatic = true;
            // Добавляем материал
            voxel.GetComponent<Renderer>().material = material; //_materials[UnityEngine.Random.Range(0, _materials.Count)];
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

            for (int j = 0; j < vertices.Length; j++)
                colors[j] = UnityEngine.Random.ColorHSV(); //Color.Lerp(Color.red, Color.green, vertices[i].y);

            // assign the array of colors to the Mesh.
            // Debug.Log($"vertices.Length={vertices.Length}, mesh.colors {string.Join(",", mesh.colors.ToArray())}, colors {string.Join(",", colors.ToArray())}");
            mesh.colors = colors;
            voxel.GetComponent<MeshFilter>().sharedMesh = mesh;

            listGameObjects[i] = voxel.gameObject;

        }

        StaticBatchingUtility.Combine(listGameObjects, gameObject);
        VoxelObjectData voxelObjectData = new()
        {
            gameObjects = listGameObjects,
        };
        return voxelObjectData;
    }
    #endregion

}
[Serializable]
public struct VoxelObjectData
{
    public GameObject[] gameObjects;
}