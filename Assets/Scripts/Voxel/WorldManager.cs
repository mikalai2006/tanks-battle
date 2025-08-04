using System.Collections.Generic;
using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using static Mikalai2006.Voxel.Container;

namespace Mikalai2006.Voxel
{
    public class WorldManager : MonoBehaviour
    {
        public Material worldMaterial;
        public VoxelColor[] WorldColors;
        private Container container;
        public Container Container => container;
        public SOVoxelData sOVoxelData;

        public MeshData[] meshDatas = new MeshData[1];

        void Start()
        {
            if (_instance != null)
            {
                if (_instance != this)
                    Destroy(this);
            }
            else
            {
                _instance = this;
            }

            // Vector3[] voxelList = new Vector3[16*16*16];
            // for (int x = 0; x < 16; x++)
            // {
            //     for (int z = 0; z < 16; z++)
            //     {
            //         int randomYHeight = Random.Range(8, 16);
            //         for (int y = 0; y < randomYHeight; y++)
            //         {
            //             voxelList[x + (y * 16) + (z * 16 * 16)] = new Vector3(x, y, z);
            //         }
            //     }
            // }
        }

        private void CreateOnlyEdgeVoxels()
        {
            Dictionary<Vector3, bool> dictionaryVoxels = sOVoxelData.voxels.ToDictionary(t => t, s => true);
            List<Vector3> visibleVoxels = new List<Vector3>();

            for (int i = 0; i < sOVoxelData.voxels.Count; i++)
            {
                int countNeighbours = GetVoxelNeighbours(sOVoxelData.voxels[i], dictionaryVoxels);
                if (countNeighbours < 24)
                {
                    visibleVoxels.Add(sOVoxelData.voxels[i]);
                }
            }

            Debug.Log($"Count voxels = {visibleVoxels.Count} (all count = {sOVoxelData.voxels.Count})");

            CreateContainer(sOVoxelData, 1);
        }

        private int GetVoxelNeighbours(Vector3 pos, Dictionary<Vector3, bool> allVoxels)
        {
            int i = 0;

            Vector3[] coordsNeighbours = new Vector3[]{
                new Vector3(-1,-1,-1),
                new Vector3(-1,-1,0),
                new Vector3(-1,-1,1),
                new Vector3(-1,0,0),
                new Vector3(-1,0,1),
                new Vector3(-1,1,-1),
                new Vector3(-1,1,0),
                new Vector3(-1,1,1),
                new Vector3(0,-1,-1),
                new Vector3(0,-1,0),
                new Vector3(0,-1,1),
                new Vector3(0,-1,0),
                new Vector3(0,-1,1),
                new Vector3(0,0,-1),
                new Vector3(0,0,0),
                new Vector3(0,0,1),
                new Vector3(0,1,-1),
                new Vector3(0,1,0),
                new Vector3(0,1,1),
                new Vector3(1,-1,-1),
                new Vector3(1,-1,0),
                new Vector3(1,-1,1),
                new Vector3(1,0,-1),
                new Vector3(1,0,0),
                new Vector3(1,0,1),
                new Vector3(1,1,-1),
                new Vector3(1,1,0),
                new Vector3(1,1,1),
            };

            for (int x = 0; x < coordsNeighbours.Length; x++)
            {
                Vector3 nPos = pos + coordsNeighbours[x];
                bool value = false;
                if (allVoxels.TryGetValue(nPos, out value))
                {
                    i++;
                }
            }

            return i;
        }

        public MeshData[] CreateContainer(SOVoxelData sOVoxelData, float scale = 1, bool isDrawMesh = true)
        {
            GameObject cont = new GameObject("Container");
            cont.transform.parent = transform;
            container = cont.AddComponent<Container>();
            container.Initialize(worldMaterial, Vector3.zero, true);
            container.SetSizeVoxel(sOVoxelData.sizeVoxel);
            // container.GetComponent<Collider>().isTrigger = true;

            // var segment = new ArraySegment<Vector3>(voxelList, 1, 10);
            // container.SetData(segment.ToArray(), scale);

            container.SetData(sOVoxelData, 0, scale);


            container.GenerateMesh();
            meshDatas[0] = container.UploadMesh(isDrawMesh);

            return meshDatas;
        }

        public void CreateInstanceCubes()
        {

        }

        // void Update()
        // {
        //     RenderParams rp = new RenderParams(worldMaterial);
        //     for (int i = 0; i < meshDatas.Length; i++)
        //     {
        //         Graphics.RenderMesh(rp, meshDatas[i].mesh, 0, Matrix4x4.Translate(new Vector3(0f, 0.5f, 0f)));
        //     }
        // }

        private static WorldManager _instance;

        public static WorldManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = FindFirstObjectByType<WorldManager>();
                return _instance;
            }
        }
    }
}