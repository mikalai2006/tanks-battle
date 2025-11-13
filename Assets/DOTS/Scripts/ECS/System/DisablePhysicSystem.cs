// using Unity.Burst;
// using Unity.Collections;
// using Unity.Entities;
// using Unity.Mathematics;
// using Unity.Physics;
// using Unity.Physics.Extensions;
// using Unity.Rendering;


// [BurstCompile]
// public partial class DisablePhysicSystem : SystemBase
// {
    
//     [BurstCompile]
//     protected override void OnUpdate()
//     {

//         EntityQuery query = EntityManager.CreateEntityQuery(typeof(MaterialMeshInfo));

//         // // Get all entities matching the query
//         NativeArray<Entity> allEntities = query.ToEntityArray(Allocator.Temp);

//         EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

//         for (int i =0; i < allEntities.Length; i++)
//         {
//             ecb.SetSharedComponent(allEntities[i], new PhysicsWorldIndex(1));
//         }

//         // foreach (var (physicsWorldIndex, entity) in SystemAPI.Query<PhysicsWorldIndex>()
//         //     // .WithAll<EnableComponent>()
//         //     .WithAll<MaterialMeshInfo>()
//         //     .WithEntityAccess())
//         // {
//         //     // if (EntityManager.HasComponent<PhysicsWorldIndex>(entity))
//         //     // {
//         //     //     RefRW<PhysicsWorldIndex> physicsWorldIndex = SystemAPI.GetComponentRW<PhysicsWorldIndex>(entity);
//         //     //     physicsWorldIndex.Value = 1;
//         //     //     // VelocityComponent velocityComponent = EntityManager.GetComponentData<VelocityComponent>(entity);

//         //     //     // RefRW<PhysicsVelocity> physicsVelocity = SystemAPI.GetComponentRW<PhysicsVelocity>(entity);
//         //     //     if(velocityComponent.ValueRW.forceAmount > 0)
//         //     //     {
//         //     //         var forceVector = (float3)velocityComponent.ValueRO.direction * velocityComponent.ValueRO.forceAmount * SystemAPI.Time.DeltaTime;
//         //     //         physicVelocity.ValueRW.ApplyLinearImpulse(physicMass, forceVector);
//         //     //         var forceVectorAngular = (float3)velocityComponent.ValueRO.direction * (velocityComponent.ValueRO.forceAmount / 100) * SystemAPI.Time.DeltaTime;
//         //     //         physicVelocity.ValueRW.ApplyAngularImpulse(physicMass, forceVectorAngular);
//         //     //         // physicsVelocity.ValueRW.Linear += new float3(
//         //     //         //     velocityComponent.ValueRO.direction.x * speed,
//         //     //         //     velocityComponent.ValueRO.direction.y * speed,
//         //     //         //     velocityComponent.ValueRO.direction.z * speed
//         //     //         // );
//         //     //         velocityComponent.ValueRW.forceAmount -= 1000; // Mathf.Max(0, velocityComponent.ValueRW.forceAmount - 100f);
//         //     //         // physicVelocity.ValueRW.Linear = new float3(0, -1, 0);
//         //     //     }

//         //     //     // velocityComponent.ValueRW.direction = new float3(
//         //     //     //     Mathf.Max(0, velocityComponent.ValueRO.direction.x - 0.01f),
//         //     //     //     Mathf.Max(1f, velocityComponent.ValueRO.direction.y - 0.01f),
//         //     //     //     Mathf.Max(0, velocityComponent.ValueRO.direction.z - 0.01f)
//         //     //     // );
//         //     // }

            
//         //     // Check if the entity is not already in the desired world (e.g., world index 1)
            
//         //     if (physicsWorldIndex.Value != 1)
//         //     {
//         //         // Create a new PhysicsWorldIndex with the desired world index
//         //         var newWorldIndex = new PhysicsWorldIndex(1);

//         //         // Set the new PhysicsWorldIndex shared component on the entity
//         //         EntityManager.SetSharedComponent(entity, newWorldIndex);

//         //         // Optional: Remove MyTagComponent if it was used as a one-time trigger
//         //         // EntityManager.RemoveComponent<MyTagComponent>(entity);
//         //     }
//         // }
//             ecb.Playback(EntityManager);
//             ecb.Dispose();
//     }
// }
