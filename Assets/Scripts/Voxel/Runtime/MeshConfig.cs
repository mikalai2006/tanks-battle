using UnityEngine;

namespace Mikalai2006.Voxel
{
    [System.Serializable]
    public struct MeshConfig
    {
        public SOVoxelData sOVoxelData;
        public Material _material;
        public bool existCollider;
        // public TypeCollider typeCollider;
        public bool isGreedy;
        public bool isRigidbody;
        public bool isConvex;
        [Tooltip("Рендерит все объекты модели в один меш и использует цвет вершин. Нужно, чтобы материал поддерживал шейдер с Vertex Color")]
        public bool isOneMesh;
        // [Tooltip("Принудительное включение GPU Instances.Включение этой опции принудительно заставит меши использовать GPU Instances, если он активирован в материале")]
        // public bool enableGPUInstances;
        public float emissionValue;

        public bool isTile;
        [Tooltip("Указывает разрушаемый ли объект")]
        public bool isDestructible;
        [Tooltip("Если это тайл, то использовать пивот, как половина максимального измерения по X,Z. Например: 32x32x20 - пивот -> 15.5x0.5x15.5")]
        public UnityEngine.Rendering.ShadowCastingMode shadowCastingMode;

        [Header("Масштаб объекта")]
        [Tooltip("Устанавливать ли глобальный масштаб к Wrapper. Это нужно делать на объектах, которые не имеют много вложенных VoxelMeshRender.")]
        public bool useGlobalScale;
        [Tooltip("Если он больше 0, то устанавливается для объекта")]
        [Range(0,1)] public float customScale;
        [Tooltip("Список новых цветов (если заданы цвета здесь, они перезапишут цвета из sOVoxelData используя соответствующие индексы) (В идеале число цветов здесь и в sOVoxelData должны совпадать)")]
        public Color[] color;
    }

    // public enum TypeCollider
    // {
    //     MeshCollider,
    //     BoxCollider,
    //     SphereCollider
    // }
}
