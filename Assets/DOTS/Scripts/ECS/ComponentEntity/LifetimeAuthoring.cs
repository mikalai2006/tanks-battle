using Unity.Entities;
using UnityEngine;

public class LifetimeAuthoring : MonoBehaviour
{
    public float LifetimeRemaining;

    public class Baker : Baker<LifetimeAuthoring>
    {
        public override void Bake(LifetimeAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);

            AddComponent(entity, new LifetimeComponent
            {
                LifetimeRemaining = authoring.LifetimeRemaining
            });
        }
    }
}

    public struct LifetimeComponent : IComponentData
    {
        public float LifetimeRemaining;
    }
