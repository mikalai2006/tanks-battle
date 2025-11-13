using Unity.Entities;
using UnityEngine;

public class StatusAuthoring : MonoBehaviour
{
    public bool active;

    public class Baker : Baker<StatusAuthoring>
    {
        public override void Bake(StatusAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);

            AddComponent(entity, new StatusComponent
            {
                active = authoring.active
            });
        }
    }
}

public struct StatusComponent : IComponentData
{
    public bool active;
}
