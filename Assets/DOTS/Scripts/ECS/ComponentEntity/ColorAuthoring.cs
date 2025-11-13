using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

public class ColorAuthoring : MonoBehaviour
{
    public Color baseColor = Color.white;

    public class Baker : Baker<ColorAuthoring>
    {
        public override void Bake(ColorAuthoring authoring)
        {
            // ColorComponent component = default;
            // // float4 colorValues;
            // // colorValues.x = authoring.color.linear.r;
            // // colorValues.y = authoring.color.linear.g;
            // // colorValues.z = authoring.color.linear.b;
            // // colorValues.w = authoring.color.linear.a;
            // // component.colorMaterial = colorValues;
            // component.colorMaterial = authoring.color;
            
            // var entity = GetEntity(TransformUsageFlags.Renderable);

            // AddComponent(entity, component);
            var entity = GetEntity(authoring, TransformUsageFlags.Renderable);
            // Convert UnityEngine.Color to float4 for ECS
            float4 colorValue = new float4(
                authoring.baseColor.linear.r, 
                authoring.baseColor.linear.g, 
                authoring.baseColor.linear.b, 
                authoring.baseColor.linear.a
            );
            AddComponent(entity, new URPMaterialPropertyBaseColor { Value = colorValue });
        }
    }
}

[MaterialProperty("_Color")] 
public struct URPMaterialPropertyBaseColor : IComponentData
{
    public float4 Value;
}
