using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Mikalai2006.Voxel
{
    [RequireComponent(typeof(BoxCollider))]
    public class ContainerBox : Container
    {
        private BoxCollider boxCollider;

        public override void Initialize(MeshConfig config, Vector3 position, VoxelMeshRender vmr)
        {
            base.Initialize(config, position, vmr);

            if (!config.existCollider)
            {
                boxCollider.enabled = false;
            }
        }

        protected override void ConfigureComponents()
        {
            base.ConfigureComponents();
            
            boxCollider = GetComponent<BoxCollider>();
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

        /// <summary>
        /// Определяет находится ли точка внутри box коллайдера.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public override bool PointInCollider(Vector3 point)
        {
            // Get the closest point on the collider to the given point.
            Vector3 closestPoint = boxCollider.ClosestPoint(point);

            // Check if the test point is inside the collider by comparing its distance to the closest point.
            // If the distance is very small, the point is inside.
            return Vector3.Distance(point, closestPoint) < 0.001f;
        }
    }
}