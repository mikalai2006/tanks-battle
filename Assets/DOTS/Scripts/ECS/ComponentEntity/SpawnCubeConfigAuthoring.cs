using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class SpawnCubeConfigAuthoring : MonoBehaviour
{
    public GameObject cubePrefab;
    public GameObject cubePrefabWithoutPhysic;

    public class Baker : Baker<SpawnCubeConfigAuthoring>
    {
        public override void Bake(SpawnCubeConfigAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);

            AddComponent(entity, new SpawnCubeConfig
            {
                cubePrefab = GetEntity(authoring.cubePrefab, TransformUsageFlags.Dynamic),
                cubePrefabWithoutPhysic = GetEntity(authoring.cubePrefabWithoutPhysic, TransformUsageFlags.Dynamic),
            });
        }
    }
}

public struct SpawnCubeConfig : IComponentData
{
    public Entity cubePrefab;
    public Entity cubePrefabWithoutPhysic;
}
