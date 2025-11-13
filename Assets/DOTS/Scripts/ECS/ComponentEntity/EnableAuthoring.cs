using Unity.Entities;
using UnityEngine;

public class EnableAuthoring : MonoBehaviour
{
    public class Baker : Baker<EnableAuthoring>
    {
        public override void Bake(EnableAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);

            AddComponent(entity, new EnableComponent());

            SetComponentEnabled<EnableComponent>(entity, false);
        }
    }
}

public struct EnableComponent : IComponentData, IEnableableComponent
{
}
