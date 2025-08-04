using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class CubeVoxels : MonoBehaviour
{
    public Vector3Int size;
    public Vector3 offset;
    public GameObject prefab;
    [HideInInspector]
    public List<GameObject> listCubesGameObject;
    public Mesh MeshVoxel;
    public Material[] materials;

    private Matrix4x4[] _matrices;
    private Vector3[] _positions;
    private RenderParams _rp;


    // private Container container;
    // public Container Container => container;

    void Start()
    {
        CreateInstance();
        // CreateGPU();
        // CreateBurst();
    }

    // private void CreateBurst()
    // {
    //     var count = size.x * size.y * size.z;
        
    //     _nativePositions = new NativeArray<float3>(count, Allocator.Persistent);
    //     _nativeMatrices = new NativeArray<Matrix4x4>(count, Allocator.Persistent);
    //     _nativeCubeYOffsets = new NativeArray<float>(count, Allocator.Persistent);

    //     // SceneTools.LoopPositions((i, p) =>
    //     // {
    //     //     _nativeCubeYOffsets[i] = p.y;
    //     //     _nativePositions[i] = p;
    //     // });
        
    //     float offsetX = 1f;// / size.x;
    //     float offsetY = 1f;// / size.y;
    //     float offsetZ = 1f;// / size.z;
    //     int yyy = 0;

    //     for (int x = 0; x < size.x; x++)
    //     {
    //         for (int y = 0; y < size.y; y++)
    //         {
    //             for (int z = 0; z < size.z; z++)
    //             {
    //                 _nativeCubeYOffsets[yyy] = .y;
    //                 _nativePositions[yyy] = p;
    //                 var position = new Vector3(offset.x + offsetX * x, offset.y + offsetY * y, offset.z + offsetZ * z);
    //                 _positions[yyy] = position;
    //                 yyy++;
    //             }
    //         }
    //     }


    //     _job = new CubePositionJob
    //     {
    //         Positions = _nativePositions,
    //         YOffsets = _nativeCubeYOffsets
    //     };

    //     _rp = new RenderParams(_material);
    // }

    private void CreateGPU()
    {
        var count = size.x * size.y * size.z;
        _positions = new Vector3[count];

        float offsetX = 1f;// / size.x;
        float offsetY = 1f;// / size.y;
        float offsetZ = 1f;// / size.z;

        int yyy = 0;
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                for (int z = 0; z < size.z; z++)
                {
                    var position = new Vector3(offset.x + offsetX * x, offset.y + offsetY * y, offset.z + offsetZ * z);
                    _positions[yyy] = position;
                    yyy++;
                }
            }
        }
        _matrices = new Matrix4x4[_positions.Length];

        _rp = new RenderParams(materials[0]);

    }

    // void Update()
    // {
    //     if (_positions.Length > 0)
    //     {
    //         for (var i = 0; i < _positions.Length; i++)
    //         {
    //             _matrices[i].SetTRS(_positions[i], Quaternion.identity, new Vector3(1,1,1));
    //         }
    //         Graphics.RenderMeshInstanced(_rp, MeshVoxel, 0, _matrices);
    //     }
    // }

    private void CreateInstance()
    {
        listCubesGameObject = new();

        float offsetX = 1f;// / size.x;
        float offsetY = 1f;// / size.y;
        float offsetZ = 1f;// / size.z;

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                for (int z = 0; z < size.z; z++)
                {
                    // GameObject obj = Instantiate(prefab, new Vector3(offset.x + offsetX * x, offset.y + offsetY * y, offset.z + offsetZ * z), Quaternion.identity, transform);
                    // obj.transform.localScale = new Vector3(offsetX,offsetY,offsetZ);
                    // obj.gameObject.SetActive(false);
                    // listCubesGameObject.Add(obj);

                    GameObject voxel = new GameObject("Voxel" + (x * y * z), typeof(MeshRenderer), typeof(MeshFilter), typeof(MeshCollider));
                    voxel.transform.SetParent(transform);
                    Vector3 position = new Vector3(offset.x + offsetX * x + (offsetX/ 2), offset.y + offsetY * y +(offsetY/ 2), offset.z + offsetZ * z + (offsetZ/ 2));
                    voxel.transform.position = position;
                    voxel.transform.localScale = new Vector3(1, 1, 1);
                    voxel.GetComponent<MeshFilter>().mesh = MeshVoxel;
                    voxel.GetComponent<MeshRenderer>().materials = materials;
                    voxel.gameObject.isStatic = true;

                    listCubesGameObject.Add(voxel);

                }
            }
        }
        StaticBatchingUtility.Combine(listCubesGameObject.ToArray(), gameObject);

        // MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        // CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        // int i = 0;
        // while (i < meshFilters.Length)
        // {
        //     combine[i].mesh = meshFilters[i].sharedMesh;
        //     combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
        //     meshFilters[i].gameObject.SetActive(false);

        //     i++;
        // }
    }
}
