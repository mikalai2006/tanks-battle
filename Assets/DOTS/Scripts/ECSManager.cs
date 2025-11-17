
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Rendering;

public class ECSManager : MonoBehaviour
{
    private EntityManager _entityManager;
    // public GameObject cubePrefab;
    // [SerializeField] private Button button;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Mesh Mesh;
    public UnityEngine.Material Material;
    public int EntityCountGenerate;
    public int EntityCountUpdate;
    public Vector2 forceAmount;
    public Vector2 lifeTimeRange;
    [SerializeField] private int countCreatePerFrame;
    BlobAssetReference<Unity.Physics.Collider> _collider;

    void Start()
    {
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;


        OnTestRenderMeshUtility();
    }

    public void CreateDots()
    {
        List<ECSDataSpawn> listData = new List<ECSDataSpawn>();

        for (int i = 0; i < EntityCountGenerate; i++)
        {
            var Position = new float3(
                0,
                0,
                0
            );

            var Color = new Color(
                UnityEngine.Random.Range(0f, 1f),
                UnityEngine.Random.Range(0f, 1f),
                UnityEngine.Random.Range(0f, 1f),
                UnityEngine.Random.Range(0f, 1f)
            );
            var direction = new float3(
                UnityEngine.Random.insideUnitSphere.x,
                UnityEngine.Random.insideUnitSphere.y,
                UnityEngine.Random.insideUnitSphere.z
            );
            var _forceAmount = UnityEngine.Random.Range(forceAmount.x, forceAmount.y);
            var LifetimeRemaining = UnityEngine.Random.Range(lifeTimeRange.x, lifeTimeRange.y);

            listData.Add(new ECSDataSpawn
            {
                color = Color,
                direction = direction,
                forceAmount = _forceAmount,
                position = Position,
                lifetimeRemaining = LifetimeRemaining,
                scale = 1
            });
        }

        GenerateDots(listData);
    }

    public void TestUpdateDots()
    {

        List<ECSDataSpawn> listData = new List<ECSDataSpawn>();

        for (int i = 0; i < EntityCountUpdate; i++)
        {
            var Position = new float3(
                // UnityEngine.Random.insideUnitSphere.x * 5 + 30,
                // UnityEngine.Random.insideUnitSphere.y * 5 + 30,
                // UnityEngine.Random.insideUnitSphere.z * 5 + 30

                UnityEngine.Random.Range(30f, 150f),
                UnityEngine.Random.Range(20f, 150f),
                UnityEngine.Random.Range(30, 150f)
            );

            var Color = new Color(
                UnityEngine.Random.Range(0f, 1f),
                UnityEngine.Random.Range(0f, 1f),
                UnityEngine.Random.Range(0f, 1f),
                UnityEngine.Random.Range(0f, 1f)
            );
            var direction = new float3(
                UnityEngine.Random.insideUnitSphere.x,
                UnityEngine.Random.insideUnitSphere.y,
                UnityEngine.Random.insideUnitSphere.z
            );
            var _forceAmount = UnityEngine.Random.Range(forceAmount.x, forceAmount.y);
            var LifetimeRemaining = UnityEngine.Random.Range(lifeTimeRange.x, lifeTimeRange.y);

            listData.Add(new ECSDataSpawn
            {
                color = Color,
                direction = direction,
                forceAmount = _forceAmount,
                position = Position,
                lifetimeRemaining = LifetimeRemaining,
                scale = 1
            });
        }

        UpdateDataDots(listData).Forget();
    }
    
    // public async UniTask CreateObjectsAsync()
    //     {
    //         int count = GameManager.Instance.Settings.countCreateVoxelByFrame;

    //         while (needUpdateEntities.Count > 0)
    //         {
    //             ECSDataSpawn elem = needUpdateEntities.Pop();

