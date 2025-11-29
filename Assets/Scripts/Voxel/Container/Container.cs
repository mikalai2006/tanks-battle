using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Mikalai2006.Voxel
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    // [RequireComponent(typeof(MeshCollider))]
    public abstract class Container : MonoBehaviour
    {
        private Dictionary<float3, Voxel> dataVoxels;
        [SerializeField] private ContainerData _containerData;
        public ContainerData ContainerData => _containerData;
        [SerializeField] private MeshConfig meshConfig;
        string meshName = "procedure";
        // private NativeArray<Vector3> vertices;
        // private NativeArray<int> triangles;
        // private NativeArray<Vector3> newVertices;
        // private NativeArray<int> newTriangles;
        private NativeArray<Voxel> arrayVoxels;
        private NativeArray<VoxelColors> arrayVoxelColors;

        private Vector3 pointCollision;
        protected MeshData meshData = new MeshData();
        // private float sizeVoxel = 1;
        // private RenderParams _rp;
        // [SerializeField] private Material material;

        private MeshRenderer meshRenderer;
        protected MeshFilter meshFilter;
        private GPUInstanceEnabler gPUInstanceEnabler;
        // private PropertyBlockChanger propertyBlockChanger;
        // private MeshCollider meshCollider;
        // private Stack<RemoveVoxel> needCreateElements;
        // private Stack<RemoveVoxel> needGravityCreateElements;
        // private Collision collision;
        // private GameObject explodeGameObject;
        // private SOVoxelData _sOVoxelData;
        // [SerializeField] private bool isGreedy = true;
        List<Vector3> prevContacts;
        [SerializeField] private LevelManager _levelManager;
        private System.Threading.CancellationTokenSource cancelTokenSrc;

        void Awake()
        {
            cancelTokenSrc = new System.Threading.CancellationTokenSource();           
        }

        void OnEnable()
        {
            cancelTokenSrc = new System.Threading.CancellationTokenSource();   
        }

        void OnDisable()
        {
            arrayVoxels.Dispose();
            arrayVoxelColors.Dispose();

            cancelTokenSrc.Cancel();
            cancelTokenSrc.Dispose();
        }

        void OnDestroy()
        {
            // vertices.Dispose();
            // triangles.Dispose();
            // newVertices.Dispose();
            // newTriangles.Dispose();
            
            if (arrayVoxels.IsCreated)
            {
                arrayVoxels.Dispose();
            }

            if (arrayVoxelColors.IsCreated) {
                arrayVoxelColors.Dispose();
            }

            if (cancelTokenSrc != null && !cancelTokenSrc.IsCancellationRequested)
            {
                cancelTokenSrc.Cancel();
                cancelTokenSrc.Dispose();
            }
        }

        
        // void OnCollisionEnter(Collision collision)
        // {
        //     Debug.Log($"<color=blue>Container is collision with trigger {collision.gameObject.name}</color>");
        // }
        // void OnTriggerEnter(Collider collision)
        // {
        //     Debug.Log($"<color=green>Trigger: Container is collision with trigger {collision.gameObject.name}</color>");
        // }

        public virtual void SetConfig(MeshConfig config)
        {
            meshConfig = config;
        }

        public virtual bool PointInCollider(Vector3 point)
        {
            return false;
        }

        public virtual void Initialize(MeshConfig config, Vector3 position)
        {
            prevContacts = new();

            SetConfig(config);

            // isGreedy = _isGreedy;
            // vertices = new NativeArray<Vector3>();
            // triangles = new NativeArray<int>();
            // newTriangles = new NativeArray<int>();
            // newVertices = new NativeArray<Vector3>();

            // gameObject.isStatic = true;

            ConfigureComponents();

            // if (!config.existCollider)
            // {
            //     meshCollider.enabled = false;
            // }
            // else
            // {
            //     meshCollider.convex = config.isConvex;
            //     // meshCollider.providesContacts = true;
            // }

            if (config.isRigidbody)
            {
                var r = gameObject.GetComponent<Rigidbody>();
                if (r == null)
                {
                    r = gameObject.AddComponent<Rigidbody>();
                }
                r.isKinematic = config.isKinematic;
                r.mass = config.mass > 0 ? config.mass : 1000;
                r.freezeRotation = config.isKinematic;
                r.constraints = config.constraints; // RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezeRotationZ
            }

            dataVoxels = new Dictionary<float3, Voxel>();

            meshRenderer.sharedMaterial = config._material;
            // material = config._material;
            // _rp = new RenderParams(config._material);

            // needCreateElements = new Stack<RemoveVoxel>();
            // needGravityCreateElements = new Stack<RemoveVoxel>();

            _levelManager = GameObject.FindGameObjectWithTag("LevelManager")?.GetComponent<LevelManager>();
        }

        public void SetData(int indexGroup)
        {
            if (gPUInstanceEnabler != null)
            {
                gPUInstanceEnabler.SetColor(meshConfig.sOVoxelData.groups[indexGroup].color);
            }
            // if (propertyBlockChanger != null)
            // {
            //     propertyBlockChanger.SetData(meshConfig.emissionValue);
            // }

            // for (int i = 0; i < voxelList.Length; i++)
            // {
            //     vertices[i] = voxelList[i];
            // }

            // // исключение внутренних вершин.
            // Dictionary<Vector3, bool> dictionaryVoxels = sOVoxelData.voxels.AsParallel().ToDictionary(t => t, s => true);
            // List<Vector3> visibleVoxels = new List<Vector3>();

            // for (int i = 0; i < sOVoxelData.voxels.Count; i++)
            // {
            //     int countNeighbours = GetVoxelNeighbours(sOVoxelData.voxels[i], dictionaryVoxels);
            //     if (countNeighbours < 24)
            //     {
            //         visibleVoxels.Add(sOVoxelData.voxels[i]);
            //     }
            // }
            // Debug.Log($"allVoxels = {sOVoxelData.voxels.Count}, visibleVoxels = {visibleVoxels.Count}");

            // for (int j = 0; j < sOVoxelData.groups.Count; j++) {
            Vector3Int[] voxelList = meshConfig.sOVoxelData.groups[indexGroup].voxels.AsParallel().ToArray();
            // Vector3[] voxelList = sOVoxelData.groups.ElementAt(j).voxels.AsParallel().ToArray();
            // Color groupColor = sOVoxelData.groups.ElementAt(j).color;

            arrayVoxelColors = new NativeArray<VoxelColors>(0, Allocator.Persistent);

            // create array voxels for jobs.
            arrayVoxels = new NativeArray<Voxel>(meshConfig.sOVoxelData.Bounds.x * meshConfig.sOVoxelData.Bounds.y * meshConfig.sOVoxelData.Bounds.z, Allocator.Persistent);

            // parse list voxels and create data. 
            for (int i = 0; i < voxelList.Length; i++)
            {
                Color groupColor = meshConfig.meshConfigModify.colors != null && meshConfig.meshConfigModify.colors.Count > indexGroup ? meshConfig.meshConfigModify.colors[indexGroup] : meshConfig.sOVoxelData.groups[indexGroup].color;
                var vox = new Voxel() // * scale
                {
                    ID = 1,
                    color = groupColor, // meshConfig.sOVoxelData.groups[indexGroup].color
                    type = VoxelType.Grass,
                    // IndexSubMesh = j
                };
                this[voxelList[i]] = vox;

                Vector3Int pos = Vector3Int.FloorToInt(voxelList[i]);
                arrayVoxels[Helpers.To1D(pos.x, pos.y, pos.z, meshConfig.sOVoxelData.Bounds.x, meshConfig.sOVoxelData.Bounds.y)] = vox;
            }
            // }

            _containerData.countVoxels = meshConfig.sOVoxelData.countVoxels;
            
            BaseMachine bm = transform.GetComponentInParent<BaseMachine>();
            if (bm)
            {
                bm.RefreshHP();
            }
        }

        public void SetData()
        {
            // if (propertyBlockChanger != null)
            // {
            //     propertyBlockChanger.SetData(meshConfig.emissionValue);
            // }
            // Vector3Int[] voxelList = meshConfig.sOVoxelData.groups[indexSubMesh].voxels.AsParallel().ToArray();

            arrayVoxelColors = new NativeArray<VoxelColors>(meshConfig.sOVoxelData.groups.Count + 1, Allocator.Persistent);

            // create array voxels for jobs.
            arrayVoxels = new NativeArray<Voxel>(meshConfig.sOVoxelData.Bounds.x * meshConfig.sOVoxelData.Bounds.y * meshConfig.sOVoxelData.Bounds.z, Allocator.Persistent);

            // parse list voxels and create data. 
            for (int j = 0; j < meshConfig.sOVoxelData.groups.Count; j++)
            {
                Color color = meshConfig.meshConfigModify.colors != null && meshConfig.meshConfigModify.colors.Count > j ? meshConfig.meshConfigModify.colors[j] : meshConfig.sOVoxelData.groups[j].color;
                // TODO color.a = 1;
                arrayVoxelColors[j + 1] = new VoxelColors()
                {
                    color = color,
                    type = (VoxelType)(j + 1)
                };


                for (int i = 0; i < meshConfig.sOVoxelData.groups[j].voxels.Count; i++)
                {

                    Vector3Int pos = Vector3Int.FloorToInt(meshConfig.sOVoxelData.groups[j].voxels[i]);
                    var vox = new Voxel() // * scale
                    {
                        ID = 1,
                        color = color, //meshConfig.sOVoxelData.colors.ElementAt(i),
                        type = (VoxelType)(j + 1),
                        position = pos,
                        IndexSubMesh = j,
                    };
                    this[meshConfig.sOVoxelData.groups[j].voxels[i]] = vox;

                    arrayVoxels[Helpers.To1D(pos.x, pos.y, pos.z, meshConfig.sOVoxelData.Bounds.x, meshConfig.sOVoxelData.Bounds.y)] = vox;
                }
            }

            _containerData.countVoxels = meshConfig.sOVoxelData.countVoxels;
            BaseMachine bm = transform.GetComponentInParent<BaseMachine>();
            if (bm)
            {
                bm.RefreshHP();
            }
        }

        public void ClearData()
        {
            // vertices.Dispose();
            // triangles.Dispose();
            dataVoxels.Clear();
        }

        // public void SetSizeVoxel(float _sizeVoxel)
        // {
        //     sizeVoxel = _sizeVoxel;
        // }

        public void GenerateMesh()
        {
            float startTime = Time.realtimeSinceStartup;
            meshData.ClearData();

            Vector3 blockPos;
            Voxel block;

            int counter = 0;
            Vector3[] faceVertices = new Vector3[4];
            Vector2[] faceUVs = new Vector2[4];

            // VoxelColor voxelColor;
            // Color voxelColorAlpha;
            // Vector2 voxelSmoothness;

            foreach (KeyValuePair<float3, Voxel> kvp in dataVoxels)
            {
                // Проверяйте только сплошные блоки.
                if (!kvp.Value.isSolid)
                {
                    continue;
                }

                blockPos = kvp.Key;
                block = kvp.Value;

                // // проверяем есть ли в словаре запись для индекса подсетки.
                // if (!meshData.triangles.ContainsKey(block.IndexSubMesh))
                // {
                //     meshData.triangles[block.IndexSubMesh] = new List<int>();
                // }

                // voxelColor = WorldManager.Instance.WorldColors[block.ID - 1];
                Color voxelColorAlpha = block.color;
                // voxelColorAlpha.a = 1;
                // voxelSmoothness = new Vector2(voxelColor.metallic, voxelColor.smoothness);

                // Iterate over each face direction
                for (int i = 0; i < 6; i++)
                {
                    // Проверьте, есть ли сплошной блок напротив этой грани.
                    if (this[blockPos + HelperVoxel.voxelFaceChecks[i]].isSolid)
                    {
                        continue;
                    }

                    //Draw this face

                    // Соберите соответствующие вершины из вершин по умолчанию и добавьте позицию блока.
                    for (int j = 0; j < 4; j++)
                    {
                        faceVertices[j] = HelperVoxel.voxelVertices[HelperVoxel.voxelVertexIndex[j + i * 4]] + blockPos;
                        faceUVs[j] = HelperVoxel.voxelUVs[j];
                    }

                    for (int j = 0; j < 6; j++)
                    {
                        meshData.vertices.Add(faceVertices[HelperVoxel.voxelTris[j + i * 6]]);
                        meshData.UVs.Add(faceUVs[HelperVoxel.voxelTris[j + i * 6]]);
                        meshData.colors.Add(voxelColorAlpha);
                        meshData.UVs2.Add(new Vector2(0f, 0.5f)); // voxelSmoothness

                        // meshData.triangles[block.IndexSubMesh].Add(counter++);
                        meshData.triangles.Add(counter++);
                    }
                }

            }

            Debug.Log($"Time generate mesh {gameObject.name}: {(Time.realtimeSinceStartup - startTime) * 1000f} ms.\r\nData count ={dataVoxels.Count}, Create {meshData.vertices.Count} vertices, {meshData.triangles.Count} triangles");

            // var a = meshData.vertices.GroupBy(ff => ff).ToList();
            // Debug.Log($"uniqueVertice: {a.Count()}");

            // int xxx = 0;
            // foreach (var group in a)
            // {
            //     if (group.Count() > 2)
            //     {
            //         Debug.Log($"More 2: {group.Count()}");
            //         xxx++;
            //     }
            // }
            //         Debug.Log($"More all: {xxx}");

            // // Исключение повторяющихся вершин.
            // Vector3[] newVertices = new Vector3[meshData.vertices.Count];
            // int[] newTriangles = new int[meshData.triangles.Count];
            // List<Color> newColors = new ();
            // List<Vector2> newUVs = new ();
            // List<Vector2> newUVs2 = new ();
            // Dictionary<Vector3, int> uniqueVertices = new Dictionary<Vector3, int>();
            // int uniqueVertexCount = 0;
            // // Перебор вершин и обновление индексов
            // for (int i = 0; i < meshData.triangles.Count; i++)
            // {
            //     int vertexIndex = meshData.triangles[i];
            //     Vector3 vertex = meshData.vertices[vertexIndex];

            //     if (uniqueVertices.ContainsKey(vertex))
            //     {
            //         newTriangles[i] = uniqueVertices[vertex];
            //     }
            //     else
            //     {
            //         newTriangles[i] = uniqueVertexCount;
            //         newVertices[uniqueVertexCount] = vertex;
            //         uniqueVertices.Add(vertex, uniqueVertexCount);
            //         uniqueVertexCount++;
            //         newColors.Add(meshData.colors.ElementAt(i));
            //         newUVs.Add(meshData.UVs.ElementAt(i));
            //         newUVs2.Add(meshData.UVs2.ElementAt(i));
            //     }
            // }

            // meshData.triangles = newTriangles.ToList();
            // meshData.vertices = uniqueVertices.Keys.ToList();
            // meshData.colors = newColors.ToList();
            // meshData.UVs = newUVs;
            // meshData.UVs2 = newUVs2;
            // Debug.Log($"Create {uniqueVertices.Count} vertices, {newTriangles.Length} triangles");
            // Debug.Log("=====================================================");

            // vertices = new NativeArray<Vector3>(meshData.vertices.Count, Allocator.Persistent);
            // newVertices = new NativeArray<Vector3>(meshData.vertices.Count, Allocator.Persistent);
            // for (int r = 0; r < meshData.vertices.Count; r++)
            // {
            //     vertices[r] = meshData.vertices[r];
            // }

            // triangles = new NativeArray<int>(meshData.triangles.Count, Allocator.Persistent);
            // newTriangles = new NativeArray<int>(meshData.triangles.Count, Allocator.Persistent);
            // for (int r = 0; r < meshData.triangles.Count; r++)
            // {
            //     triangles[r] = meshData.triangles[r];
            // }
        }
        // public void GenerateMesh(int resolution)
        // {
        //     // Allocate MeshData
        //     Mesh.MeshDataArray meshDataArray = Mesh.AllocateWritableMeshData(1);
        //     Mesh.MeshData meshData = meshDataArray[0];

        //     // Set up mesh parameters
        //     int vertexCount = resolution * resolution;
        //     int triangleCount = (resolution - 1) * (resolution - 1) * 2;
        //     meshData.subMeshCount = 1;
        //     meshData.SetVertexBufferParams(vertexCount, Vertex.Layout);
        //     meshData.SetIndexBufferParams(triangleCount * 3, IndexFormat.UInt32);

        //     // Create and schedule the job
        //     GenerateMeshJob generateMeshJob = new GenerateMeshJob
        //     {
        //         resolution = resolution,
        //         vertexData = meshData.GetVertexData<Vertex>(),
        //         indexData = meshData.GetIndexData<uint>(),
        //         // ... other parameters
        //     };

        //     JobHandle jobHandle = generateMeshJob.Schedule();
        //     jobHandle.Complete(); // Or use dependency

        //     // Apply and dispose
        //     Mesh mesh = new Mesh { name = "Procedural Mesh" };
        //     Mesh.ApplyAndDisposeWritableMeshData(meshDataArray, mesh);
        //     GetComponent<MeshFilter>().mesh = mesh;

        //     meshFilter.sharedMesh = mesh;
        //     meshCollider.sharedMesh = mesh;
        // }

        // private int GetVoxelNeighbours(Vector3 pos, Dictionary<Vector3, bool> allVoxels)
        // {
        //     int i = 0;

        //     Vector3[] coordsNeighbours = new Vector3[]{
        //         new Vector3(-1,-1,-1),
        //         new Vector3(-1,-1,0),
        //         new Vector3(-1,-1,1),
        //         new Vector3(-1,0,0),
        //         new Vector3(-1,0,1),
        //         new Vector3(-1,1,-1),
        //         new Vector3(-1,1,0),
        //         new Vector3(-1,1,1),
        //         new Vector3(0,-1,-1),
        //         new Vector3(0,-1,0),
        //         new Vector3(0,-1,1),
        //         new Vector3(0,-1,0),
        //         new Vector3(0,-1,1),
        //         new Vector3(0,0,-1),
        //         new Vector3(0,0,0),
        //         new Vector3(0,0,1),
        //         new Vector3(0,1,-1),
        //         new Vector3(0,1,0),
        //         new Vector3(0,1,1),
        //         new Vector3(1,-1,-1),
        //         new Vector3(1,-1,0),
        //         new Vector3(1,-1,1),
        //         new Vector3(1,0,-1),
        //         new Vector3(1,0,0),
        //         new Vector3(1,0,1),
        //         new Vector3(1,1,-1),
        //         new Vector3(1,1,0),
        //         new Vector3(1,1,1),
        //     };

        //     for (int x = 0; x < coordsNeighbours.Length; x++)
        //     {
        //         Vector3 nPos = pos + coordsNeighbours[x];
        //         bool value = false;
        //         if (allVoxels.TryGetValue(nPos, out value))
        //         {
        //             i++;
        //         }
        //     }

        //     return i;
        // }


        public virtual MeshData UploadMesh(bool isDrawMesh)
        {
            meshData.mesh.name = $"{meshName}_{gameObject.name}";
            meshData.UploadMesh();

            if (isDrawMesh)
            {
                if (meshRenderer == null)
                    ConfigureComponents();

                meshFilter.sharedMesh = meshData.mesh;

                // // // simplify mesh.
                // // float startTime = Time.realtimeSinceStartup;
                // // if (true)
                // //     {
                // //         var originalMesh = meshFilter.sharedMesh;
                // //         float quality = 0.35f;
                // //         var meshSimplifier = new UnityMeshSimplifier.MeshSimplifier();
                // //         var simpleOptions = SimplificationOptions.Default;
                // //         simpleOptions.VertexLinkDistance = 0.1;
                // //         meshSimplifier.SimplificationOptions = simpleOptions;
                // //         meshSimplifier.Initialize(originalMesh);
                // //         meshSimplifier.SimplifyMesh(quality);
                // //         var destMesh = meshSimplifier.ToMesh();
                // //         meshFilter.sharedMesh = destMesh;
                // //     }
                // //     Debug.Log($"Time simplify mesh: {(Time.realtimeSinceStartup - startTime) * 1000f} ms");

                // // greedy mesh.
                // if (isGreedy)
                // {
                //     float startTime = Time.realtimeSinceStartup;

                //     // Voxel[] voxArray = new Voxel[_sOVoxelData.Bounds.x * _sOVoxelData.Bounds.y * _sOVoxelData.Bounds.z];
                //     var mesh = meshFilter.sharedMesh;
                //     var meshArray = Mesh.AllocateWritableMeshData(mesh);
                //     var _job = new MeshGreedyJob();
                //     _job.mesh = meshArray[0];
                //     _job.chunkSize = new int3(_sOVoxelData.Bounds.x, _sOVoxelData.Bounds.y, _sOVoxelData.Bounds.z);
                //     _job.blockSize = 1;
                //     _job.voxelColors = arrayVoxelColors;

                //     // Debug.Log($"Time greedy mesh step0: {(Time.realtimeSinceStartup - startTime) * 1000f} ms");
                //     // Parallel.For(0, data.Count, (g) =>
                //     // {
                //     //     Vector3Int pos = Vector3Int.FloorToInt(data.ElementAt(g).Key);
                //     //     voxArray[Helpers.To1D(pos.x, pos.y, pos.z, _sOVoxelData.Bounds.x, _sOVoxelData.Bounds.y)] = data.ElementAt(g).Value;
                //     // });
                //     // Debug.Log($"Time greedy mesh step1: {(Time.realtimeSinceStartup - startTime) * 1000f} ms");
                //     _job.voxels = arrayVoxels; // new NativeArray<Voxel>(voxArray, Allocator.TempJob);
                //     _job.Schedule().Complete();

                //     // Debug.Log($"Time greedy mesh step2: {(Time.realtimeSinceStartup - startTime) * 1000f} ms");
                //     Mesh.ApplyAndDisposeWritableMeshData(meshArray, mesh);

                //     // FIXME: For some reason setting bounds directly doesn't work so this is needed as a workaround, investigate
                //     mesh.RecalculateBounds();
                //     meshFilter.sharedMesh = mesh;

                //     // _job.voxels.Dispose();
                //     Debug.Log($"Time greedy mesh: {(Time.realtimeSinceStartup - startTime) * 1000f} ms");
                // }

            }
            else
            {
                // подставляем готовый меш, чтобы получить GPU Instancing в начале
                meshFilter.sharedMesh = meshConfig.sOVoxelData.startMesh;
            }


            // if (meshData.vertices.Count > 3)
            // {
            //     // meshData.mesh.Optimize();
            //     meshCollider.sharedMesh = meshData.mesh;
            // }

            // _rp = new RenderParams[meshData.subMeshCount];
            // Material[] materials = new Material[meshData.subMeshCount];
            //     Debug.LogWarning($"meshData.subMeshCount={meshData.subMeshCount}");
            // for (int j = 0; j < meshData.subMeshCount; j++)
            // {
            //     materials[j] = material;
            //     _rp[j] = new RenderParams(material);
            //     Graphics.RenderMesh(_rp[j], meshFilter.mesh, j, Matrix4x4.Translate(Vector3.zero));
            // }
            // meshRenderer.sharedMaterials = materials;

            return meshData;
        }

        async virtual public UniTask<Mesh> UploadMeshGreedy(bool isDrawMesh)
        {
            if (!cancelTokenSrc.IsCancellationRequested)
            {
                if (meshRenderer == null)
                    ConfigureComponents();

                meshData.ClearData();

                Mesh mesh = meshData.mesh;  //meshFilter.sharedMesh;

                // meshFilter.sharedMesh = meshData.mesh;

                mesh.name = $"{meshName}_{gameObject.name}";
#if UNITY_EDITOR
                float startTime = Time.realtimeSinceStartup;
#endif
                var meshArray = Mesh.AllocateWritableMeshData(mesh);
                var _job = new MeshGreedyJob();
                _job.mesh = meshArray[0];
                _job.chunkSize = new int3(meshConfig.sOVoxelData.Bounds.x, meshConfig.sOVoxelData.Bounds.y, meshConfig.sOVoxelData.Bounds.z);
                _job.blockSize = 1;
                _job.voxelColors = arrayVoxelColors;
                _job.voxels = arrayVoxels;
                _job.ScheduleByRef().Complete();

                Mesh.ApplyAndDisposeWritableMeshData(meshArray, mesh);

                // FIXME: For some reason setting bounds directly doesn't work so this is needed as a workaround, investigate

                // mesh.Optimize();

                // mesh.RecalculateNormals();

                // mesh.RecalculateBounds();

                // mesh.UploadMeshData(false);
                mesh.RecalculateBounds();

                if (isDrawMesh)
                {
                    meshFilter.sharedMesh = mesh;

                    // _job.voxels.Dispose();
                }
                else
                {
                    // подставляем готовый меш, чтобы получить GPU Instancing в начале
                    meshFilter.sharedMesh = meshConfig.sOVoxelData.startMesh;
                }

                // if (meshFilter.sharedMesh.vertices.Length > 3)
                // {
                //     // meshData.mesh.Optimize();
                //     meshCollider.sharedMesh = meshFilter.sharedMesh; //meshData.mesh;
                // }
#if UNITY_EDITOR
                Debug.Log($"Time greedy mesh {gameObject.name}: {(Time.realtimeSinceStartup - startTime) * 1000f} ms");
#endif
            return mesh;
            }
            return default;
        }

        // void Update()
        // {
        //     Graphics.RenderMesh(_rp, meshFilter.mesh, 0, Matrix4x4.Translate(transform.position));           
        // }

        virtual protected void ConfigureComponents()
        {
            meshFilter = GetComponent<MeshFilter>();
            meshRenderer = GetComponent<MeshRenderer>();
            // meshCollider = GetComponent<MeshCollider>();
            if (!meshConfig.isOneMesh)
            {
                gPUInstanceEnabler = gameObject.AddComponent<GPUInstanceEnabler>();
            }
            // if (meshConfig.emissionValue > 0)
            // {
            //     propertyBlockChanger = gameObject.AddComponent<PropertyBlockChanger>();
            // }

            // устанавливаем режим отображения теней.
            meshRenderer.shadowCastingMode = meshConfig.shadowCastingMode;
        }

        public Voxel this[Vector3 index]
        {
            get
            {
                if (dataVoxels.ContainsKey(index))
                    return dataVoxels[index];
                else
                    return emptyVoxel;
            }

            set
            {
                if (dataVoxels.ContainsKey(index))
                    dataVoxels[index] = value;
                else
                    dataVoxels.Add(index, value);
            }
        }

        public static Voxel emptyVoxel = new Voxel() { ID = 0 };

#region My functions
        // /// <summary>
        // /// Замена цветов вершин.
        // /// </summary>
        // public void UploadColors()
        // {
        //     Dictionary<Color, int> replacements = new Dictionary<Color, int>();
            
        //     // формируем цвета из групп вокселей.
        //     for (int i = 0; i < meshConfig.sOVoxelData.groups.Count; i++)
        //     {
        //         replacements[meshConfig.sOVoxelData.groups[i].color] = i;
        //     }

        //     // проходим по всем данным.
        //     for (int i = 0; i < dataVoxels.Count; i++) {
        //         // если цвет вершины, есть в группе, получаем его индекс.
        //         if (replacements.TryGetValue(dataVoxels[i].color, out int indexNewValue))
        //         {
        //             // используем индекс, чтобы выбрать новый цвет из настроек.
        //             Voxel _vox = dataVoxels[i];
        //             _vox.color = meshConfig.color.Length > indexNewValue ? meshConfig.color[indexNewValue] : dataVoxels[i].color;
        //             dataVoxels[i] = _vox;
        //             Debug.Log($"OnSetConfig::: change color {dataVoxels[i].color}");
        //         }
        //     }
        // }

        public Voxel GetVoxel(Vector3Int pos)
        {
            Voxel voxel = default;

            if (pos.x < meshConfig.sOVoxelData.Bounds.x && pos.y < meshConfig.sOVoxelData.Bounds.y && pos.z < meshConfig.sOVoxelData.Bounds.z)
            {
                voxel = arrayVoxels[Helpers.To1D(pos.x, pos.y, pos.z, meshConfig.sOVoxelData.Bounds.x, meshConfig.sOVoxelData.Bounds.y)];
            }

            return voxel;
        }

        async public UniTask ExposionVoxels(BaseMachine ktoStrelyal, Vector3 _pointCollision, bool isDrawMesh, GameObject _explodeGameObject, float radiusExplode, Vector3 direction, Vector3 normal)
        {
            if (!meshConfig.isDestructible)
            {
                return;
            }

            if (!cancelTokenSrc.IsCancellationRequested)
            {
                float startTime = Time.realtimeSinceStartup;

                pointCollision = _pointCollision;

                // проверяем - было ли попадание в это место.
                for (int i = 0; i < prevContacts.Count; i++)
                {
                    if (Helpers.IsInsideSphere(pointCollision, prevContacts[i], radiusExplode))
                    {
                        Vector3 worldPoint = _explodeGameObject.transform.parent.TransformPoint(pointCollision);
                        var normalizeVector = direction.normalized;
                        float scaleValue = _explodeGameObject.transform.lossyScale.x;
                        // Debug.Log($"scale={_explodeGameObject.transform.lossyScale}");
                        Vector3 worldPointCollisionWithOffset = worldPoint + (normalizeVector * radiusExplode * scaleValue);
                        pointCollision = _explodeGameObject.transform.parent.InverseTransformPoint(worldPointCollisionWithOffset);
                        // Debug.Log($"point  collision {_pointCollision} => new point => {pointCollision}[direction={direction}/{normalizeVector}]");
                    }
                }
                prevContacts.Add(pointCollision);

                // explodeGameObject = _explodeGameObject;

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

                float3[] keys = dataVoxels.Keys.ToArray();
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
                JobHandle collisionJobHandle = collisionJob.ScheduleByRef(points.Length, 1);
                collisionJobHandle.Complete(); // Or use dependency

                // Debug.Log($"Time JOB create data1: {(Time.realtimeSinceStartup - startTime) * 1000f} ms. Count point={points.Count()}. ");
                List<RemoveVoxel> needCreateElements = new();
                for (int el = 0; el < collisionJob._needCreateElements.Length; el++)
                {
                    if (!collisionJob._needCreateElements[el].Equals(float3.zero))
                    {
                        needCreateElements.Add(new RemoveVoxel()
                        {
                            position = collisionJob._needCreateElements[el],
                            color = dataVoxels[collisionJob._needCreateElements[el]].color
                        });
                    }
                }
                ;
                // Debug.Log($"Time JOB create data2: {(Time.realtimeSinceStartup - startTime) * 1000f} ms. Count point={points.Count()}. ");

                int countRemoved = 0;
                for (int el = 0; el < collisionJob.needRemoveElements.Length; el++)
                {
                    if (!collisionJob.needRemoveElements[el].Equals(float3.zero))
                    {
                        float3 pos = collisionJob.needRemoveElements[el];
                        dataVoxels.Remove(pos);
                        Vector3Int posInt = Vector3Int.FloorToInt(pos);

                        var vox = arrayVoxels[Helpers.To1D(posInt.x, posInt.y, posInt.z, meshConfig.sOVoxelData.Bounds.x, meshConfig.sOVoxelData.Bounds.y)];
                        vox.type = VoxelType.Destroyed;
                        // vox = default;
                        arrayVoxels[Helpers.To1D(posInt.x, posInt.y, posInt.z, meshConfig.sOVoxelData.Bounds.x, meshConfig.sOVoxelData.Bounds.y)] = vox;

                        countRemoved++;
                    }
                };

                // Debug.Log($"collisionJob._needCreateElements={collisionJob._needCreateElements.Length}, needRemoveElements={collisionJob.needRemoveElements.Length}");

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

                // list.Add(new Vector3(x,y,z),data[new Vector3(x,y,z)]);

                // Debug.Log($"Time find exploded data: {(Time.realtimeSinceStartup - startTime) * 1000f} ms; \r\ncountRemoved={countRemoved}, needCreate={needCreateElements.Count}");
                _containerData.countVoxelsDestructible += countRemoved;
                
                // if (numberIterate < 2)
                // {
                //     await ExposionVoxels(_pointCollision, isDrawMesh, _explodeGameObject, radiusExplode, numberIterate + 1);
                // }

                // проверяем видим ли компонент в камере.
                bool isVisibleExplodeObject = false;
                Plane[] planes = GeometryUtility.CalculateFrustumPlanes(_levelManager.Camera);
                if (GeometryUtility.TestPlanesAABB(planes, meshRenderer.bounds))
                {
                    isVisibleExplodeObject = true;
                }

                // если прошли предыдущую проверку, выполняем райкаст, чтобы проверить препятствия.
                if (isVisibleExplodeObject)
                {
                    Vector3 worldPointCollision = transform.TransformPoint(_pointCollision);
                    Vector3 dir = (worldPointCollision - _levelManager.Camera.transform.position).normalized;
                    float distance = Vector3.Distance(worldPointCollision, _levelManager.Camera.transform.position);

                    if (Physics.Raycast(_levelManager.Camera.transform.position, dir, out RaycastHit hit, distance, LayerMask.GetMask("Wall", "Machine", "Build", "Nature")))
                    {
                        // Debug.DrawRay(_levelManager.Camera.transform.position, dir * distance, Color.yellow,10);
                        // Debug.Log($"hits:::::{hit.transform.name}/{transform.name}");
                        if (hit.transform != transform)
                        {
                            isVisibleExplodeObject = false;
                        }
                    }
                }
                
                // if (!isVisibleExplodeObject)
                // {
                //     Debug.Log($"container not visible");
                // } else
                // {
                //     Debug.Log($"container visible");
                // }

                await CheckTopAirVoxels(_explodeGameObject, isVisibleExplodeObject);

                if (dataVoxels.Count >= 10 && needCreateElements.Count > 0)
                {
                    if (!meshConfig.isGreedy)
                    {
                        GenerateMesh();
                        UploadMesh(isDrawMesh);
                        // Debug.Log("Time generate mesh: " + (Time.realtimeSinceStartup - temp).ToString("f6"));
                    }
                    else
                    {
                        UploadMeshGreedy(isDrawMesh).Forget();
                    }
                }

                BaseMachine bm = _explodeGameObject.transform.GetComponentInParent<BaseMachine>();
                if (bm)
                {
                    bm.RefreshHP();

                    if (bm.Data.ContainerData.levelDestruction > 0.85f)
                    {
                        GameSceneEvents.AddInfoDamage(new AppInfoDamageData
                        {
                            kto = ktoStrelyal,
                            komy = bm,
                            userText = "",
                            duration = 2f
                        });
                    }
                }


                if (needCreateElements.Count > 0 && isVisibleExplodeObject)
                {
                    // Debug.Log($"Time upload mesh: {(Time.realtimeSinceStartup - startTime) * 1000f} ms");
                    // Debug.Log("Time upload mesh: " + (Time.realtimeSinceStartup - temp).ToString("f6"));
                    // Debug.Log($"needCreateElements {needCreateElements.Count} voxels!");
                    // StartCoroutine(createGO());
                    await CreateECS(needCreateElements, Mathf.Min(radiusExplode, GameManager.Instance.Settings.maxRadiusCreateVoxels), direction, normal);
                }


                if (dataVoxels.Count() < 10)
                {
                    transform.gameObject.SetActive(false);
                }

                _needCreateElements.Dispose();
                _needRemoveElements.Dispose();
                points.Dispose();
            }
        }

        async private UniTask CheckTopAirVoxels(GameObject _explodeGameObject, bool isVisibleExplodeObject)
        {
            if (!cancelTokenSrc.IsCancellationRequested)
            {
                float startTime = Time.realtimeSinceStartup;
                List<KeyValuePair<float3, Voxel>> airVoxels = new List<KeyValuePair<float3, Voxel>>();
                // var groupY = arrayVoxels.GroupBy(x => x.position.y);
                // Debug.Log($"groupY: {groupY.Count()}");

                float maxY = -1f;
                // for (int y = 0; y < meshConfig.sOVoxelData.Bounds.y; y++)
                // {
                //     var isY = arrayVoxels.FirstOrDefault(x => x.position.y == y && x.type != VoxelType.Destroyed && x.type != VoxelType.Air);

                //     if (isY.type == default)
                //     {
                //         maxY = y;
                //         break;
                //     }
                // }
                // Debug.Log($"maxY={maxY}");

                // // filter jobs.
                // JobHandle lastJobHandle = default;
                // for (int y = 0; y < meshConfig.sOVoxelData.Bounds.y; y++)
                // {
                //     NativeList<int> filteredIndices = new NativeList<int>(Allocator.TempJob);
                //     FilterYJob filterJob = new FilterYJob
                //     {
                //         dataToFilter = arrayVoxels,
                //         y = y
                //     };

                //     // ScheduleAppend добавит индексы, которые возвращают true в Execute, в filteredIndices.
                //     lastJobHandle = filterJob.ScheduleAppend(filteredIndices, arrayVoxels.Length, lastJobHandle);
                //     lastJobHandle.Complete(); // Дождитесь завершения работы.

                //     // int countY = data.Where(v => (int)v.Key.y == y).Count();

                //     if (filteredIndices.Length == 0)
                //     {
                //         maxY = y;
                //     }

                //     // if (maxY != -1)
                //     // {
                //     //     for (int i = 0; i < filteredIndices.Length; i++)
                //     //     {
                //     //         // airVoxels.Add(arrayVoxels[filteredIndices[i]].position, arrayVoxels[filteredIndices[i]]);
                //     //         needGravityCreateElements.Push(new RemoveVoxel()
                //     //         {
                //     //             color = arrayVoxels[filteredIndices[i]].color,
                //     //             position = arrayVoxels[filteredIndices[i]].position
                //     //         });
                //     //     }
                //     // }

                //     filteredIndices.Dispose();

                //     if (maxY != -1)
                //     {
                //         break;
                //     }
                //     // // bool noBottomVoxel = false;
                //     // int countVoxel = 0;

                //     // for (int x = 0; x < meshConfig.sOVoxelData.Bounds.z; x++) // Проход по второму измерению (Y)
                //     // {

                //     //     for (int z = 0; z < meshConfig.sOVoxelData.Bounds.y; z++) // Проход по третьему измерению (Z)
                //     //     {
                //     //         Voxel voxel = arrayVoxels[Helpers.To1D(y, z, x, meshConfig.sOVoxelData.Bounds.x, meshConfig.sOVoxelData.Bounds.y)];
                //     //         if (voxel.type.HasFlag(VoxelType.Destroyed))
                //     //         {
                //     //             noBottomVoxel = true;
                //     //         }

                //     //         if (noBottomVoxel && !voxel.type.HasFlag(VoxelType.Destroyed))
                //     //         {
                //     //             airVoxels.Add(voxel);
                //     //             voxel.type = VoxelType.Destroyed;
                //     //             // vox = default;
                //     //             arrayVoxels[Helpers.To1D(y, z, x, meshConfig.sOVoxelData.Bounds.x, meshConfig.sOVoxelData.Bounds.y)] = voxel;
                //     //         }
                //     //     }
                //     // }
                // }


                // find jobs.
                JobHandle lastJobHandle = default;
                for (int y = 0; y < meshConfig.sOVoxelData.Bounds.y; y++)
                {
                    NativeArray<bool> foundResult = new NativeArray<bool>(1, Allocator.TempJob);
                    FilterYJobFind findJob = new FilterYJobFind
                    {
                        data = arrayVoxels,
                        y = y,
                        found = foundResult
                    };


                    lastJobHandle = findJob.Schedule(lastJobHandle);
                    lastJobHandle.Complete();

                    if (!foundResult[0])
                    {
                        maxY = y;
                    }

                    // if (maxY != -1)
                    // {
                    //     for (int i = 0; i < filteredIndices.Length; i++)
                    //     {
                    //         // airVoxels.Add(arrayVoxels[filteredIndices[i]].position, arrayVoxels[filteredIndices[i]]);
                    //         needGravityCreateElements.Push(new RemoveVoxel()
                    //         {
                    //             color = arrayVoxels[filteredIndices[i]].color,
                    //             position = arrayVoxels[filteredIndices[i]].position
                    //         });
                    //     }
                    // }

                    foundResult.Dispose();

                    if (maxY != -1)
                    {
                        break;
                    }
                    // // bool noBottomVoxel = false;
                    // int countVoxel = 0;

                    // for (int x = 0; x < meshConfig.sOVoxelData.Bounds.z; x++) // Проход по второму измерению (Y)
                    // {

                    //     for (int z = 0; z < meshConfig.sOVoxelData.Bounds.y; z++) // Проход по третьему измерению (Z)
                    //     {
                    //         Voxel voxel = arrayVoxels[Helpers.To1D(y, z, x, meshConfig.sOVoxelData.Bounds.x, meshConfig.sOVoxelData.Bounds.y)];
                    //         if (voxel.type.HasFlag(VoxelType.Destroyed))
                    //         {
                    //             noBottomVoxel = true;
                    //         }

                    //         if (noBottomVoxel && !voxel.type.HasFlag(VoxelType.Destroyed))
                    //         {
                    //             airVoxels.Add(voxel);
                    //             voxel.type = VoxelType.Destroyed;
                    //             // vox = default;
                    //             arrayVoxels[Helpers.To1D(y, z, x, meshConfig.sOVoxelData.Bounds.x, meshConfig.sOVoxelData.Bounds.y)] = voxel;
                    //         }
                    //     }
                    // }
                }




                List<RemoveVoxel> needGravityCreateElements = new();

                if (maxY > -1)
                {
                    airVoxels = dataVoxels.Where(v => v.Key.y > maxY && !v.Value.type.HasFlag(VoxelType.Destroyed)).OrderBy(t => -t.Key.y).ToList();//.ToDictionary(t => t.Key, t => t.Value);

                    // Debug.Log($"Time checkAirVoxels2: {(Time.realtimeSinceStartup - startTime) * 1000f}. \r\n airVoxels.count={needGravityCreateElements.Count}");


                    for (int i = 0; i < airVoxels.Count; i++)
                    {
                        var voxelItem = airVoxels.ElementAt(i);
                        Vector3Int pos = Vector3Int.FloorToInt(voxelItem.Value.position);

                        var index = Helpers.To1D(pos.x, pos.y, pos.z, meshConfig.sOVoxelData.Bounds.x, meshConfig.sOVoxelData.Bounds.y);

                        var voxelItem2 = arrayVoxels[index];
                        voxelItem2.type = VoxelType.Destroyed;
                        // vox = default;
                        arrayVoxels[index] = voxelItem2;
                        dataVoxels.Remove(voxelItem.Key);

                        needGravityCreateElements.Add(new RemoveVoxel()
                        {
                            position = voxelItem2.position,
                            color = voxelItem2.color
                        });
                    }

                    // Обновляем данные о разрушенных вокселях.
                    _containerData.countVoxelsDestructible += airVoxels.Count();
                    
                    BaseMachine bm = _explodeGameObject.transform.GetComponentInParent<BaseMachine>();
                    if (bm)
                    {
                        bm.RefreshHP();
                    }
                }

                // Debug.Log($"Time checkAirVoxels1: {(Time.realtimeSinceStartup - startTime) * 1000f}. \r\n airVoxels.count={needGravityCreateElements.Count}, maxY={maxY}");

                // Debug.Log($"Time checkAirVoxels2: {(Time.realtimeSinceStartup - startTime) * 1000f}. \r\n airVoxels.count={needGravityCreateElements.Count}, maxY={maxY}");



                if (needGravityCreateElements.Count > 0 && isVisibleExplodeObject)
                {
                    // CreateGravityObjectsAsync().Forget();
                    // await UniTask.NextFrame();
                    CreateGravityECS(needGravityCreateElements).Forget();
                }


                // // Debug.Log($"airVoxels.count={airVoxels.Count}");


                // if (airVoxels.Count > 0)
                // {
                //     if (!meshConfig.isGreedy)
                //     {
                //         GenerateMesh();
                //         UploadMesh(true);
                //         // Debug.Log("Time generate mesh: " + (Time.realtimeSinceStartup - temp).ToString("f6"));
                //     }
                //     else
                //     {
                //         UploadMeshGreedy(true);
                //     }
                // }


                // Debug.Log($"Time checkAirVoxels: {(Time.realtimeSinceStartup - startTime) * 1000f}. \r\n airVoxels.count={airVoxels.Count}");
            }
        }

        // public Dictionary<Vector3, Voxel> ExposionVoxels2(Vector3 _pointCollision, bool isDrawMesh, Collision _collision)
        // {
        //     float startTime = Time.realtimeSinceStartup;

        //     collision = _collision;
        //     pointCollision = _pointCollision;

        //     // // var meshDataArray = Mesh.AcquireReadOnlyMeshData(meshFilter.sharedMesh);
        //     // // var meshData = meshDataArray[0];
        //     // // Create and schedule the job
        //     // var l = new NativeArray<Vector3>(data.Count, Allocator.Persistent);
        //     // var ls = new NativeArray<byte>(data.Count, Allocator.Persistent);

        //     // for (int j = 0; j < data.Count; j++)
        //     // {
        //     //     l[j] = data.ElementAt(j).Key;
        //     //     ls[j] = data.ElementAt(j).Value.ID;
        //     // }

        //     // // vertices = new NativeArray<Vector3>(meshData.vertices.Count, Allocator.Persistent);
        //     // var newVertices = new NativeArray<Vector3>(meshData.vertices.Count, Allocator.Persistent);
        //     // // for (int r = 0; r < meshData.vertices.Count; r++)
        //     // // {
        //     // //     vertices[r] = meshData.vertices[r];
        //     // // }

        //     // // triangles = new NativeArray<int>(meshData.triangles.Count, Allocator.Persistent);
        //     // var newTriangles = new NativeArray<int>(meshData.triangles.Count, Allocator.Persistent);
        //     // // for (int r = 0; r < meshData.triangles.Count; r++)
        //     // // {
        //     // //     triangles[r] = meshData.triangles[r];
        //     // // }

        //     var vertices = new NativeList<Vector3>(Allocator.Persistent);
        //     var triangles = new NativeList<int>(Allocator.Persistent);
        //     var uVs = new NativeList<Vector2>(Allocator.Persistent);
        //     var colors = new NativeList<Color>(Allocator.Persistent);
        //     var uVs2 = new NativeList<Vector2>(Allocator.Persistent);



        //     NativeHashMap<Vector3, Voxel> _data2 = new NativeHashMap<Vector3, Voxel>(data.Count, Allocator.Persistent);
        //     NativeHashMap<Vector3, Voxel> _data = new NativeHashMap<Vector3, Voxel>(data.Count, Allocator.Persistent);
        //     for (int b = 0; b < data.Count; b++)
        //     {
        //         KeyValuePair<Vector3, Voxel> el = data.ElementAt(b);
        //         _data[el.Key] = new Voxel() { ID = 1 };
        //     }


        //     NativeArray<int> voxelVertexIndex2 = new NativeArray<int>(new int[]{
        //                 0,1,2,3,
        //                 4,5,6,7,
        //                 4,0,6,2,
        //                 5,1,7,3,
        //                 0,1,4,5,
        //                 2,3,6,7,
        //                 }, Allocator.Persistent);
        //     NativeArray<int> voxelTris2 = new NativeArray<int>(new int[36]
        //         {
        //                 0,2,3,0,3,1,
        //                 0,1,2,1,3,2,
        //                 0,2,3,0,3,1,
        //                 0,1,2,1,3,2,
        //                 0,1,2,1,3,2,
        //                 0,2,3,0,3,1,
        //         }, Allocator.Persistent);

        //     NativeArray<Vector3> faceVertices = new NativeArray<Vector3>(4, Allocator.Persistent);
        //     NativeArray<Vector2> faceUVs = new NativeArray<Vector2>(4, Allocator.Persistent);
        //     NativeArray<VoxelColor> worldColors = new NativeArray<VoxelColor>(WorldManager.Instance.WorldColors, Allocator.Persistent);

        //     ModifyMeshJob modifyMeshJob = new ModifyMeshJob
        //     {
        //         // vertexData = meshData.GetVertexData<Vertex>(),
        //         _data = _data,
        //         _data2 = _data2,
        //         colors = colors,
        //         uVs = uVs,
        //         uVs2 = uVs2,
        //         worldColors = worldColors,
        //         vertices = vertices,
        //         faceUVs = faceUVs,
        //         faceVertices = faceVertices,
        //         triangles = triangles,
        //         // newTriangles = newTriangles,
        //         // newVertices = newVertices,
        //         PointCollision = pointCollision,
        //         // removedVertexIndices = new NativeArray<int>(vertices.Length, Allocator.Persistent),
        //         // vertexRemap = new NativeArray<int>(vertices.Length, Allocator.Persistent),
        //         voxelTris2 = voxelTris2,
        //         voxelVertexIndex2 = voxelVertexIndex2,
        //         // vertexData = meshData.GetVertexData<Vertex>(),
        //         // indexData = meshData.GetIndexData<uint>(),
        //         // ... other parameters
        //     };

        //     JobHandle jobHandle = modifyMeshJob.Schedule();
        //     jobHandle.Complete(); // Or use dependency

        //     // Apply and dispose

        //     meshData.ClearData();


        //     // Debug.Log($"count vertices.length={vertices.Length}, modifyMeshJob.vertices.Length={modifyMeshJob.vertices.Length}");
        //     data.Clear();
        //     foreach (var d in _data2)
        //     {
        //         KVPair<Vector3, Voxel> dat = d;
        //         data.Add(dat.Key, dat.Value);
        //     }

        //     meshData.vertices = modifyMeshJob.vertices.AsArray().ToList();
        //     meshData.UVs = modifyMeshJob.uVs.AsArray().ToList();
        //     meshData.colors = modifyMeshJob.colors.AsArray().ToList();
        //     meshData.UVs2 = modifyMeshJob.uVs2.AsArray().ToList();
        //     meshData.triangles = modifyMeshJob.triangles.AsArray().ToList();
        //     // Debug.Log($"count vertices={modifyMeshJob.vertices.Length}, meshData.vertices.count={meshData.vertices.Count}, meshData.triangles.count={meshData.triangles.Count}");


        //     // Mesh mesh = new Mesh { name = "Procedural Mesh" };
        //     // mesh.vertices = newVertices.ToArray(); //SetVertexBufferData(newVertices, 0, 0, newVertices.Length);
        //     // mesh.triangles = newTriangles.ToArray(); // mesh.SetTriangles(newTriangles.ToArray(), 0);
        //     // // Mesh.ApplyAndDisposeWritableMeshData(meshDataArray, mesh);
        //     // // GetComponent<MeshFilter>().mesh = mesh;

        //     var list = new Dictionary<Vector3, Voxel>();
        //     // // var x = Mathf.RoundToInt(pos.x);
        //     // // var y = Mathf.RoundToInt(pos.y);
        //     // // var z = Mathf.RoundToInt(pos.z);
        //     // // Debug.Log($"Vector3Int {new Vector3(x, y, z)}, [{pos}]");

        //     // // Voxel first;
        //     // // List<Vector3> checkPositions = new List<Vector3>()
        //     // //     {
        //     // //         new Vector3(x, y, z),
        //     // //         new Vector3(x+1, y, z),
        //     // //         new Vector3(x-1, y, z),
        //     // //         new Vector3(x, y+1, z),
        //     // //         new Vector3(x, y-1, z),
        //     // //         new Vector3(x, y, z+1),
        //     // //         new Vector3(x, y, z-1)
        //     // //     };

        //     // for (int j = 0; j < data.Keys.Count; j++)
        //     // {
        //     //     Vector3 posx = data.ElementAt(j).Key;
        //     //     if (TestHelpers.IsInsideSphere(posx, _pointCollision, 3))
        //     //     {
        //     //         list.Add(posx, data.ElementAt(j).Value);
        //     //         data.Remove(posx);
        //     //         needCreateElements.Push(posx);
        //     //     }
        //     // }
        //     // Debug.Log("Time for create data: " + (Time.realtimeSinceStartup - temp).ToString("f6"));

        //     // // list.Add(new Vector3(x,y,z),data[new Vector3(x,y,z)]);
        //     // GenerateMesh();
        //     Debug.Log($"Time jobs: {(Time.realtimeSinceStartup - startTime) * 1000f} ms");

        //     // Debug.Log("Time generate mesh: " + (Time.realtimeSinceStartup - temp).ToString("f6"));
        //     UploadMesh(isDrawMesh);

        //     // Debug.Log("Time upload mesh: " + (Time.realtimeSinceStartup - temp).ToString("f6"));
        //     // Debug.Log($"Exploded {list.Count} voxels!");

        //     // // StartCoroutine(createGO());


        //     Debug.Log($"Time upload mesh: {(Time.realtimeSinceStartup - startTime) * 1000f} ms");
        //     _data.Dispose();
        //     voxelTris2.Dispose();
        //     vertices.Dispose();
        //     triangles.Dispose();
        //     voxelVertexIndex2.Dispose();
        //     worldColors.Dispose();
        //     colors.Dispose();
        //     uVs.Dispose();
        //     uVs2.Dispose();
        //     faceVertices.Dispose();
        //     faceUVs.Dispose();

        //     return list;
        // }

        public async UniTask CreateECS(List<RemoveVoxel> needCreateElements, float radiusExplode, Vector3 direction, Vector3 normal)
        {
            if (!cancelTokenSrc.IsCancellationRequested)
            {
                float startTime = Time.realtimeSinceStartup;

                needCreateElements = needCreateElements.OrderBy(t => UnityEngine.Random.value).ToList();
                List<ECSDataSpawn> listData = new List<ECSDataSpawn>();
                var maxCount = Mathf.Min(
                    GameManager.Instance.Settings.DebugSettings.mode == AppMode.Mobile ? GameManager.Instance.Settings.countMaxCreateVoxelsByStepMobile : GameManager.Instance.Settings.countMaxCreateVoxelsByStep,
                    needCreateElements.Count
                );

                for (int i = 0; i < maxCount; i++)
                {
                    var reflexDirection = Vector3.Reflect(direction, normal);
                    var rot = Quaternion.Euler(
                        UnityEngine.Random.Range(0, 90),
                        UnityEngine.Random.Range(0, 90),
                        UnityEngine.Random.Range(0, 90)
                    );
                    var dir = rot * reflexDirection;
                    listData.Add(new ECSDataSpawn
                    {
                        color = needCreateElements[i].color,
                        direction = i % 50 != 0 ?  dir.normalized : UnityEngine.Random.onUnitSphere.normalized, // UnityEngine.Random.onUnitSphere,
                        forceAmount = UnityEngine.Random.Range(radiusExplode, 100 * radiusExplode),
                        lifetimeRemaining = UnityEngine.Random.Range(.3f, 1f),
                        position = transform.TransformPoint(needCreateElements[i].position),
                        scale = GameManager.Instance.Settings.scaleObjects
                    });
                }

                // Debug.Log($"Time CreateGravityECS: {(Time.realtimeSinceStartup - startTime) * 1000f} ms. \r\nCount  = {listData.Count}");
                // Debug.Log($"CreateGravityECS: {listData.Count}");

                // await UniTask.NextFrame();
                _levelManager.ECSManager.UpdateDataDots(listData).Forget();
                // Debug.Log($"Time CreateESC: {(Time.realtimeSinceStartup - startTime) * 1000f} ms, CreateECS: {maxCount}");
            }
        }

        // public async UniTask CreateObjectsAsync()
        // {
        //     int count = GameManager.Instance.Settings.countCreateVoxelByFrame;

        //     while (needCreateElements.Count > 0)
        //     {
        //         RemoveVoxel elem = needCreateElements.Pop();

        //         float forceMagnitude = 10 * 30;
        //         // GameObject gObj = Instantiate(GameManager.Instance.Settings.prefabVoxel, Machine.levelManager.objectSpawnEffect.transform);
        //         GameObject gObj = Lean.Pool.LeanPool.Spawn(GameManager.Instance.Settings.prefabVoxel, _levelManager.objectSpawnEffect.transform);
        //         Vector3 pointSpawnVoxel = transform.TransformPoint(elem.position);
        //         gObj.transform.SetPositionAndRotation(pointSpawnVoxel, Quaternion.identity);
        //         var voxPrefab = gObj.GetComponent<VoxelPrefab>();
        //         voxPrefab.Init(meshConfig.sOVoxelData);
        //         voxPrefab.SetColor(elem.color);
        //         // gObj.isStatic = true;
        //         // gObj.transform.SetLocalPositionAndRotation(listVoxels.ElementAt(k).Key, Quaternion.identity);
        //         // gObj.gameObject.AddComponent<BoxCollider>();


        //         // var mat = gObj.gameObject.GetComponent<MeshRenderer>().material;
        //         // var mesh = gObj.gameObject.GetComponent<MeshFilter>().mesh;
        //         // RenderParams _rp = new RenderParams(WorldManager.Instance.worldMaterial);
        //         // Graphics.RenderMesh(_rp, mesh, 0, Matrix4x4.Translate(pointSpawnVoxel));

        //         var r = gObj.gameObject.GetComponent<Rigidbody>();
        //         if (r == null)
        //         {
        //             r = gObj.gameObject.AddComponent<Rigidbody>();
        //         }
        //         r.collisionDetectionMode = CollisionDetectionMode.Continuous;
        //         r.mass = 100f;
        //         r.useGravity = true;
        //         var forceDirection = UnityEngine.Random.onUnitSphere; //Vector3.Scale(UnityEngine.Random.onUnitSphere, transform.forward);
        //         r.AddForce(forceDirection * forceMagnitude, ForceMode.Impulse);
        //         // gameObjects[count - 1] = gObj;
        //         // gObj.isStatic = false;
        //         // Destroy(gObj, 15);
        //         Lean.Pool.LeanPool.Despawn(gObj, UnityEngine.Random.Range(1, 3));


        //         // // simulate paraboloid.
        //         // var forceDirection = UnityEngine.Random.onUnitSphere;
        //         // float time = UnityEngine.Random.Range(1, 5);
        //         // gObj.Init(forceDirection * 10, UnityEngine.Random.onUnitSphere, time * 0.5f);
        //         // Lean.Pool.LeanPool.Despawn(gObj, time);

        //         count--;

        //         if (count < 0)
        //         {
        //             count = GameManager.Instance.Settings.countCreateVoxelByFrame;
        //             await UniTask.NextFrame();
        //         }
        //     }

        // }

        async private UniTaskVoid CreateGravityECS(List<RemoveVoxel> needGravityCreateElements)
        {
            if (!cancelTokenSrc.IsCancellationRequested)
            {
                float startTime = Time.realtimeSinceStartup;

                needGravityCreateElements = needGravityCreateElements.OrderBy(t => UnityEngine.Random.value).ToList();
                List<ECSDataSpawn> listData = new List<ECSDataSpawn>();
                var maxCount = Mathf.Min(
                    GameManager.Instance.Settings.DebugSettings.mode == AppMode.Mobile ? GameManager.Instance.Settings.countMaxCreateVoxelsByStepMobile : GameManager.Instance.Settings.countMaxCreateVoxelsByStep,
                    needGravityCreateElements.Count
                ); //needGravityCreateElements.Count; //Mathf.Min(200, needGravityCreateElements.Count);

                for (int i = 0; i < maxCount; i++)
                {
                    listData.Add(new ECSDataSpawn
                    {
                        color = needGravityCreateElements[i].color,
                        direction = UnityEngine.Random.onUnitSphere.normalized,
                        forceAmount = 0, //UnityEngine.Random.Range(20000, 30000),
                        lifetimeRemaining = UnityEngine.Random.Range(.3f, 1f),
                        position = transform.TransformPoint(needGravityCreateElements[i].position),
                        scale = GameManager.Instance.Settings.scaleObjects
                    });
                }

                // Debug.Log($"Time CreateGravityECS: {(Time.realtimeSinceStartup - startTime) * 1000f} ms");
                // Debug.Log($"CreateGravityECS: {listData.Count}");

                // await UniTask.NextFrame();
                await _levelManager.ECSManager.UpdateDataDots(listData);
                // Debug.Log($"Time GenerateDots: {(Time.realtimeSinceStartup - startTime) * 1000f} ms, CreateGravityECS: {maxCount}");
            }
        }

        // public async UniTask CreateGravityObjectsAsync()
        // {
        //     int count = GameManager.Instance.Settings.countCreateVoxelByFrame;

        //     while (needGravityCreateElements.Count > 0)
        //     {
        //         RemoveVoxel elem = needGravityCreateElements.Pop();

        //         float forceMagnitude = 10;
        //         // GameObject gObj = Instantiate(GameManager.Instance.Settings.prefabVoxel, Machine.levelManager.objectSpawnEffect.transform);
        //         GameObject gObj = Lean.Pool.LeanPool.Spawn(GameManager.Instance.Settings.prefabVoxel, _levelManager.objectSpawnEffect.transform);
        //         Vector3 pointSpawnVoxel = transform.TransformPoint(elem.position);
        //         gObj.transform.SetPositionAndRotation(pointSpawnVoxel, Quaternion.identity);
        //         var voxPrefab = gObj.GetComponent<VoxelPrefab>();
        //         voxPrefab.Init(meshConfig.sOVoxelData);
        //         voxPrefab.SetColor(elem.color);
        //         // gObj.isStatic = true;
        //         // gObj.transform.SetLocalPositionAndRotation(listVoxels.ElementAt(k).Key, Quaternion.identity);
        //         // gObj.gameObject.AddComponent<BoxCollider>();


        //         // var mat = gObj.gameObject.GetComponent<MeshRenderer>().material;
        //         // var mesh = gObj.gameObject.GetComponent<MeshFilter>().mesh;
        //         // RenderParams _rp = new RenderParams(WorldManager.Instance.worldMaterial);
        //         // Graphics.RenderMesh(_rp, mesh, 0, Matrix4x4.Translate(pointSpawnVoxel));

        //         var r = gObj.gameObject.GetComponent<Rigidbody>();
        //         if (r == null)
        //         {
        //             r = gObj.gameObject.AddComponent<Rigidbody>();
        //         }
        //         r.collisionDetectionMode = CollisionDetectionMode.Continuous;
        //         // r.mass = 1f;
        //         r.useGravity = true;
        //         var forceDirection = transform.up;// UnityEngine.Random.onUnitSphere; //Vector3.Scale(UnityEngine.Random.onUnitSphere, transform.forward);
        //         r.AddForce(forceDirection * forceMagnitude, ForceMode.Impulse);
        //         // gameObjects[count - 1] = gObj;
        //         // gObj.isStatic = false;
        //         // Destroy(gObj, 15);
        //         Lean.Pool.LeanPool.Despawn(gObj, UnityEngine.Random.Range(1, 3));


        //         // // simulate paraboloid.
        //         // var forceDirection = UnityEngine.Random.onUnitSphere;
        //         // float time = UnityEngine.Random.Range(1, 5);
        //         // gObj.Init(forceDirection * 10, UnityEngine.Random.onUnitSphere, time * 0.5f);
        //         // Lean.Pool.LeanPool.Despawn(gObj, time);

        //         count--;

        //         if (count < 0)
        //         {
        //             count = GameManager.Instance.Settings.countCreateVoxelByFrame;
        //             await UniTask.NextFrame();
        //         }
        //     }

        // }

        public bool IsDestructible()
        {
            return meshConfig.isDestructible;
        }

        // private IEnumerator createGO()
        // {
        //     int count = 10;
        //     // GameObject[] gameObjects = new GameObject[count];
        //     while (needCreateElements.Count > 0 && count > 0)
        //     {
        //         Vector3 elem = needCreateElements.Pop();

        //         float forceMagnitude = 10 * 1000;
        //         // GameObject gObj = Instantiate(GameManager.Instance.Settings.prefabVoxel, Machine.levelManager.objectSpawnEffect.transform);
        //         GameObject gObj = Lean.Pool.LeanPool.Spawn(GameManager.Instance.Settings.prefabVoxel, _levelManager.objectSpawnEffect.transform);
        //         Vector3 pointSpawnVoxel = explodeGameObject.transform.TransformPoint(elem);
        //         gObj.transform.SetPositionAndRotation(pointSpawnVoxel, Quaternion.identity);
        //         // gObj.isStatic = true;
        //         // gObj.transform.SetLocalPositionAndRotation(listVoxels.ElementAt(k).Key, Quaternion.identity);
        //         // gObj.gameObject.AddComponent<BoxCollider>();


        //         // var mat = gObj.gameObject.GetComponent<MeshRenderer>().material;
        //         // var mesh = gObj.gameObject.GetComponent<MeshFilter>().mesh;
        //         // RenderParams _rp = new RenderParams(WorldManager.Instance.worldMaterial);
        //         // Graphics.RenderMesh(_rp, mesh, 0, Matrix4x4.Translate(pointSpawnVoxel));

        //         var r = gObj.gameObject.GetComponent<Rigidbody>();
        //         if (r == null)
        //         {
        //             r = gObj.gameObject.AddComponent<Rigidbody>();
        //         }
        //         r.collisionDetectionMode = CollisionDetectionMode.Continuous;
        //         // r.mass = 100f;
        //         r.useGravity = true;
        //         var forceDirection = UnityEngine.Random.onUnitSphere; //Vector3.Scale(UnityEngine.Random.onUnitSphere, transform.forward);
        //         r.AddForce(forceDirection * forceMagnitude, ForceMode.Impulse);
        //         // gameObjects[count - 1] = gObj;
        //         // gObj.isStatic = false;
        //         // Destroy(gObj, 15);
        //         Lean.Pool.LeanPool.Despawn(gObj, UnityEngine.Random.Range(1, 5));


        //         // // simulate paraboloid.
        //         // var forceDirection = UnityEngine.Random.onUnitSphere;
        //         // float time = UnityEngine.Random.Range(1, 5);
        //         // gObj.Init(forceDirection * 10, UnityEngine.Random.onUnitSphere, time * 0.5f);
        //         // Lean.Pool.LeanPool.Despawn(gObj, time);
        //     }
        //     // StaticBatchingUtility.Combine(gameObjects, _levelManager.objectSpawnEffect.gameObject);

        //     count--;
        //     if (count < 0)
        //     {
        //         count = 20;
        //         yield return null;
        //     }

        // }

        #endregion

        // [BurstCompile]
        // public struct Vertex
        // {
        //     public Vector3 position;
        //     public Vector2 uv;
        //     public static readonly VertexAttributeDescriptor[] Layout = {
        //         new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 3),
        //         new VertexAttributeDescriptor(VertexAttribute.TexCoord0, VertexAttributeFormat.Float32, 2)
        //     };
        // }

        // [BurstCompile]
        // struct GenerateMeshJob : IJob
        // {
        //     public int resolution;
        //     public NativeArray<Vertex> vertexData;
        //     public NativeArray<uint> indexData;
        //     // ... other data

        //     public void Execute()
        //     {
        //         // ... Generate vertices and UVs
        //         // ... Calculate indices
        //         // ... Populate vertexData and indexData
        //     }
        // }


        //     [BurstCompile]
        // public struct ModifyMeshJob : IJob
        // {
        //         // public NativeArray<Vertex> vertexData;
        //     [WriteOnly]
        //     public NativeHashMap<Vector3, Voxel> _data2;
        //     public NativeHashMap<Vector3, Voxel> _data;
        //     // public NativeArray<Vector3> data;
        //     public NativeList<Vector3> vertices;
        //     public NativeList<int> triangles;
        //     public NativeList<Vector2> uVs;
        //     public NativeList<Vector2> uVs2;
        //     public NativeList<Color> colors;
        //     // public NativeArray<int> newTriangles;
        //     // public NativeArray<Vector3> newVertices;
        //         public NativeArray<Vector3> faceVertices;
        //         public NativeArray<Vector2> faceUVs;
        //     // public float removeThreshold;
        //         // public NativeArray<int> removedVertexIndices; // Output: Индексы удаленных вершин
        //     // public NativeArray<int> vertexRemap; // Output: Переопределяет старые индексы вершин в новые индексы вершин.
        //     public Vector3 PointCollision; // Точка в которой возникла коллизия.
        //         public NativeArray<int> voxelVertexIndex2;
        //         public NativeArray<int> voxelTris2;
        //         public NativeArray<VoxelColor> worldColors;

        //         public void Execute()
        //         {
        //             // // Modify vertex position
        //             // // vertices[i] = vertices[i] + new Vector3(0.1f, 0, 0);

        //             // // detect distance of by point collision.
        //             // Vector3 posx = vertices[i];
        //             // if (TestHelpers.IsInsideSphere(vertices[i], PointCollision, 3))
        //             // {
        //             //     // list.Add(posx, data.ElementAt(j).Value);
        //             //     // data.Remove(posx);
        //             //     // needCreateElements.Push(posx);
        //             //     vertices[i] = Vector3.zero;
        //             // }

        //             // // Modify normals, colors, etc. if needed
        //             // // normals[i] = ...;
        //             // // colors[i] = ...;

        //             Vector3 blockPos;
        //             Voxel block;

        //             int counter = 0;

        //             VoxelColor voxelColor;
        //             Color voxelColorAlpha;
        //             Vector2 voxelSmoothness;

        //             // NativeList<Vector3> vertices2 = new NativeList<Vector3>(vertices.Length, Allocator.Persistent);
        //             // NativeList<int> triangles2 = new NativeList<int>(triangles.Length, Allocator.Persistent);

        //             foreach (KVPair<Vector3, Voxel> kvp in _data)
        //             {
        //                 if (TestHelpers.IsInsideSphere(kvp.Key, PointCollision, 3))
        //                 {
        //                     continue;
        //                 }

        //                 // Only check on solid blocks
        //                 if (!kvp.Value.isSolid)
        //                 {
        //                     continue;
        //                 }


        //                 blockPos = kvp.Key;
        //                 block = kvp.Value;
        //                 _data2.Add(blockPos, block);

        //                 // Debug.Log($"step job {blockPos.ToString()}");

        //                 voxelColor = worldColors[block.ID - 1];
        //                 voxelColorAlpha = voxelColor.color;
        //                 voxelColorAlpha.a = 1;
        //                 voxelSmoothness = new Vector2(voxelColor.metallic, voxelColor.smoothness);
        //                 //Iterate over each face direction
        //                 for (int i = 0; i < 6; i++)
        //                 {
        //                     //Check if there's a solid block against this face
        //                     Voxel vox;
        //                     if (_data.TryGetValue(blockPos + voxelFaceChecks[i], out vox))
        //                     {
        //                         if (vox.isSolid)
        //                         {
        //                             continue;
        //                         }
        //                     }

        //                     //Draw this face

        //                     //Collect the appropriate vertices from the default vertices and add the block position
        //                     for (int j = 0; j < 4; j++)
        //                     {
        //                         faceVertices[j] = voxelVertices[voxelVertexIndex2[j + i * 4]] + blockPos;
        //                         faceUVs[j] = voxelUVs[j];
        //                     }

        //                     for (int j = 0; j < 6; j++)
        //                     {
        //                         vertices.Add(faceVertices[voxelTris2[j + i * 6]]);
        //                         uVs.Add(faceUVs[voxelTris2[j + i * 6]]);
        //                         colors.Add(voxelColorAlpha);
        //                         uVs2.Add(voxelSmoothness);

        //                         triangles.Add(counter++);

        //                     }
        //                 }

        //             }

        //             // Debug.Log($"!!! vertices={vertices2.Count}, newVertices={newVertices.Length}, data={data.Length}");

        //             // for (int i = 0; i < vertices2.Length; i++)
        //             // {
        //             //     newVertices[i] = vertices2[i];
        //             // }
        //             // for (int i = 0; i < triangles2.Length; i++)
        //             // {
        //             //     newTriangles[i] = triangles2[i];
        //             // }

        //             // vertices2.Dispose();
        //             // triangles2.Dispose();
        //         }
        // }

    }
    
    [Serializable]
    public struct ContainerData
    {
        public int countVoxels;
        public int countVoxelsDestructible;
        public float levelDestruction;
    }
}