// using Unity.Entities;
// using Unity.Mathematics;
// using Unity.Transforms;

// public partial class SpawnCubeSystem : SystemBase
// {
//     InputComponent inputComponent;
//     protected override void OnCreate()
//     {
//         RequireForUpdate<SpawnCubeConfig>();
//     }

//     protected override void OnUpdate()
//     {
//         if (!SystemAPI.TryGetSingleton(out inputComponent))
//         {
//             return;
//         }

//         if (inputComponent.pressingLMB > 0.5f)
//         {
//             OnStartRunning();
//         }
//     }

//     protected override void OnStartRunning()
//     {
//         // this.Enabled = false;

//         SpawnCubeConfig spawnCubeConfig = SystemAPI.GetSingleton<SpawnCubeConfig>();

//         // EntityManager.Instantiate(spawnCubeConfig.cubePrefab, 10000, Allocator.Temp);

//         // for (int i = 0; i < 100000; i++)
//         // {
//         //     Entity spawnedEntity = EntityManager.Instantiate(spawnCubeConfig.cubePrefab);
//         //     EntityManager.SetComponentData(spawnedEntity, new LocalTransform
//         //     {
//         //         Position = new Unity.Mathematics.float3(UnityEngine.Random.Range(0f, 1000f), UnityEngine.Random.Range(20f, 50f), UnityEngine.Random.Range(0, 1000f)),
//         //         Rotation = quaternion.identity,
//         //         Scale = 1,
//         //     });
//         // }

//         // if (inputComponent.pressingLMB > 0.5f) {
//         //     // with buffer.
//         //     EntityCommandBuffer ecb = new EntityCommandBuffer(WorldUpdateAllocator);
//         //     for (int i = 0; i < spawnCubeConfig.countEntity; i++)
//         //     {
//         //         Entity spawnedEntity = ecb.Instantiate(spawnCubeConfig.cubePrefab);
//         //         ecb.AddComponent(spawnedEntity, new LocalTransform
//         //         {
//         //             Position = new Unity.Mathematics.float3(
//         //                 UnityEngine.Random.Range(30f, 150f),
//         //                 UnityEngine.Random.Range(20f, 150f),
//         //                 UnityEngine.Random.Range(30, 150f)
//         //             ),
//         //             Rotation = quaternion.identity,
//         //             Scale = 1,
//         //         });

//         //         ecb.AddComponent(spawnedEntity, new URPMaterialPropertyBaseColor{ Value = new float4(0,1,0,1), });
                
//         //         // EntityManager.SetComponentData(spawnedEntity, new ColorComponent
//         //         // {
//         //         //     colorMaterial = new float4(0,1,0,1),
//         //         // });
//         //     }

//         //     ecb.Playback(EntityManager);
//         //     ecb.Dispose();
        
//         // }
//     }

// }