    //             float forceMagnitude = 10 * 30;
    //             // GameObject gObj = Instantiate(GameManager.Instance.Settings.prefabVoxel, Machine.levelManager.objectSpawnEffect.transform);
    //             GameObject gObj = Lean.Pool.LeanPool.Spawn(GameManager.Instance.Settings.prefabVoxel, _levelManager.objectSpawnEffect.transform);
    //             Vector3 pointSpawnVoxel = transform.TransformPoint(elem.position);
    //             gObj.transform.SetPositionAndRotation(pointSpawnVoxel, Quaternion.identity);
    //             var voxPrefab = gObj.GetComponent<VoxelPrefab>();
    //             voxPrefab.Init(meshConfig.sOVoxelData);
    //             voxPrefab.SetColor(elem.color);
    //             // gObj.isStatic = true;
    //             // gObj.transform.SetLocalPositionAndRotation(listVoxels.ElementAt(k).Key, Quaternion.identity);
    //             // gObj.gameObject.AddComponent<BoxCollider>();


    //             // var mat = gObj.gameObject.GetComponent<MeshRenderer>().material;
    //             // var mesh = gObj.gameObject.GetComponent<MeshFilter>().mesh;
    //             // RenderParams _rp = new RenderParams(WorldManager.Instance.worldMaterial);
    //             // Graphics.RenderMesh(_rp, mesh, 0, Matrix4x4.Translate(pointSpawnVoxel));

    //             var r = gObj.gameObject.GetComponent<Rigidbody>();
    //             if (r == null)
    //             {
    //                 r = gObj.gameObject.AddComponent<Rigidbody>();
    //             }
    //             r.collisionDetectionMode = CollisionDetectionMode.Continuous;
    //             r.mass = 100f;
    //             r.useGravity = true;
    //             var forceDirection = UnityEngine.Random.onUnitSphere; //Vector3.Scale(UnityEngine.Random.onUnitSphere, transform.forward);
    //             r.AddForce(forceDirection * forceMagnitude, ForceMode.Impulse);
    //             // gameObjects[count - 1] = gObj;
    //             // gObj.isStatic = false;
    //             // Destroy(gObj, 15);
    //             Lean.Pool.LeanPool.Despawn(gObj, UnityEngine.Random.Range(1, 3));


    //             // // simulate paraboloid.
    //             // var forceDirection = UnityEngine.Random.onUnitSphere;
    //             // float time = UnityEngine.Random.Range(1, 5);
    //             // gObj.Init(forceDirection * 10, UnityEngine.Random.onUnitSphere, time * 0.5f);
    //             // Lean.Pool.LeanPool.Despawn(gObj, time);

    //             count--;

    //             if (count < 0)
    //             {
    //                 count = GameManager.Instance.Settings.countCreateVoxelByFrame;
    //                 await UniTask.NextFrame();
    //             }
    //         }

    //     }

    async public UniTask UpdateDataDots(List<ECSDataSpawn> listData)
    {
        EntityQuery query = _entityManager.CreateEntityQuery(new EntityQueryDesc
        {
            Disabled = new ComponentType[]
            {
                ComponentType.ReadWrite<EnableComponent>(),
            },
            // Optionally, you can also specify Any and None here
            // Any = new ComponentType[] { typeof(MyOptionalComponent) },
            // None = new ComponentType[] { typeof(MyExcludedComponent) }
        });

        // // Get all entities matching the query
        NativeArray<Entity> allEntities = query.ToEntityArray(Allocator.Persistent);

        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Persistent);

        var maxCountEntities = Mathf.Min(allEntities.Length, listData.Count);

