using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;


namespace Mikalai2006.Voxel
{
    public class VoxelMeshRender : MonoBehaviour, IVoxeled
    {
        // public Mesh _mesh;
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
        private Container[] containers;
        public Container[] Containers => containers;
        private RenderParams _rp;
        private Vector3 position = Vector3.zero;

        private void Start()
        {
            if (Wrapper == null)
            {
                Wrapper = transform.gameObject;
            }

            containers = new Container[Config.sOVoxelData.groups.Count];

            for (int j = 0; j < Config.sOVoxelData.groups.Count; j++)
            {
                CreateContainer(j);
            }
        }

        private void CreateContainer(int index)
        {
            Wrapper.transform.localRotation = Config.sOVoxelData.GlobalRotation;

            int count = Config.sOVoxelData.voxels.Count;
            // position = new Vector3(UnityEngine.Random.Range(0, 150), 0.5f, UnityEngine.Random.Range(0, 180));

            // _nativePositions = new NativeArray<float3>(count, Allocator.Persistent);
            // _nativeMatrices = new NativeArray<Matrix4x4>(count, Allocator.Persistent);
            // _nativeCubeYOffsets = new NativeArray<float>(count, Allocator.Persistent);
            // _nativeVoxelsPositions = new NativeArray<Vector3>(count, Allocator.Persistent);

            GameObject cont = new GameObject("Container");
            cont.transform.parent = Wrapper.transform;
            var container = cont.AddComponent<Container>();
            containers[index] = container;
            container.Initialize(Config, Vector3.zero);
            container.transform.SetPositionAndRotation(position, Quaternion.identity);
            container.transform.SetLocalPositionAndRotation((-1 * Config.sOVoxelData.Pivot) + (Vector3.one * Config.sOVoxelData.sizeVoxel / 2), Quaternion.identity);
            // container.gameObject.isStatic = true;

            // SceneTools.LoopPositions((i, p) =>
            // {
            //     _nativeCubeYOffsets[i] = p.y;
            //     _nativePositions[i] = p;
            // });
            // for (int x = 0; x < count; x++)
            // {
            //     _nativePositions[x] = sOVoxelData.voxels[x];
            //     _nativeVoxelsPositions[x] = sOVoxelData.voxels[x];
            // }

            // _job = new CubePositionJob
            // {
            //     // Positions = _nativePositions,
            //     // YOffsets = _nativeCubeYOffsets
            //     Voxels = _nativeVoxelsPositions,
            //     // mesh = _mesh,
            //     // container = container
            // };

            _rp = new RenderParams(Config._material);

            container.SetSizeVoxel(Config.sOVoxelData.sizeVoxel);
            // container.GetComponent<Collider>().isTrigger = true;

            // var segment = new ArraySegment<Vector3>(voxelList, 1, 10);
            // container.SetData(segment.ToArray(), scale);

            //  for (int j = 0; j < sOVoxelData.groups.Count; j++)
            // {
            //     // Vector3[] voxelList = sOVoxelData.voxels.AsParallel().ToArray();
            //     // Vector3[] voxelList = sOVoxelData.groups.ElementAt(j).voxels.AsParallel().ToArray();
            //     // Color groupColor = sOVoxelData.groups.ElementAt(j).color;

            container.SetData(Config.sOVoxelData, index, Config.isGreedy, 1);
            // }

            container.GenerateMesh();
            container.UploadMesh(true);

            // Graphics.RenderMesh(_rp, _mesh, 0, Matrix4x4.Translate(new Vector3(0f, 0.5f, 0f)));
        }

        private void Update()
        {
            // _job.Matrices = _nativeMatrices;
            // _job.Time = Time.time;
            // _job.Schedule(_nativeMatrices.Length, 64).Complete();
            // Graphics.RenderMesh(_rp, _mesh, 0, Matrix4x4.Translate(position));

        }

        private void OnDestroy()
        {
            // _nativePositions.Dispose();
            // _nativeMatrices.Dispose();
            // _nativeCubeYOffsets.Dispose();
        }

        public void OnSetConfigMeshGenerator(MeshConfig config)
        {
            // Config._material = config._material;
            // Config.sOVoxelData = config.sOVoxelData;
            // Config.existCollider = config.existCollider;
            // Config.isGreedy = config.isGreedy;
            Config = config;
        }
    }

    [BurstCompile]
    public struct CubePositionJob : IJobParallelFor
    {
        public NativeArray<Vector3> Voxels;
        // public Mesh mesh;
        // public NativeArray<float3> Positions;
        // [ReadOnly] public NativeArray<float> YOffsets;
        public NativeArray<Matrix4x4> Matrices;
        public float Time;
        // [ReadOnly] public Container container;

        public void Execute(int index)
        {
            // container.SetData(Voxels.ToArray(), 1);

            // container.GenerateMesh();
            // mesh = container.UploadMesh(false).mesh;
            // var (pos, rot) = Positions[index].CalculatePosBurst(YOffsets[index], Time);

            // Positions[index] = pos;
            // Matrices[index] = Matrix4x4.TRS(pos, rot, SceneTools.CubeScale);

        }
    }

    [Serializable]
    public struct MeshConfig
    {
        public SOVoxelData sOVoxelData;
        public Material _material;
        public bool existCollider;
        public bool isGreedy;
        public bool isRigidbody;
        public bool isConvex;
    }
}
