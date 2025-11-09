using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial class SpawnCubeSystem : SystemBase
{
    protected override void OnCreate()
    {
        RequireForUpdate<SpawnCubeConfig>();
    }
    protected override void OnUpdate()
    { }
    protected override void OnStartRunning()
    {
        this.Enabled = false;

        SpawnCubeConfig spawnCubeConfig = SystemAPI.GetSingleton<SpawnCubeConfig>();

        // EntityManager.Instantiate(spawnCubeConfig.cubePrefab, 10000, Allocator.Temp);

        // for (int i = 0; i < 100000; i++)
        // {
        //     Entity spawnedEntity = EntityManager.Instantiate(spawnCubeConfig.cubePrefab);
        //     EntityManager.SetComponentData(spawnedEntity, new LocalTransform
        //     {
        //         Position = new Unity.Mathematics.float3(UnityEngine.Random.Range(0f, 1000f), UnityEngine.Random.Range(20f, 50f), UnityEngine.Random.Range(0, 1000f)),
        //         Rotation = quaternion.identity,
        //         Scale = 1,
        //     });
        // }

        // with buffer.
        EntityCommandBuffer entityCommandBuffer = new EntityCommandBuffer(WorldUpdateAllocator);
        for (int i = 0; i < 10000; i++)
        {
            Entity spawnedEntity = entityCommandBuffer.Instantiate(spawnCubeConfig.cubePrefab);
            entityCommandBuffer.SetComponent(spawnedEntity, new LocalTransform
            {
                Position = new Unity.Mathematics.float3(
                    UnityEngine.Random.Range(30f, 150f),
                    UnityEngine.Random.Range(20f, 150f),
                    UnityEngine.Random.Range(30, 150f)
                ),
                Rotation = quaternion.identity,
                Scale = 1,
            });
        }

        entityCommandBuffer.Playback(EntityManager);
    }

}
