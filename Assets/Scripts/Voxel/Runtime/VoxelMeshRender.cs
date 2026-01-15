using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Mikalai2006.Voxel
{
    public class VoxelMeshRender : MonoBehaviour, IVoxeled
    {
        // public Action OnSetData;
        GameManager _gameManager => GameManager.Instance;
        [SerializeField] public MeshConfig Config;
        [SerializeField] public List<ColorsModify> colorsModify;
        [SerializeField] public Dictionary<Vector3, int> destroyedVoxels;
        public DataDetail _dataDetail;
        // public bool isActive = true;
        // [SerializeField] public MeshConfigModify ConfigModify;
        // [SerializeField] private SOVoxelData sOVoxelData;
        // [SerializeField] private Material _material;
        // [SerializeField] bool existCollider;
        // [SerializeField] private bool isGreedy = true;
        [SerializeField] public GameObject Wrapper;
        // private CubePositionJob _job;
        // private NativeArray<float> _nativeCubeYOffsets;
        // private NativeArray<Matrix4x4> _nativeMatrices;

        // private NativeArray<float3> _nativePositions;
        // private NativeArray<Vector3> _nativeVoxelsPositions;
        private Container[] containers;
        public Container[] Containers => containers;
        // private RenderParams _rp;
        private Vector3 position = Vector3.zero;

        void Awake()
        {
            colorsModify = new();
            destroyedVoxels = new();
        }

        private void Start()
        {
            Container[] existCntainers = GetComponentsInChildren<Container>();
            if (existCntainers.Length > 0)
            {
                Helpers.DestroyChildren(transform);
            }

            if (Wrapper == null)
            {
                Wrapper = transform.gameObject;
            }

            // Debug.Log($"CreateContainer {gameObject.name}");
            Wrapper.transform.localRotation = Config.sOVoxelData.GlobalRotation;

            if (Config.customScale > 0) {
                Wrapper.transform.localScale = new Vector3(Config.customScale, Config.customScale, Config.customScale);
            }  else if (Config.useGlobalScale)
            {
                // var maxBoundsSize = Mathf.Max(Config.sOVoxelData.Bounds.x, Config.sOVoxelData.Bounds.y, Config.sOVoxelData.Bounds.z);
                if (_gameManager)
                {
                    Wrapper.transform.localScale = new Vector3(_gameManager.Settings.scaleObjects, _gameManager.Settings.scaleObjects, _gameManager.Settings.scaleObjects);
                }
            }


            BaseMachine bm = transform.GetComponentInParent<BaseMachine>();
            if (_gameManager && _gameManager.LevelConfig && bm == false)
            {
                SetColorsModify(_gameManager.LevelConfig.colorsModify);
            }

            if (Config.isOneMesh)
            {
                containers = new Container[1];
                CreateContainer(0);
            }
            else
            {
                containers = new Container[Config.sOVoxelData.groups.Count];
                for (int j = 0; j < Config.sOVoxelData.groups.Count; j++)
                {
                    CreateContainer(j);
                }
            }
        }

        private void CreateContainer(int index)
        {

            // int count = Config.sOVoxelData.voxels.Count;
            // position = new Vector3(UnityEngine.Random.Range(0, 150), 0.5f, UnityEngine.Random.Range(0, 180));

            // _nativePositions = new NativeArray<float3>(count, Allocator.Persistent);
            // _nativeMatrices = new NativeArray<Matrix4x4>(count, Allocator.Persistent);
            // _nativeCubeYOffsets = new NativeArray<float>(count, Allocator.Persistent);
            // _nativeVoxelsPositions = new NativeArray<Vector3>(count, Allocator.Persistent);

            GameObject cont = new GameObject($"Container{Config.typeCollider}_{Config.sOVoxelData.name}___{index}");
            cont.transform.parent = Wrapper.transform;
            cont.layer = transform.gameObject.layer;// LayerMask.NameToLayer("Wall");
            Container container;
            switch (Config.typeCollider)
            {
                case TypeCollider.BoxCollider:
                    container = cont.AddComponent<ContainerBox>();
                break;
                case TypeCollider.SphereCollider:
                    container = cont.AddComponent<ContainerSphere>();
                break;
                default:
                    container = cont.AddComponent<ContainerMesh>();
                break;
            }
            
            containers[index] = container;
            container.Initialize(Config, Vector3.zero, this);
            SetTransforms(container);
            // // container.gameObject.isStatic = true;

            // // SceneTools.LoopPositions((i, p) =>
            // // {
            // //     _nativeCubeYOffsets[i] = p.y;
            // //     _nativePositions[i] = p;
            // // });
            // // for (int x = 0; x < count; x++)
            // // {
            // //     _nativePositions[x] = sOVoxelData.voxels[x];
            // //     _nativeVoxelsPositions[x] = sOVoxelData.voxels[x];
            // // }

            // // _job = new CubePositionJob
            // // {
            // //     // Positions = _nativePositions,
            // //     // YOffsets = _nativeCubeYOffsets
            // //     Voxels = _nativeVoxelsPositions,
            // //     // mesh = _mesh,
            // //     // container = container
            // // };

            // // _rp = new RenderParams(Config._material);

            // // container.SetSizeVoxel(Config.sOVoxelData.sizeVoxel);
            // // container.GetComponent<Collider>().isTrigger = true;

            // // var segment = new ArraySegment<Vector3>(voxelList, 1, 10);
            // // container.SetData(segment.ToArray(), scale);

            // //  for (int j = 0; j < sOVoxelData.groups.Count; j++)
            // // {
            // //     // Vector3[] voxelList = sOVoxelData.voxels.AsParallel().ToArray();
            // //     // Vector3[] voxelList = sOVoxelData.groups.ElementAt(j).voxels.AsParallel().ToArray();
            // //     // Color groupColor = sOVoxelData.groups.ElementAt(j).color;

            // // }
            // if (Config.isOneMesh)
            // {
            //     container.SetData();
            // }
            // else
            // {
            //     container.SetData(index);
            // }
            UpdateMeshContainer(container, index);

            // // OnSetData?.Invoke();

            // if (Config.isGreedy)
            // {
            //     container.UploadMeshGreedy(Config.sOVoxelData.startMesh == null).Forget();
            // }
            // else
            // {
            //     container.GenerateMesh();
            //     container.UploadMesh(Config.sOVoxelData.startMesh == null);
            // }

            // // Graphics.RenderMesh(_rp, _mesh, 0, Matrix4x4.Translate(new Vector3(0f, 0.5f, 0f)));
        }

        public void SetTransforms(Container container)
        {
            container.transform.localScale = new Vector3(1, 1, 1);
            container.transform.SetPositionAndRotation(position, Quaternion.identity);
            if (Config.isTile)
            {
                var maxAxis = Mathf.Max(Config.sOVoxelData.Bounds.x, Config.sOVoxelData.Bounds.y, Config.sOVoxelData.Bounds.z) / 2f;
                container.transform.SetLocalPositionAndRotation((-1 * new Vector3(maxAxis, 0.5f, maxAxis)) + (Vector3.one * Config.sOVoxelData.sizeVoxel / 2), Quaternion.identity);           
            } else
            {
                container.transform.SetLocalPositionAndRotation((-1 * Config.sOVoxelData.Pivot) + (Vector3.one * Config.sOVoxelData.sizeVoxel / 2), Quaternion.identity);
            }
        }

        public void SetColorsModify(List<ColorsModify> _colorsModify)
        {
            if (_colorsModify == null) return;

            colorsModify.Clear();
            colorsModify.AddRange(_colorsModify);
        }

        public void SetDestroyedVoxels(SerializeVector3 vector3Ints)
        {
            if (vector3Ints == null) return;

            for (int i = 0; i < vector3Ints.Count; i++)
            {
                KeyValuePair<Vector3Int, TypeEntity> item = vector3Ints.ElementAt(i); 
                
                if (!destroyedVoxels.ContainsKey(item.Key))
                {
                    destroyedVoxels.Add(item.Key, 1);
                }
            }
        }

        public void SetData(StateMachinePlayerData data, DataDetail dataDetail)
        {
            if (data != null)
            {
                SetColorsModify(data.colorsModifies);
            }
            
            if (dataDetail != null)
            {
                _dataDetail = dataDetail;

                SetDestroyedVoxels(dataDetail.destroyVoxels);
            }
        }

        private void UpdateMeshContainer(Container container, int index)
        {
            if (Config.isOneMesh)
            {
                container.SetData();
            }
            else
            {
                container.SetData(index);
            }

            // OnSetData?.Invoke();

            if (Config.isGreedy)
            {
                container.UploadMeshGreedy(Config.sOVoxelData.startMesh == null).Forget();
            }
            else
            {
                container.GenerateMesh();
                container.UploadMesh(Config.sOVoxelData.startMesh == null);
            }
        }

        public void UploadedAllMeshes(List<ColorsModify> colors = null)
        {
            if (colors != null)
            {
                SetColorsModify(colors);
            }
            // Debug.Log($"Upload all meshes {name} {containers.Length}");
            if (containers != null)
            {
                // Debug.Log($"Set color 3 UploadedAllMeshes lenght={containers.Length}");
                for (int index = 0; index < containers.Length; index++)
                {
                    UpdateMeshContainer(containers[index], index);
                }
            }
        }

        public void UploadedAllMeshes(DataDetail dataDetail)
        {
            if (dataDetail != null)
            {
                SetData(null, dataDetail);
            }
            if (containers != null)
            {
                for (int index = 0; index < containers.Length; index++)
                {
                    SetTransforms(containers[index]);
                    
                    UpdateMeshContainer(containers[index], index);
                }
            }
        }

        // private void Update()
        // {
        //     // _job.Matrices = _nativeMatrices;
        //     // _job.Time = Time.time;
        //     // _job.Schedule(_nativeMatrices.Length, 64).Complete();
        //     // Graphics.RenderMesh(_rp, _mesh, 0, Matrix4x4.Translate(position));

        // }

        public Voxel GetVoxel(int indexContainer, Vector3Int position)
        {
            return containers[indexContainer].GetVoxel(position);
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

            // если есть контейнеры, обновляем в них Config.
            if (containers != null)
            {
                for (int index = 0; index < containers.Length; index++)
                {
                    containers[index].SetConfig(Config);
                }
            }
            // if (Config.meshConfigModify != default)
            // {
            //     for (int i = 0; i < Config.sOVoxelData.groups.Count; i++)
            //     {
            //         var group = Config.sOVoxelData.groups[i];
            //         group.color = Config.color.Length > i ? Config.color[i] : group.color;
            //         Config.sOVoxelData.groups[i] = group;
            //         Debug.Log($"Set color {group.color} ///{Config.sOVoxelData.groups[i].color}");
            //         // if (Config.isGreedy)
            //         // {
            //         //     containers[i].UploadMeshGreedy(Config.sOVoxelData.startMesh == null).Forget();
            //         // }
            //         // else
            //         // {
            //         //     containers[i].GenerateMesh();
            //         //     containers[i].UploadMesh(Config.sOVoxelData.startMesh == null);
            //         // }
            //     }
            //     Debug.Log($"Set color 2 {Config.color[0]} ///{Config.sOVoxelData.groups[0].color}");
            // }
        }

        public void SetActive(bool v)
        {
            Tile3D tile3D = transform.GetComponentInParent<Tile3D>();
            
            if (tile3D != null)
            {
                tile3D.SetActive(v);
            }
        }
    }

    // [BurstCompile]
    // public struct CubePositionJob : IJobParallelFor
    // {
    //     public NativeArray<Vector3> Voxels;
    //     // public Mesh mesh;
    //     // public NativeArray<float3> Positions;
    //     // [ReadOnly] public NativeArray<float> YOffsets;
    //     public NativeArray<Matrix4x4> Matrices;
    //     public float Time;
    //     // [ReadOnly] public Container container;

    //     public void Execute(int index)
    //     {
    //         // container.SetData(Voxels.ToArray(), 1);

    //         // container.GenerateMesh();
    //         // mesh = container.UploadMesh(false).mesh;
    //         // var (pos, rot) = Positions[index].CalculatePosBurst(YOffsets[index], Time);

    //         // Positions[index] = pos;
    //         // Matrices[index] = Matrix4x4.TRS(pos, rot, SceneTools.CubeScale);

    //     }
    // }
}

// [System.Serializable]
// public struct MeshConfigModify
// {
//     public List<Color> colors;
// }