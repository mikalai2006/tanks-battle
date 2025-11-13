using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class VelocityAuthoring : MonoBehaviour
{
    public float3 direction;
    public float forceAmount;

    public class Baker : Baker<VelocityAuthoring>
    {
        public override void Bake(VelocityAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);

            AddComponent(entity, new VelocityComponent
            {
                direction = authoring.direction,
                forceAmount = authoring.forceAmount
            });
        }
    }
}

public struct VelocityComponent : IComponentData
{
    public float3 direction;
    public float forceAmount;
}
