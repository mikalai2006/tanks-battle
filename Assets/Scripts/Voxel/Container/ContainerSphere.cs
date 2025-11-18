using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Mikalai2006.Voxel
{
    [RequireComponent(typeof(SphereCollider))]
    public class ContainerSphere : Container
    {
        private SphereCollider sphereCollider;

        public override void Initialize(MeshConfig config, Vector3 position)
        {
            base.Initialize(config, position);

            if (!config.existCollider)
            {
                sphereCollider.enabled = false;
            }
        }

        protected override void ConfigureComponents()
        {
            base.ConfigureComponents();
            
            sphereCollider = GetComponent<SphereCollider>();
        }

        public override async UniTask<Mesh> UploadMeshGreedy(bool isDrawMesh)
        {
            Mesh mesh = await base.UploadMeshGreedy(isDrawMesh);

            return mesh;
        }

        public override MeshData UploadMesh(bool isDrawMesh)
        {
            base.UploadMesh(isDrawMesh);

            return meshData;
        }

    }
}