        // foreach (Entity entity in allEntities)
        for (int i = 0; i < maxCountEntities; i++)
        {
            var elemData = listData[i]; //needUpdateEntities.Pop();

            if (_entityManager.HasComponent<MaterialMeshInfo>(allEntities[i]))
            {
                // LocalTransform localTransform = _entityManager.GetComponentData<LocalTransform>(allEntities[i]);
                _entityManager.AddComponentData(allEntities[i], new LocalTransform
                {
                    Position = elemData.position,
                    Rotation = quaternion.identity,
                    Scale = elemData.scale,
                });

                _entityManager.AddComponentData(allEntities[i], new LocalTransform
                {
                    Position = elemData.position,
                    Rotation = quaternion.identity,
                    Scale = elemData.scale,
                });

                _entityManager.AddComponentData(allEntities[i], new URPMaterialPropertyBaseColor
                {
                    Value =
                    new float4(
                        elemData.color.r,
                        elemData.color.g,
                        elemData.color.b,
                        elemData.color.a
                    ),
                });
                _entityManager.AddComponentData(allEntities[i], new VelocityComponent
                {
                    direction = new Unity.Mathematics.float3(
                        elemData.direction.x,
                        elemData.direction.y,
                        elemData.direction.z
                    ),
                    forceAmount = elemData.forceAmount,
                });
                _entityManager.AddComponentData(allEntities[i], new LifetimeComponent
                {
                    LifetimeRemaining = elemData.lifetimeRemaining
                });
                // _entityManager.RemoveComponent<Disabled>(allEntities[i]);
                // _entityManager.SetEnabled(allEntities[i], true);
                _entityManager.SetComponentEnabled<EnableComponent>(allEntities[i], true);
                _entityManager.SetComponentEnabled<MaterialMeshInfo>(allEntities[i], true);
                _entityManager.SetSharedComponent(allEntities[i], new PhysicsWorldIndex(0));
            }
            if (i % countCreatePerFrame == 0 && i > 0)
            {
                await UniTask.NextFrame();
            }
        }

