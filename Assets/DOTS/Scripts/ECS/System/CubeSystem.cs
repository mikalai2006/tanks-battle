// using Unity.Collections;
// using Unity.Entities;
// using Unity.Physics;
// using Unity.Mathematics;
// using UnityEngine;

// public partial struct CubeSystem : ISystem
// {
//     InputComponent inputComponent;

//     private void OnUpdate(ref SystemState state)
//     {
//         EntityManager entityManager = state.EntityManager;

//         if (!SystemAPI.TryGetSingleton(out inputComponent))
//         {
//             return;
//         }

//         NativeArray<Entity> entities = entityManager.GetAllEntities(Allocator.Temp);

//         foreach (Entity entity in entities)
//         // foreach (Entity entity in SystemAPI.Query<RefRW<PhysicsVelocity>>)
//         {
//             if (entityManager.HasComponent<CubeComponent>(entity))
//             {
//                 CubeComponent cubeComponent = entityManager.GetComponentData<CubeComponent>(entity);

//                 RefRW<PhysicsVelocity> physicsVelocity = SystemAPI.GetComponentRW<PhysicsVelocity>(entity);

//                 var speed = cubeComponent.moveSpeed * SystemAPI.Time.DeltaTime;

//                 physicsVelocity.ValueRW.Linear += new float3(
//                     inputComponent.movemement.x * speed,
//                     0,
//                     inputComponent.movemement.y * speed
//                 );
//             }
//         }

//         entities.Dispose();
//     }
// }
