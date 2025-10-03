using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;


namespace Mikalai2006.Voxel
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshCollider))]
    public class VoxelMeshRenderWithSubmeshes : MonoBehaviour, IVoxeled
    {
        private MeshRenderer meshRenderer;
        private MeshFilter meshFilter;
        private MeshCollider meshCollider;
        private GPUInstanceEnabler gPUInstanceEnabler;

        private Vector3 pointCollision;

        private Stack<Vector3> needCreateElements;
        private GameObject explodeGameObject;
        [SerializeField] private LevelManager _levelManager;


        [SerializeField] private MeshConfig Config;
        // [SerializeField] private SOVoxelData sOVoxelData;
        // [SerializeField] private Material _material;
        // [SerializeField] bool existCollider;
        // [SerializeField] private bool isGreedy = true;
        [SerializeField] GameObject Wrapper;
        // private CubePositionJob _job;
        // private NativeArray<float> _nativeCubeYOffsets;
        // private NativeArray<Matrix4x4> _nativeMatrices;

        // private NativeArray<float3> _nativePositions;
        // private NativeArray<Vector3> _nativeVoxelsPositions;
        private VoxelMesh mesh;
        public VoxelMesh Mesh => mesh;
        // private RenderParams _rp;
        private Vector3 position = Vector3.zero;

        private void Start()
        {
            _levelManager = GameObject.FindGameObjectWithTag("LevelManager")?.GetComponent<LevelManager>();

            needCreateElements = new Stack<Vector3>();

            // подключаем необходимые компоненты.
            ConfigureComponents();

            if (Wrapper == null)
            {
                Wrapper = transform.gameObject;
            }

            Wrapper.transform.localRotation = Config.sOVoxelData.GlobalRotation;

            transform.localScale = new Vector3(1, 1, 1);
            transform.SetPositionAndRotation(position, Quaternion.identity);
            transform.SetLocalPositionAndRotation((-1 * Config.sOVoxelData.Pivot) + (Vector3.one * Config.sOVoxelData.sizeVoxel / 2), Quaternion.identity);

            // mesh = new VoxelMesh[Config.sOVoxelData.groups.Count];

            // for (int j = 0; j < Config.sOVoxelData.groups.Count; j++)
            // {
            //     // CreateContainer(j);
            // }
            CreateMesh();

            UploadMesh();
        }

        void OnDestroy()
        {
            // for (int i = 0; i < meshes.Length; i++)
            // {
            //     meshes[i].OnDestroy();
            // }

            mesh.OnDestroy();
        }

        private void ConfigureComponents()
        {
            meshFilter = GetComponent<MeshFilter>();
            meshRenderer = GetComponent<MeshRenderer>();
            meshCollider = GetComponent<MeshCollider>();
            // gPUInstanceEnabler = gameObject.AddComponent<GPUInstanceEnabler>();


            if (!Config.existCollider)
            {
                meshCollider.enabled = false;
            }
            else
            {
                meshCollider.convex = Config.isConvex;
            }

            if (Config.isRigidbody)
            {
                var r = gameObject.GetComponent<Rigidbody>();
                if (r == null)
                {
                    r = gameObject.AddComponent<Rigidbody>();
                }
                r.isKinematic = true;
                r.mass = 1000;
                r.freezeRotation = true;
                r.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezeRotationZ;
            }

            // устанавливаем материалы.
            meshRenderer.sharedMaterial = Config._material;
            // Material[] materials = new Material[Config.sOVoxelData.groups.Count];
            // for (int i = 0; i < Config.sOVoxelData.groups.Count; i++)
            // {
            //     materials[i] = Config._material;
            // }
            // meshRenderer.sharedMaterials = materials;
            // for (int i = 0; i < Config.sOVoxelData.groups.Count; i++)
            // {
            //     gPUInstanceEnabler.SetColor(Config.sOVoxelData.groups[i].color, i);
            // }

            // устанавливаем режим отображения теней.
            meshRenderer.shadowCastingMode = Config.shadowCastingMode;
        }

        private void CreateMesh()
        {
            mesh = new VoxelMesh();

            mesh.Initialize(Config);

            mesh.SetData();

            // mesh.ClearData();
        }

        private void UploadMesh()
        {
            if (meshRenderer == null)
                ConfigureComponents();

            MeshDataWithSub meshData;

            if (Config.isGreedy)
            {
                meshData = mesh.UploadMeshGreedy();
            }
            else
            {
                meshData = mesh.GenerateMesh();
            }

            meshFilter.sharedMesh = meshData.mesh;

            // Debug.Log($"meshFilter.sharedMesh.vertices.Length={meshFilter.sharedMesh.vertices.Length}");
            if (meshFilter.sharedMesh.vertices.Length > 3)
            {
                // meshData.mesh.Optimize();
                meshCollider.sharedMesh = meshData.mesh;
            }
        }

        public void OnSetConfigMeshGenerator(MeshConfig config)
        {
            Config = config;
        }

        async public UniTask ExposionVoxels(Vector3 _pointCollision, bool isDrawMesh, GameObject _explodeGameObject, float radiusExplode)
        {
            float startTime = Time.realtimeSinceStartup;

            pointCollision = _pointCollision;
            explodeGameObject = _explodeGameObject;

            // var list = new Dictionary<Vector3, Voxel>();
            // var x = Mathf.RoundToInt(pos.x);
            // var y = Mathf.RoundToInt(pos.y);
            // var z = Mathf.RoundToInt(pos.z);
            // Debug.Log($"Vector3Int {new Vector3(x, y, z)}, [{pos}]");

            // Voxel first;
            // List<Vector3> checkPositions = new List<Vector3>()
            //     {
            //         new Vector3(x, y, z),
            //         new Vector3(x+1, y, z),
            //         new Vector3(x-1, y, z),
            //         new Vector3(x, y+1, z),
            //         new Vector3(x, y-1, z),
            //         new Vector3(x, y, z+1),
            //         new Vector3(x, y, z-1)
            //     };

            float3[] keys = mesh.Data.Keys.AsParallel().ToArray();
            // Debug.Log($"Time copy keys: {(Time.realtimeSinceStartup - startTime) * 1000f} ms");
            // Debug.Log($"keys count={keys.Length}");
            NativeArray<float3> _needCreateElements = new NativeArray<float3>(keys.Length, Allocator.Persistent);
            NativeArray<float3> _needRemoveElements = new NativeArray<float3>(keys.Length, Allocator.Persistent);
            NativeArray<float3> points = new NativeArray<float3>(keys, Allocator.Persistent);
            // points.CopyFrom(keys);

            var collisionJob = new CheckCollisionJob
            {
                _needCreateElements = _needCreateElements,
                _pointCollision = pointCollision,
                points = points,
                needRemoveElements = _needRemoveElements,
                _radiusExplode = radiusExplode,
                maxRadius = GameManager.Instance.Settings.maxRadiusCreateVoxels
            };
            JobHandle collisionJobHandle = collisionJob.Schedule(points.Length, 64);
            collisionJobHandle.Complete(); // Or use dependency

            Debug.Log($"Time JOB create data: {(Time.realtimeSinceStartup - startTime) * 1000f} ms. Count point={points.Length}. _needCreateElements={collisionJob._needCreateElements.Length}.  needRemoveElements={collisionJob.needRemoveElements.Length}. ");

            for (int el = 0; el < collisionJob.needRemoveElements.Length; el++)
            {
                if (!collisionJob.needRemoveElements[el].Equals(float3.zero))
                {
                    float3 pos = collisionJob.needRemoveElements[el];
                    mesh.Data.Remove(pos);
                    mesh.SetVoxelData(pos, default);
                }
            };

            for (int el = 0; el < collisionJob._needCreateElements.Length; el++)
            {
                if (!collisionJob._needCreateElements[el].Equals(float3.zero))
                {
                    needCreateElements.Push(collisionJob._needCreateElements[el]);
                }
            }
            ;

            // for (int j = 0; j < keys.Length; j++)
            // {
            //     Vector3 posx = keys.ElementAt(j);
            //     if (TestHelpers.IsInsideSphere(posx, _pointCollision, 3))
            //     {
            //         // list.Add(posx, data.ElementAt(j).Value);
            //         // data[posx] = new Voxel()
            //         // {
            //         //     ID = 0,
            //         // };
            //         data.Remove(posx);

            //         // needCreateElements.Push(posx);
            //     }
            // }

            // Debug.Log($"Time for create data: {(Time.realtimeSinceStartup - startTime) * 1000f} ms");
            // list.Add(new Vector3(x,y,z),data[new Vector3(x,y,z)]);

            if (mesh.Data.Count >= 10 && needCreateElements.Count > 0)
            {
                // mesh.GenerateMesh();
                // // Debug.Log("Time generate mesh: " + (Time.realtimeSinceStartup - temp).ToString("f6"));
                // mesh.UploadMesh();
                UploadMesh();
            }

            if (mesh.Data.Count() < 10)
            {
                transform.gameObject.SetActive(false);
            }

            if (needCreateElements.Count > 0)
            {
                // Debug.Log($"Time upload mesh: {(Time.realtimeSinceStartup - startTime) * 1000f} ms");
                // Debug.Log("Time upload mesh: " + (Time.realtimeSinceStartup - temp).ToString("f6"));
                Debug.Log($"needCreateElements {needCreateElements.Count} voxels!");
                // StartCoroutine(createGO());
                await CreateObjectsAsync();
            }

            _needCreateElements.Dispose();
            _needRemoveElements.Dispose();
            points.Dispose();
        }
        
        
        public async UniTask CreateObjectsAsync()
        {
            int count = GameManager.Instance.Settings.countCreateVoxelByFrame;

            while (needCreateElements.Count > 0)
            {
                Vector3 elem = needCreateElements.Pop();

                float forceMagnitude = 10 * 100;
                // GameObject gObj = Instantiate(GameManager.Instance.Settings.prefabVoxel, Machine.levelManager.objectSpawnEffect.transform);
                GameObject gObj = Lean.Pool.LeanPool.Spawn(GameManager.Instance.Settings.prefabVoxel, _levelManager.objectSpawnEffect.transform);
                Vector3 pointSpawnVoxel = transform.TransformPoint(elem);
                gObj.transform.SetPositionAndRotation(pointSpawnVoxel, Quaternion.identity);
                gObj.GetComponent<VoxelPrefab>().Init();
                // gObj.isStatic = true;
                // gObj.transform.SetLocalPositionAndRotation(listVoxels.ElementAt(k).Key, Quaternion.identity);
                // gObj.gameObject.AddComponent<BoxCollider>();


                // var mat = gObj.gameObject.GetComponent<MeshRenderer>().material;
                // var mesh = gObj.gameObject.GetComponent<MeshFilter>().mesh;
                // RenderParams _rp = new RenderParams(WorldManager.Instance.worldMaterial);
                // Graphics.RenderMesh(_rp, mesh, 0, Matrix4x4.Translate(pointSpawnVoxel));

                var r = gObj.gameObject.GetComponent<Rigidbody>();
                if (r == null)
                {
                    r = gObj.gameObject.AddComponent<Rigidbody>();
                }
                r.collisionDetectionMode = CollisionDetectionMode.Continuous;
                // r.mass = 100f;
                r.useGravity = true;
                var forceDirection = UnityEngine.Random.onUnitSphere; //Vector3.Scale(UnityEngine.Random.onUnitSphere, transform.forward);
                r.AddForce(forceDirection * forceMagnitude, ForceMode.Impulse);
                // gameObjects[count - 1] = gObj;
                // gObj.isStatic = false;
                // Destroy(gObj, 15);
                Lean.Pool.LeanPool.Despawn(gObj, UnityEngine.Random.Range(1, 3));


                // // simulate paraboloid.
                // var forceDirection = UnityEngine.Random.onUnitSphere;
                // float time = UnityEngine.Random.Range(1, 5);
                // gObj.Init(forceDirection * 10, UnityEngine.Random.onUnitSphere, time * 0.5f);
                // Lean.Pool.LeanPool.Despawn(gObj, time);

                count--;

                if (count < 0)
                {
                    count = GameManager.Instance.Settings.countCreateVoxelByFrame;
                    await UniTask.NextFrame();
                }
            }

        }

    }
}
