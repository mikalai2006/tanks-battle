using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Rendering;

[BurstCompile]
[RequireMatchingQueriesForUpdate]
public partial class LifetimeSystem : SystemBase
{
    EntityQuery query;

    // public void OnCreate(ref SystemState state)
    // {
    //     state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
    // }

    [BurstCompile]
    protected override void OnCreate()
    {
        query = GetEntityQuery(
            ComponentType.ReadWrite<EnableComponent>(),
            ComponentType.ReadWrite<LifetimeComponent>(),
            ComponentType.ReadWrite<VelocityComponent>(),
            ComponentType.ReadWrite<PhysicsVelocity>(),
            ComponentType.ReadOnly<PhysicsMass>()
        );
    }
    
    [BurstCompile]
    protected override void OnUpdate()
    {
        // // EntityQuery query = EntityManager.CreateEntityQuery(typeof(LifetimeComponent));

        // // // // Get all entities matching the query
        // // NativeArray<Entity> allEntities = query.ToEntityArray(Allocator.Temp);
        var ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(EntityManager.WorldUnmanaged).AsParallelWriter();

        // foreach (var (e, entity) in SystemAPI.Query<EnableComponent>()
        //     // .WithAll<LifetimeComponent>()
        //     .WithEntityAccess())
        // {
        //     if (EntityManager.HasComponent<LifetimeComponent>(entity))
        //     {

        //         // foreach (var lifetimeComponent in SystemAPI.Query<LifetimeComponent>()) {

        //         // float deltaTime = SystemAPI.Time.DeltaTime;
        //         // var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();

        //     }
        // }
        
        new DestroyExpiredEntitiesJob
        {
            DeltaTime = SystemAPI.Time.DeltaTime,
            ECB = ecb
        }.ScheduleParallel(query);
    }

    [BurstCompile]
    public partial struct DestroyExpiredEntitiesJob : IJobEntity
    {
        public float DeltaTime;
        public EntityCommandBuffer.ParallelWriter ECB;

        public void Execute(
            Entity entity,
            ref LifetimeComponent lifetimeComponent,
            ref VelocityComponent velocityComponent,
            ref PhysicsVelocity physicVelocity,
            in PhysicsMass physicsMass,
            [EntityIndexInQuery] int entityInQueryIndex
        )
        {
            lifetimeComponent.LifetimeRemaining -= DeltaTime;
            if (lifetimeComponent.LifetimeRemaining <= 0f)
            {
                // ECB.SetEnabled(entityInQueryIndex, entity, false);
                // ECB.DestroyEntity(entityInQueryIndex, entity);

                ECB.SetComponentEnabled<EnableComponent>(entityInQueryIndex, entity, false);

                ECB.SetComponentEnabled<MaterialMeshInfo>(entityInQueryIndex, entity, false);

                ECB.SetSharedComponent(entityInQueryIndex, entity, new PhysicsWorldIndex(1));
            }
            if (velocityComponent.forceAmount > 0)
            {
                var forceVector = (float3)velocityComponent.direction * velocityComponent.forceAmount * DeltaTime;
                physicVelocity.Linear = forceVector;
                var forceVectorAngular = (float3)velocityComponent.direction * (velocityComponent.forceAmount / 1000) * DeltaTime;
                physicVelocity.Angular = forceVectorAngular;
                // physicsVelocity.ValueRW.Linear += new float3(
                //     velocityComponent.ValueRO.direction.x * speed,
                //     velocityComponent.ValueRO.direction.y * speed,
                //     velocityComponent.ValueRO.direction.z * speed
                // );
                velocityComponent.forceAmount -= 200; // Mathf.Max(0, velocityComponent.ValueRW.forceAmount - 100f);
                // physicVelocity.ValueRW.Linear = new float3(0, -1, 0);
            }

        }
    }
}
