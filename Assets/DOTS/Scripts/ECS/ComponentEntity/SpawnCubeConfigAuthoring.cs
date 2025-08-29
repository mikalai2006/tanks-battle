using Unity.Entities;
using UnityEngine;

public class SpawnCubeConfigAuthoring : MonoBehaviour
{
    public GameObject cubePrefab;

    public class Baker : Baker<SpawnCubeConfigAuthoring>
    {
        public override void Bake(SpawnCubeConfigAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);

            AddComponent(entity, new SpawnCubeConfig
            {
                cubePrefab = GetEntity(authoring.cubePrefab, TransformUsageFlags.Dynamic),
            });
        }
    }
}

public struct SpawnCubeConfig : IComponentData
{
    public Entity cubePrefab;
}
