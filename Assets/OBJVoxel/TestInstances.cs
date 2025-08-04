using UnityEngine;

public class TestInstances : MonoBehaviour
{
    public SOVoxelData data;

    public Material[] materials;

    public Mesh mesh;

    void Start()
    {
        // StaticBatchingUtility.Combine(gameObjects, gameObject);
        for (int i = 0; i < 10; i++)
        {
            CreateModel(new Vector3(UnityEngine.Random.Range(0, 50), 0, UnityEngine.Random.Range(0, 50)));
        }
    }

    private void CreateModel(Vector3 position)
    {
        GameObject obj = new GameObject("block");
        obj.transform.SetPositionAndRotation(position, Quaternion.identity);
        obj.transform.SetParent(transform);

        for (int i = 0; i < data.voxels.Count; i++)
        {
            GameObject voxel = new GameObject("Voxel" + i, typeof(MeshRenderer), typeof(MeshFilter));
            // voxel.isStatic = true;
            voxel.AddComponent<GPUInstanceEnabler>();
            voxel.transform.SetParent(obj.transform);
            voxel.transform.position = data.voxels[i];
            voxel.transform.localScale = new Vector3(1, 1, 1);
            voxel.GetComponent<MeshFilter>().sharedMesh = mesh;
            voxel.GetComponent<MeshRenderer>().material = materials[0];
        }

    }
}
