using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Mikalai2006.Voxel
{
    [RequireComponent(typeof(MeshCollider))]
    public class ContainerMesh : Container
    {
        private MeshCollider meshCollider;

        public override void Initialize(MeshConfig config, Vector3 position)
        {
            base.Initialize(config, position);


            if (!config.existCollider)
            {
                meshCollider.enabled = false;
            }
            else
            {
                meshCollider.convex = config.isConvex;
                // meshCollider.providesContacts = true;
            }
        }

        protected override void ConfigureComponents()
        {
            base.ConfigureComponents();
            
            meshCollider = GetComponent<MeshCollider>();
        }

        public override async UniTask<Mesh> UploadMeshGreedy(bool isDrawMesh)
        {
            Mesh mesh = await base.UploadMeshGreedy(isDrawMesh);
            
            if (meshFilter.sharedMesh.vertices.Length > 3)
            {
                // meshData.mesh.Optimize();
                meshCollider.sharedMesh = meshFilter.sharedMesh; //meshData.mesh;
            }
            return mesh;
        }

        public override MeshData UploadMesh(bool isDrawMesh)
        {
            base.UploadMesh(isDrawMesh);

            if (meshData.vertices.Count > 3)
            {
                // meshData.mesh.Optimize();
                meshCollider.sharedMesh = meshData.mesh;
            }

            return meshData;
        }

    }
}