        ecb.Playback(_entityManager);
        ecb.Dispose();
        allEntities.Dispose();
    }

    // Update is called once per frame
    public void GenerateDots(List<ECSDataSpawn> listData)
    {
        // 1. SIMPLE CREATE
        // EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

        // for (int i = 0; i < EntityCount; i++)
        // {

        //     Entity newEntity = ecb.CreateEntity();


        //     // ecb.AddComponent(newEntity, new SpawnCubeConfig { color = Color.red });
        //     ecb.AddComponent(newEntity, new URPMaterialPropertyBaseColor { Value = new Unity.Mathematics.float4(1,0,0,1) });


        // ecb.AddComponent(newEntity, new LocalTransform
        // {
        //     Position = new Unity.Mathematics.float3(
        //         UnityEngine.Random.Range(30f, 150f),
        //         UnityEngine.Random.Range(20f, 150f),
        //         UnityEngine.Random.Range(30, 150f)
        //     ),
        //     Rotation = Quaternion.identity,
        //     Scale = 1,
        // });

        // }

        // ecb.Playback(_entityManager);
        // ecb.Dispose();


        // 2. CREATE WITH SPAWNER
        // with buffer.
        EntityQuery query = _entityManager.CreateEntityQuery(typeof(SpawnCubeConfig));

        // // Get all entities matching the query
        NativeArray<Entity> allEntities = query.ToEntityArray(Allocator.Temp);

        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (Entity entity in allEntities)
        {
            if (_entityManager.HasComponent<SpawnCubeConfig>(entity))
            {
                SpawnCubeConfig spawnCubeConfig = _entityManager.GetComponentData<SpawnCubeConfig>(entity);

                for (int i = 0; i < listData.Count; i++)
                {
                    Entity spawnedEntity = ecb.Instantiate(spawnCubeConfig.cubePrefab);
                    ecb.AddComponent(spawnedEntity, new LocalTransform
                    {
                        Position = listData[i].position,
                        // new Unity.Mathematics.float3(
                        // UnityEngine.Random.insideUnitSphere.x * 5 + 30,
                        // UnityEngine.Random.insideUnitSphere.y * 5 + 30,
                        // UnityEngine.Random.insideUnitSphere.z * 5 + 30
                        // // UnityEngine.Random.Range(30f, 150f),
                        // // UnityEngine.Random.Range(20f, 150f),
                        // // UnityEngine.Random.Range(30, 150f)
                        // ),
                        Rotation = quaternion.identity,
                        Scale = listData[i].scale,
                    });

                    ecb.AddComponent(spawnedEntity, new URPMaterialPropertyBaseColor
                    {
                        Value =
                        new float4(
                            listData[i].color.r,
                            listData[i].color.g,
                            listData[i].color.b,
                            listData[i].color.a
                        ),
                    });
                    ecb.AddComponent(spawnedEntity, new VelocityComponent
                    {
                        direction = new Unity.Mathematics.float3(
                            listData[i].direction.x,
                            listData[i].direction.y,
                            listData[i].direction.z
                        ),
                        forceAmount = listData[i].forceAmount,
                    });
                    ecb.AddComponent(spawnedEntity, new LifetimeComponent
                    {
                        LifetimeRemaining = listData[i].lifetimeRemaining
                    });

                    // ecb.AddComponent(spawnedEntity, new MaterialMeshInfo());
                    ecb.AddComponent(spawnedEntity, new EnableComponent { });
                    ecb.SetComponentEnabled<EnableComponent>(spawnedEntity, false);
                    ecb.SetComponentEnabled<MaterialMeshInfo>(spawnedEntity, false);
                    ecb.SetSharedComponent(spawnedEntity, new PhysicsWorldIndex(1));
                    // ecb.AddComponent(spawnedEntity, new Disabled { });

                    // EntityManager.SetComponentData(spawnedEntity, new ColorComponent
                    // {
                    //     colorMaterial = new float4(0,1,0,1),
                    // });
                }
            }
        }

        ecb.Playback(_entityManager);
        ecb.Dispose();



        // 0. GET ENTITY
        // // Create a query that matches all entities (no specific component filters)
        // EntityQuery query = _entityManager.CreateEntityQuery(typeof(SpawnCubeConfig));

        // // Get all entities matching the query
        // NativeArray<Entity> allEntities = query.ToEntityArray(Allocator.Temp);

        // foreach (Entity entity in allEntities)
        // {
        //     Debug.Log($"Found entity: {entity}");
        //     // You can then interact with this entity, e.g., get its components
        //     // if (_entityManager.HasComponent<MyComponent>(entity))
        //     // {
        //     //     MyComponent componentData = _entityManager.GetComponentData<MyComponent>(entity);
        //     //     Debug.Log($"Entity {entity} has MyComponent with data: {componentData.Value}");
        //     // }
        // }

        // allEntities.Dispose(); // Remember to dispose NativeArrays
    }
    

    public void TestGetEntities()
    {
        // Создайте запрос, который соответствует всем сущностям (без определенных фильтров компонентов)
        // EntityQuery query = _entityManager.CreateEntityQuery(typeof(MaterialMeshInfo));
        EntityQuery query = _entityManager.CreateEntityQuery(new EntityQueryDesc
        {
            Disabled = new ComponentType[]
            {
                ComponentType.ReadWrite<EnableComponent>(),
            },
            // Optionally, you can also specify Any and None here
            // Any = new ComponentType[] { typeof(MyOptionalComponent) },
            // None = new ComponentType[] { typeof(MyExcludedComponent) }
        });
        
        // EntityQueryBuilder _disabledQuery = new EntityQueryBuilder(Allocator.Temp)
        //     // .WithAll<MyComponent>()
        //     .WithDisabled<EnableComponent>();

        // Get all entities matching the query
        NativeArray<Entity> allEntities = query.ToEntityArray(Allocator.Temp); //_disabledQuery.Build(_entityManager)

        Debug.Log($"Find {allEntities.Length} entities!");
        
        // foreach (Entity entity in allEntities)
        // {
        //     Debug.Log($"Found entity: {entity.Index}");
        //     // You can then interact with this entity, e.g., get its components
        //     // if (_entityManager.HasComponent<MyComponent>(entity))
        //     // {
        //     //     MyComponent componentData = _entityManager.GetComponentData<MyComponent>(entity);
        //     //     Debug.Log($"Entity {entity} has MyComponent with data: {componentData.Value}");
        //     // }
        // }


        allEntities.Dispose();
    }

    // Example Burst job that creates many entities
    [BurstCompile]
    public struct SpawnJob : IJobParallelFor
    {
        public Entity Prototype;
        public int EntityCount;
        public EntityCommandBuffer.ParallelWriter Ecb;

        public void Execute(int index)
        {
            // Clone the Prototype entity to create a new entity.
            var e = Ecb.Instantiate(index, Prototype);
            // Prototype has all correct components up front, can use SetComponent to
            // set values unique to the newly created entity, such as the transform.
            Ecb.SetComponent(index, e, new LocalToWorld { Value = ComputeTransform(index) });
            Ecb.SetComponentEnabled<EnableComponent>(index, e, false);
            // Ecb.SetEnabled(index, e, false);
        }

        public float4x4 ComputeTransform(int index)
        {
            return float4x4.Translate(new float3(0, 0, 0));
        }
    }

    public void OnTestRenderMeshUtility()
    {
        // var world = World.DefaultGameObjectInjectionWorld;
        var entityManager = _entityManager;  //world.EntityManager;

        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);

        // Create a RenderMeshDescription using the convenience constructor
        // with named parameters.
        var desc = new RenderMeshDescription(
            shadowCastingMode: ShadowCastingMode.Off,
            receiveShadows: false);

        // Create an array of mesh and material required for runtime rendering.
        var renderMeshArray = new RenderMeshArray(new UnityEngine.Material[] { Material }, new Mesh[] { Mesh });

        // Create empty base entity
        var prototype = entityManager.CreateEntity();

        // Call AddComponents to populate base entity with the components required
        // by Entities Graphics
        RenderMeshUtility.AddComponents(
            prototype,
            entityManager,
            desc,
            renderMeshArray,
            MaterialMeshInfo.FromRenderMeshArrayIndices(0, 0));

        if (GameManager.Instance.Settings.DebugSettings.ECSColliderSphere)
        {
            _collider = Unity.Physics.SphereCollider.Create(new SphereGeometry { Center = float3.zero, Radius = 0.5f });
        } else
        {
            _collider = Unity.Physics.BoxCollider.Create(new BoxGeometry { Center = float3.zero, Size = new float3(1, 1, 1), BevelRadius = 0, Orientation = quaternion.identity });
        }


        uint defaultLayerMask = (uint)1 << LayerMask.NameToLayer("Default");

        _collider.Value.SetCollisionFilter(new CollisionFilter
        {
            BelongsTo = defaultLayerMask,
            CollidesWith = defaultLayerMask
        });


        // TODO Leak
        entityManager.AddComponentData(prototype, new PhysicsCollider { Value = _collider });
        entityManager.AddComponentData(prototype, PhysicsMass.CreateDynamic(_collider.Value.MassProperties, 100.0f)); // 1.0f is mass
        entityManager.AddComponentData(prototype, new PhysicsVelocity { Linear = float3.zero, Angular = float3.zero });
        entityManager.AddComponentData(prototype, new PhysicsDamping { Linear = 1f, Angular = 0f });
        entityManager.AddComponentData(prototype, new LifetimeComponent { LifetimeRemaining = 1000f });
        entityManager.AddComponentData(prototype, new URPMaterialPropertyBaseColor { Value = new float4(0, 1, 0, 1) });
        entityManager.AddComponentData(prototype, new EnableComponent { });

        entityManager.SetComponentEnabled<EnableComponent>(prototype, false);
        entityManager.SetComponentEnabled<MaterialMeshInfo>(prototype, false);
        entityManager.AddSharedComponent(prototype, new PhysicsWorldIndex { Value = 1 });

        // Создаём большинство сущностей в задании Burst путём клонирования предварительно созданной сущности-прототипа,
        // которая может быть либо префабом, либо сущностью, созданной во время выполнения, как в этом примере.
        // Это самый быстрый и эффективный способ создания сущностей во время выполнения.
        var spawnJob = new SpawnJob
        {
            Prototype = prototype,
            Ecb = ecb.AsParallelWriter(),
            EntityCount = EntityCountGenerate,
        };

        var spawnHandle = spawnJob.Schedule(EntityCountGenerate, 128);
        spawnHandle.Complete();

        ecb.Playback(entityManager);
        ecb.Dispose();
        entityManager.DestroyEntity(prototype);
    }

    void OnDestroy()
    {
        _collider.Dispose();
    }
}


[System.Serializable]
public struct ECSDataSpawn
{
    public Vector3 position;
    public Color color;
    public Vector3 direction;
    public float scale;
    public float forceAmount;
    public float lifetimeRemaining;
}