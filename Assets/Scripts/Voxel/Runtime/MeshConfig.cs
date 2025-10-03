using UnityEngine;

namespace Mikalai2006.Voxel
{

    [System.Serializable]
    public struct MeshConfig
    {
        public SOVoxelData sOVoxelData;
        public Material _material;
        public bool existCollider;
        public bool isGreedy;
        public bool isRigidbody;
        public bool isConvex;
        [Tooltip("Рендерит все объекты модели в один меш и использует цвет вершин. Нужно, чтобы материал поддерживал шейдер с Vertex Color")]
        public bool isOneMesh;
        // [Tooltip("Принудительное включение GPU Instances.Включение этой опции принудительно заставит меши использовать GPU Instances, если он активирован в материале")]
        // public bool enableGPUInstances;
        public UnityEngine.Rendering.ShadowCastingMode shadowCastingMode;
    }
}