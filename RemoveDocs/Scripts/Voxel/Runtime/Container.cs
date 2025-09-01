using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
    public class Container : MonoBehaviour
    {
        private Dictionary<float3, Voxel> data;
        // private NativeArray<Vector3> vertices;
        // private NativeArray<int> triangles;
        // private NativeArray<Vector3> newVertices;
        // private NativeArray<int> newTriangles;
        private NativeArray<float3> vertices;
        private NativeArray<int> triangles;
        private NativeArray<float2> uvs;

        private Vector3 pointCollision;
        private MeshData meshData = new MeshData();
        // private float sizeVoxel = 1;
        // private RenderParams _rp;
        // [SerializeField] private Material material;

        private MeshRenderer meshRenderer;
        private MeshFilter meshFilter;
        private GPUInstanceEnabler gPUInstanceEnabler;
        private MeshCollider meshCollider;
        private Stack<Vector3> needCreateElements;
        // private Collision collision;
        private GameObject explodeGameObject;
        private SOVoxelData _sOVoxelData;
        [SerializeField] private bool isGreedy = true;
        [SerializeField] private LevelManager _levelManager;

        void OnDestroy()
        {
            // vertices.Dispose();
            // triangles.Dispose();
            // newVertices.Dispose();
            // newTriangles.Dispose();
        }

        // void OnCollisionEnter(Collision collision)
        // {
        //     Debug.Log($"<color=green>Container is collision with trigger {collision.gameObject.name}</color>");
        // }
        // void OnTriggerEnter(Collider collision)
        // {
        //     Debug.Log($"<color=green>Trigger: Container is collision with trigger {collision.gameObject.name}</color>");
        // }

        public void Initialize(MeshConfig config, Vector3 position)
        {

            // vertices = new NativeArray<Vector3>();
            // triangles = new NativeArray<int>();
            // newTriangles = new NativeArray<int>();
            // newVertices = new NativeArray<Vector3>();

            // gameObject.isStatic = true;

            ConfigureComponents();

            if (!config.existCollider)
            {
                meshCollider.enabled = false;
            }
            // else
            // {
            //     meshCollider.convex = config.isConvex;
            // }

            if (config.isRigidbody)
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

            data = new Dictionary<float3, Voxel>();

            meshRenderer.sharedMaterial = config._material;
            // material = config._material;
            // _rp = new RenderParams(config._material);

            needCreateElements = new Stack<Vector3>();

            _levelManager = GameObject.FindGameObjectWithTag("LevelManager")?.GetComponent<LevelManager>();
        }

        public void SetData(SOVoxelData sOVoxelData, int indexGroup, bool _isGreedy = true, float scale = 1)
        {
            isGreedy = _isGreedy;

            _sOVoxelData = sOVoxelData;

            // устанавливаем режим отображения теней.
            meshRenderer.shadowCastingMode = sOVoxelData.shadowCastingMode;

            gPUInstanceEnabler.SetColor(sOVoxelData.groups[indexGroup].color);

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
            Vector3Int[] voxelList = sOVoxelData.groups[indexGroup].voxels.AsParallel().ToArray();
            // Vector3[] voxelList = sOVoxelData.groups.ElementAt(j).voxels.AsParallel().ToArray();
            // Color groupColor = sOVoxelData.groups.ElementAt(j).color;
            for (int i = 0; i < voxelList.Length; i++)
            {
                this[voxelList[i]] = new Voxel() // * scale
                {
                    ID = 1,
                    color = sOVoxelData.colors.ElementAt(i), // groupColor, 
                    type = VoxelType.Grass,
                    // IndexSubMesh = j
                };
            }
            // }
        }

        public void ClearData()
        {
            // vertices.Dispose();
            // triangles.Dispose();
            data.Clear();
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

            foreach (KeyValuePair<float3, Voxel> kvp in data)
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
                // voxelColorAlpha = voxelColor.color;
                // voxelColorAlpha.a = 1;
                // voxelSmoothness = new Vector2(voxelColor.metallic, voxelColor.smoothness);
                // Iterate over each face direction
                for (int i = 0; i < 6; i++)
                {
                    // Проверьте, есть ли сплошной блок напротив этой грани.
                    if (this[blockPos + voxelFaceChecks[i]].isSolid)
                    {
                        continue;
                    }

                    //Draw this face

                    // Соберите соответствующие вершины из вершин по умолчанию и добавьте позицию блока.
                    for (int j = 0; j < 4; j++)
                    {
                        faceVertices[j] = voxelVertices[voxelVertexIndex[j + i * 4]] + blockPos;
                        faceUVs[j] = voxelUVs[j];
                    }

                    for (int j = 0; j < 6; j++)
                    {
                        meshData.vertices.Add(faceVertices[voxelTris[j + i * 6]]);
                        meshData.UVs.Add(faceUVs[voxelTris[j + i * 6]]);
                        meshData.colors.Add(block.color); //voxelColorAlpha);
                        meshData.UVs2.Add(new Vector2(0.4f, 0.75f)); // voxelSmoothness

                        // meshData.triangles[block.IndexSubMesh].Add(counter++);
                        meshData.triangles.Add(counter++);
                    }
                }

            }

            Debug.Log($"Time generate mesh: {(Time.realtimeSinceStartup - startTime) * 1000f} ms. Create {meshData.vertices.Count} vertices, {meshData.triangles.Count} triangles");

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


        public MeshData UploadMesh(bool isDrawMesh)
        {
            meshData.UploadMesh();

            if (isDrawMesh)
            {
                if (meshRenderer == null)
                    ConfigureComponents();

                meshFilter.mesh = meshData.mesh;

                // // simplify mesh.
                // float startTime = Time.realtimeSinceStartup;
                // if (true)
                //     {
                //         var originalMesh = meshFilter.sharedMesh;
                //         float quality = 0.35f;
                //         var meshSimplifier = new UnityMeshSimplifier.MeshSimplifier();
                //         var simpleOptions = SimplificationOptions.Default;
                //         simpleOptions.VertexLinkDistance = 0.1;
                //         meshSimplifier.SimplificationOptions = simpleOptions;
                //         meshSimplifier.Initialize(originalMesh);
                //         meshSimplifier.SimplifyMesh(quality);
                //         var destMesh = meshSimplifier.ToMesh();
                //         meshFilter.sharedMesh = destMesh;
                //     }
                //     Debug.Log($"Time simplify mesh: {(Time.realtimeSinceStartup - startTime) * 1000f} ms");

                // greedy mesh.
                if (isGreedy)
                {
                    float startTime = Time.realtimeSinceStartup;

                    Voxel[] voxArray = new Voxel[_sOVoxelData.Bounds.x * _sOVoxelData.Bounds.y * _sOVoxelData.Bounds.z];
                    var mesh = meshFilter.sharedMesh;
                    var meshArray = Mesh.AllocateWritableMeshData(mesh);
                    var _job = new MeshGreedyJob();
                    _job.mesh = meshArray[0];
                    _job.chunkSize = new int3(_sOVoxelData.Bounds.x, _sOVoxelData.Bounds.y, _sOVoxelData.Bounds.z);
                    _job.blockSize = 1;
                    
                    // Debug.Log($"Time greedy mesh step0: {(Time.realtimeSinceStartup - startTime) * 1000f} ms");
                    Parallel.For(0, data.Count, (g) =>
                    {
                        Vector3Int pos = Vector3Int.FloorToInt(data.ElementAt(g).Key);
                        voxArray[Helpers.To1D(pos.x, pos.y, pos.z, _sOVoxelData.Bounds.x, _sOVoxelData.Bounds.y)] = data.ElementAt(g).Value;
                    });
                    Debug.Log($"Time greedy mesh step1: {(Time.realtimeSinceStartup - startTime) * 1000f} ms");
                    _job.voxels = new NativeArray<Voxel>(voxArray, Allocator.TempJob);
                    _job.Schedule().Complete();
                    
                    Debug.Log($"Time greedy mesh step2: {(Time.realtimeSinceStartup - startTime) * 1000f} ms");
                    Mesh.ApplyAndDisposeWritableMeshData(meshArray, mesh);

                    // FIXME: For some reason setting bounds directly doesn't work so this is needed as a workaround, investigate
                    mesh.RecalculateBounds();
                    meshFilter.mesh = mesh;

                    _job.voxels.Dispose();
                    Debug.Log($"Time greedy mesh step3: {(Time.realtimeSinceStartup - startTime) * 1000f} ms");
                }

                if (meshData.vertices.Count > 3)
                {
                    // meshData.mesh.Optimize();
                    meshCollider.sharedMesh = meshData.mesh;
                }
            }

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

        // void Update()
        // {
        //     Graphics.RenderMesh(_rp, meshFilter.mesh, 0, Matrix4x4.Translate(transform.position));           
        // }

        private void ConfigureComponents()
        {
            meshFilter = GetComponent<MeshFilter>();
            meshRenderer = GetComponent<MeshRenderer>();
            meshCollider = GetComponent<MeshCollider>();
            meshCollider.convex = true;

            gPUInstanceEnabler = gameObject.AddComponent<GPUInstanceEnabler>();
        }

        public Voxel this[Vector3 index]
        {
            get
            {
                if (data.ContainsKey(index))
                    return data[index];
                else
                    return emptyVoxel;
            }

            set
            {
                if (data.ContainsKey(index))
                    data[index] = value;
                else
                    data.Add(index, value);
            }
        }

        public static Voxel emptyVoxel = new Voxel() { ID = 0 };

        #region My functions

        async public UniTask ExposionVoxels(Vector3 _pointCollision, bool isDrawMesh, GameObject _explodeGameObject, float radiusExplode)
        {
            // float startTime = Time.realtimeSinceStartup;

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

            float3[] keys = data.Keys.AsParallel().ToArray();
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
                _radiusExplode = radiusExplode
            };
            JobHandle collisionJobHandle = collisionJob.Schedule(points.Length, 64);
            collisionJobHandle.Complete(); // Or use dependency

            // Debug.Log($"Time JOB create data: {(Time.realtimeSinceStartup - startTime) * 1000f} ms. Count point={points.Count()}. ");

            for (int el = 0; el < collisionJob.needRemoveElements.Length; el++)
            {
                if (!collisionJob.needRemoveElements[el].Equals(float3.zero))
                {
                    data.Remove(collisionJob.needRemoveElements[el]);
                }
            };
            
            for (int el = 0; el < collisionJob._needCreateElements.Length; el++)
            {
                if (!collisionJob._needCreateElements[el].Equals(float3.zero))
                {
                    needCreateElements.Push(collisionJob._needCreateElements[el]);
                }
            };

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

            if (data.Count >= 10 && needCreateElements.Count > 0)
            {
                GenerateMesh();
                // Debug.Log("Time generate mesh: " + (Time.realtimeSinceStartup - temp).ToString("f6"));
                UploadMesh(isDrawMesh);
            }
            
            if (needCreateElements.Count > 0)
            {
                // Debug.Log($"Time upload mesh: {(Time.realtimeSinceStartup - startTime) * 1000f} ms");
                // Debug.Log("Time upload mesh: " + (Time.realtimeSinceStartup - temp).ToString("f6"));
                // Debug.Log($"Exploded {needCreateElements.Count} voxels!");
                // StartCoroutine(createGO());
                await CreateObjectsAsync();
            }
                
            if (data.Count() < 10)
            {
                transform.gameObject.SetActive(false);
            }

            _needCreateElements.Dispose();
            _needRemoveElements.Dispose();
            points.Dispose();
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

        public async UniTask CreateObjectsAsync()
        {
            int count = 10;

            while (needCreateElements.Count > 0 && count > 0)
            {
                Vector3 elem = needCreateElements.Pop();

                float forceMagnitude = 10 * 1000;
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
            }
            
            count--;
            if (count < 0)
            {
                count = 10;
                await UniTask.NextFrame();
            }
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

        #region Mesh Data



        public struct MeshData
        {
            public Mesh mesh;
            public List<Vector3> vertices;
            public int subMeshCount;
            public List<int> triangles;
            // public Dictionary<int, List<int>> triangles;
            public List<Vector2> UVs;
            public List<Vector2> UVs2;
            public List<Color> colors;
            public bool Initialized;

            public void ClearData()
            {
                if (!Initialized)
                {
                    vertices = new List<Vector3>();
                    triangles = new List<int>(); //new Dictionary<int, List<int>>();
                    UVs = new List<Vector2>();
                    UVs2 = new List<Vector2>();
                    colors = new List<Color>();

                    Initialized = true;
                    mesh = new Mesh();
                }
                else
                {
                    vertices.Clear();
                    triangles.Clear();
                    UVs.Clear();
                    UVs2.Clear();
                    colors.Clear();

                    mesh.Clear();
                }
            }
            public void UploadMesh(bool sharedVertices = false)
            {
                mesh.SetVertices(vertices);


                mesh.SetTriangles(triangles, 0, false);

                // subMeshCount = triangles.Count;
                // mesh.subMeshCount = triangles.Count;
                // for (int i = 0; i < triangles.Count; i++)
                // {
                //     mesh.SetTriangles(triangles[i], i, false);
                // }

                mesh.SetColors(colors);

                mesh.SetUVs(0, UVs);
                mesh.SetUVs(2, UVs2);

                mesh.Optimize();

                mesh.RecalculateNormals();

                mesh.RecalculateBounds();

                mesh.UploadMeshData(false);
            }
        }
        #endregion

        #region Static Variables
        static readonly Vector3[] voxelVertices = new Vector3[8]
        {
            new Vector3(0,0,0),//0
            new Vector3(1,0,0),//1
            new Vector3(0,1,0),//2
            new Vector3(1,1,0),//3

            new Vector3(0,0,1),//4
            new Vector3(1,0,1),//5
            new Vector3(0,1,1),//6
            new Vector3(1,1,1),//7
        };

        static readonly Vector3[] voxelFaceChecks = new Vector3[6]
        {
            new Vector3(0,0,-1),//back
            new Vector3(0,0,1),//front
            new Vector3(-1,0,0),//left
            new Vector3(1,0,0),//right
            new Vector3(0,-1,0),//bottom
            new Vector3(0,1,0)//top
        };

        // static readonly int[,] voxelVertexIndex = new int[6, 4]
        // {
        //     {0,1,2,3},
        //     {4,5,6,7},
        //     {4,0,6,2},
        //     {5,1,7,3},
        //     {0,1,4,5},
        //     {2,3,6,7},
        // };
        static readonly int[] voxelVertexIndex = new int[24]
        {
            0,1,2,3,
            4,5,6,7,
            4,0,6,2,
            5,1,7,3,
            0,1,4,5,
            2,3,6,7,
        };

        static readonly Vector2[] voxelUVs = new Vector2[4]
        {
            new Vector2(0,0),
            new Vector2(0,1),
            new Vector2(1,0),
            new Vector2(1,1)
        };

        static readonly int[] voxelTris = new int[36]
        {
            0,2,3,0,3,1,
            0,1,2,1,3,2,
            0,2,3,0,3,1,
            0,1,2,1,3,2,
            0,1,2,1,3,2,
            0,2,3,0,3,1,
        };
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
    
    [BurstCompile]
    struct CheckCollisionJob : IJobParallelFor
    {
        public NativeArray<float3> points;
        public float3 _pointCollision;
        public float _radiusExplode;
        public NativeArray<float3> _needCreateElements;
        public NativeArray<float3> needRemoveElements;

        public void Execute(int index)
        {
            float3 point = points[index];
            if (Helpers.IsInsideSphere(point, _pointCollision, _radiusExplode))
            {
                // list.Add(posx, data.ElementAt(j).Value);
                // // data[posx] = new Voxel()
                // // {
                // //     ID = 0,
                // // };
                // data.Remove(posx);
                needRemoveElements[index] = point;

                if (Helpers.IsInsideSphere(point, _pointCollision, _radiusExplode > 4 ? Math.Max(4, _radiusExplode / 2) : _radiusExplode))
                {
                    _needCreateElements[index] = point;
                }
            }
            else
            {
                _needCreateElements[index] = float3.zero;
                needRemoveElements[index] = float3.zero;
            }
        }
    }
}