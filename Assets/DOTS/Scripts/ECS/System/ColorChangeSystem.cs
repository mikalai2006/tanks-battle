// using Unity.Entities;
// using Unity.Mathematics;

// public partial class ColorChangeSystem : SystemBase
// {
//     protected override void OnUpdate()
//     {
//         foreach (var (colorComponent, entity) in SystemAPI.Query<RefRW<URPMaterialPropertyBaseColor>>().WithEntityAccess())
//         {
//             colorComponent.ValueRW.Value = new float4(0, 0, 1, 1); // Blue color (R, G, B, A)
//         }
//     }
// }
