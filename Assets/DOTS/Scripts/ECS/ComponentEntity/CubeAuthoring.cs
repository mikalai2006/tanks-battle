using Unity.Entities;
using UnityEngine;

public class CubeAuthoring : MonoBehaviour
{
    public float moveSpeed = 10;

    public class Baker : Baker<CubeAuthoring>
    {
        public override void Bake(CubeAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);

            AddComponent(entity, new CubeComponent
            {
                moveSpeed = authoring.moveSpeed,
            });
        }
    }
}

public struct CubeComponent: IComponentData
{
    public float moveSpeed;
}
