// using Unity.Burst;
// using Unity.Entities;
// using Unity.Mathematics;
// using Unity.Physics;
// using Unity.Physics.Extensions;
// using Unity.Rendering;

// [RequireMatchingQueriesForUpdate]
// public partial class VelocitySystem : SystemBase
// {
//     EntityQuery query;

//     [BurstCompile]
//     protected override void OnCreate()
//     {
//         // Query that contains all of Execute params found in `QueryJob` - as well as additional user specified component `BoidTarget`.
//         query = GetEntityQuery(
//             ComponentType.ReadWrite<EnableComponent>(),
//             ComponentType.ReadWrite<VelocityComponent>(),
//             ComponentType.ReadWrite<VelocityComponent>(),
            

//         );

//         // // Query that contains all of Execute params found in `QueryJob` - as well as additional user specified component `BoidObstacle`.
//         // query_boidobstacle = GetEntityQuery(ComponentType.ReadWrite<SampleComponent>(),ComponentType.ReadOnly<BoidObstacle>());
//     }

//     [BurstCompile]
//     protected override void OnUpdate()
//     {
//         //  var ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(EntityManager.WorldUnmanaged).AsParallelWriter();


//         foreach (var (e, velocityComponent, physicMass, physicVelocity, entity) in SystemAPI.Query<EnableComponent, RefRW<VelocityComponent>, PhysicsMass, RefRW<PhysicsVelocity>>()
//             // .WithAll<EnableComponent>()
//             .WithEntityAccess())
//         {
//             if (EntityManager.HasComponent<VelocityComponent>(entity))
//             {
//                 // VelocityComponent velocityComponent = EntityManager.GetComponentData<VelocityComponent>(entity);

//                 // RefRW<PhysicsVelocity> physicsVelocity = SystemAPI.GetComponentRW<PhysicsVelocity>(entity);
//                 if (velocityComponent.ValueRW.forceAmount > 0)
//                 {
//                     var forceVector = (float3)velocityComponent.ValueRO.direction * velocityComponent.ValueRO.forceAmount * SystemAPI.Time.DeltaTime;
//                     physicVelocity.ValueRW.ApplyLinearImpulse(physicMass, forceVector);
//                     var forceVectorAngular = (float3)velocityComponent.ValueRO.direction * (velocityComponent.ValueRO.forceAmount / 100) * SystemAPI.Time.DeltaTime;
//                     physicVelocity.ValueRW.ApplyAngularImpulse(physicMass, forceVectorAngular);
//                     // physicsVelocity.ValueRW.Linear += new float3(
//                     //     velocityComponent.ValueRO.direction.x * speed,
//                     //     velocityComponent.ValueRO.direction.y * speed,
//                     //     velocityComponent.ValueRO.direction.z * speed
//                     // );
//                     velocityComponent.ValueRW.forceAmount -= 1000; // Mathf.Max(0, velocityComponent.ValueRW.forceAmount - 100f);
//                     // physicVelocity.ValueRW.Linear = new float3(0, -1, 0);
//                 }

//                 // velocityComponent.ValueRW.direction = new float3(
//                 //     Mathf.Max(0, velocityComponent.ValueRO.direction.x - 0.01f),
//                 //     Mathf.Max(1f, velocityComponent.ValueRO.direction.y - 0.01f),
//                 //     Mathf.Max(0, velocityComponent.ValueRO.direction.z - 0.01f)
//                 // );

//                 // new ApplyVelocityJob
//                 // {
//                 //     DeltaTime = SystemAPI.Time.DeltaTime,
//                 //     ECB = ecb
//                 // }.ScheduleParallel();
//             }
//         }

//     }
    
//     [BurstCompile]
//     public partial struct ApplyVelocityJob : IJobEntity
//     {
//         public float DeltaTime;
//         public EntityCommandBuffer.ParallelWriter ECB;

//         public void Execute(Entity entity, ref LifetimeComponent lifetime, [EntityIndexInQuery] int entityInQueryIndex)
//         {
//             lifetime.LifetimeRemaining -= DeltaTime;
//             if (lifetime.LifetimeRemaining <= 0f)
//             {
//                 // ECB.SetEnabled(entityInQueryIndex, entity, false);
//                 // ECB.DestroyEntity(entityInQueryIndex, entity);
                
//                 ECB.SetComponentEnabled<EnableComponent>(entityInQueryIndex, entity, false);

//                 ECB.SetComponentEnabled<MaterialMeshInfo>(entityInQueryIndex, entity, false);
                
//                 ECB.SetSharedComponent(entityInQueryIndex, entity, new PhysicsWorldIndex(1));
//             }
//         }
//     }
// }

