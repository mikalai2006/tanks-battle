using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;
using Cysharp.Threading.Tasks;
using Loader;
using UIToolkitLibrary;
using Unity.VisualScripting;
using System.Linq;
using System;

public class LevelManager : MonoBehaviour
{
    public static event System.Action<string> OnSetNotify;
    public static event System.Action<float> OnAddProgress;
    public LoaderBarProvider LoaderBarProvider { get; private set; }
    private GameManager _gameManager => GameManager.Instance;
    private GameSetting _gameSetting => GameManager.Instance.Settings;
    // [SerializeField] public Tile3DGenerator tile3DGenerator;
    [SerializeField] public WFCGenerator wFCGenerator;
    [SerializeField] public MapManager mapManager;
    [SerializeField] public Light globalLight;
    [SerializeField] public List<BaseMachine> machines;
    [SerializeField] public List<IndicatorMachine> indicators;
    [SerializeField] public List<BaseBonus> bonuses;
    [SerializeField] public GameObject objectSpawnMachines;
    [SerializeField] public GameObject objectSpawnEffect;
    [SerializeField] public GameObject objectSpawnBonuses;
    [SerializeField] public GameObject objectSpawnText;
    [SerializeField] public GameObject objectSpawnIndicators;
    [SerializeField] public UITopSide UiTopSide;
    [SerializeField] public VariableJoystick JoystickMove;
    [SerializeField] public JoystickController JoystickTower;
    
    [SerializeField] Camera _camera;
    public Camera Camera => _camera;
    [SerializeField] CameraHandler _cameraHandler;
    public CameraHandler CameraHandler => _cameraHandler;
    public CinemachineCamera cinemachineCamera;
    public CinemachineOrbitalFollow cinemachineOrbitalFollow;
    private CreateMapOperation createMapOperation;
    public ECSManager ECSManager => _gameManager.ECSManager;
    System.Threading.CancellationTokenSource cancelToken;

    [Space(5)]
    [Header("Pools")]
    public ObjectPool poolBullet;

#region  Unity methods
    void Awake()
    {
        cancelToken = new System.Threading.CancellationTokenSource();
        LoaderBarProvider = new LoaderBarProvider();
        
        MapManager.OnCompleteBakeMap += OnSpawnObjects;
    }

    async void Start()
    {
        _gameManager.SetActiveCamera(Camera);

        await Init();
    }

    void OnDestroy()
    {

        MapManager.OnCompleteBakeMap -= OnSpawnObjects;

        createMapOperation.Dispose();

        cancelToken.Cancel();
        cancelToken.Dispose();
    }
    #endregion

    async UniTask Init()
    {
        createMapOperation = new CreateMapOperation(this);

        var operations = new Queue<ILoadingOperation>();
        operations.Enqueue(createMapOperation);
        await LoaderBarProvider.LoadAndDestroy(operations);

        // await StartGame(cancelToken);
    }

    public async UniTask StartGame(System.Threading.CancellationTokenSource cancelToken)
    {
        cinemachineOrbitalFollow = cinemachineCamera.GetComponent<CinemachineOrbitalFollow>();

        // OnSetActiveCamera(null);
        _camera = GameObject.FindGameObjectWithTag("CameraGame").GetComponent<Camera>();

        // if (_gameManager.LevelConfig.light >= .5f)
        // {
        //     globalLight.intensity = _gameManager.LevelConfig.light / 2;
        //     globalLight.enabled = true;
        // } else
        // {
        //     globalLight.enabled = false;
        // }
        
        globalLight.enabled = true;
        globalLight.intensity = _gameManager.LevelConfig.light;

        // создаем карту
        mapManager.OnInit(this);
        mapManager.CreateMap();
        // mapManager.OnCreateTestObjects();
        // tile3DGenerator.CreateMap();
        // if (!_gameManager.Settings.DebugSettings.disableCreateTiles)
        // {
        //     wFCGenerator.OnUpdateColors();
        //     wFCGenerator.OnCreateVariantsPrefabs();
        // }
        await wFCGenerator.OnGenerateTiles(cancelToken);


        // Показываем UI SideBar.
        MainMenuUIEvents.GameSideBarShown?.Invoke();
    }

    private void OnSpawnObjects()
    {
        OnSetNotify?.Invoke("createPlayers");
        OnAddProgress?.Invoke(.1f);
        // создаем игровые комманды
        if (_gameManager.LevelConfig.typeLevel == TypeLevel.Command)
        {
            _gameManager.StateManager.InitDataCommandLevel();

            // // проходим по коммандам и создаем игроков
            // foreach (TeamData team in _gameManager.StateManager.stateLevel.teams)
            // {
            //     for (int i = 0; i < _gameManager.LevelConfig.countPlayers; i++)
            //     {
            //         GridTileNode node = mapManager.gridTileHelper.GetAllGridNodes().Where(n =>
            //             !n.OccupiedUnit
            //             && n.StateNode.HasFlag(StateNode.Empty)
            //             && !n.StateNode.HasFlag(StateNode.Disable)
            //         ).OrderBy(t => UnityEngine.Random.value).First();

            //         if (node != null)
            //         {
            //             bool bot = true;

            //             GameMachine configMachine = _gameSetting.machines[UnityEngine.Random.Range(0, _gameSetting.machines.Count - 1)];

            //             if (team.index == 0 && i == 0)
            //             {
            //                 bot = false;
            //             }

            //             Addressables.InstantiateAsync(
            //                 configMachine.machinePrefab,
            //                 node.position,
            //                 Quaternion.identity,
            //                 objectSpawnMachines.transform
            //             ).Completed += (AsyncOperationHandle<GameObject> handle) => LoadedAsset(handle, configMachine, data);
            //         }
            //     }
            //     //team.machines.Add(obj);
            // }
        }
        else if (_gameManager.LevelConfig.typeLevel == TypeLevel.Alone)
        {
            _gameManager.StateManager.InitDataAloneLevel();

            // создаем игроков
            foreach (MachineLevelData data in _gameManager.StateManager.stateLevel.machines)
            {

                // GridTileNode node = mapManager.gridTileHelper.GetAllGridNodes().Where(n =>
                //     // !n.OccupiedUnit
                //     // && n.StateNode.HasFlag(StateNode.Empty)
                //     n.X > 1
                //     && n.X < _gameManager.LevelConfig.gridSize.x
                //     && n.Y > 1
                //     && n.Y < _gameManager.LevelConfig.gridSize.y
                //     && !n.StateNode.HasFlag(StateNode.Disable)
                // ).OrderBy(t => UnityEngine.Random.value).First();
                Vector3 pointSpawn = mapManager.GetRandomNavmeshLocation(_gameManager.LevelConfig.levelData.size.x);


                if (pointSpawn != Vector3.zero)
                {
                    GameMachine configMachine = _gameManager.ResourceSystem.GetAllMachines().Find(m => m.name == data.id);

                    // Addressables.InstantiateAsync
                    var gObject = Instantiate(
                        configMachine.machinePrefab,
                        pointSpawn,
                        //new Vector3(data.isBot ? 30 : 241, 0.5f, data.isBot ? 30 : 22),
                        // new Vector3(node.position.x, 0.5f, node.position.y),
                        Quaternion.identity,
                        objectSpawnMachines.transform
                    );
                    gObject.name = $"{configMachine.name}_{data.id}";
                    

                    BaseMachine obj = gObject.GetComponent<BaseMachine>();
                    if (obj != null)
                    {

                    var r = gObject.AddComponent<Rigidbody>();
                    // r.isKinematic = config.isKinematic;
                    r.mass = 1000;
                    r.freezeRotation = true;
                    r.interpolation = RigidbodyInterpolation.Interpolate;
                    r.collisionDetectionMode = CollisionDetectionMode.Continuous;
                    r.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;

                    IndicatorMachine indicatorObject = Instantiate(
                        configMachine.indicatorPrefab,
                        Vector3.zero,
                        Quaternion.identity,
                        objectSpawnIndicators.transform
                        // obj.WrapperTools.transform
                    );
                    if (indicatorObject != null)
                    {
                        obj.OnSetIndicator(indicatorObject);
                        indicatorObject.OnSetMachine(obj);
                        OnAddIndicator(indicatorObject);
                        // indicatorObject.OnSetTarget(obj);
                    }



                        // Debug.Log($"load {obj.name}/{team.index}");
                        if (data.isBot)
                        {
                            obj.GetComponent<PlayerController>().enabled = false;
                            obj.GetComponent<PlayerInput>().enabled = false;
                            var navMeshAgent = obj.navMeshAgent; //obj.GetComponent<NavMeshAgent>();
                            if (navMeshAgent != null)
                            {
                                navMeshAgent.Warp(pointSpawn);
                                navMeshAgent.updatePosition = false;
                                navMeshAgent.updateRotation = false;
                                // Debug.LogWarning($"pointSpawn => {pointSpawn}, gObject position={gObject.transform.position}");
                                navMeshAgent.enabled = true;
                            };
                            var lightComponent = obj.GetComponentInChildren<Light>();
                            if (lightComponent)
                            {
                                lightComponent.enabled = false;
                            }
                            // obj.Areol.SetActive(false);
                            // obj.GetComponent<CameraFollow>().enabled = false;
                            // obj.GetComponent<CameraFollowFPS>().enabled = false;
                            obj.GetComponent<StateController>().enabled = true;
                            // obj.OnSetConfig(configMachine, data);
                            // obj.SetOccupiedNode(node);
                            // obj.transform.position = _transform;
                        }
                        else
                        {
                            obj.GetComponent<PlayerController>().enabled = true;
                            obj.GetComponent<PlayerInput>().enabled = true;
                            // obj.GetComponentInChildren<NavMeshAgent>().enabled = false;
                            // var lightComponent = obj.GetComponentInChildren<Light>();
                            // if (lightComponent)
                            // {
                            //     lightComponent.enabled = true;
                            // }
                            // obj.Areol.SetActive(true);
                            // obj.GetComponent<CameraFollow>().enabled = false;
                            // obj.GetComponent<CameraFollowFPS>().enabled = false;
                            obj.GetComponent<StateController>().enabled = false;
                            // obj.OnSetConfig(configMachine, data);
                            // obj.SetOccupiedNode(node);
                            
                            var navMeshAgent = obj.navMeshAgent;
                            if (navMeshAgent != null)
                            {
                                navMeshAgent.Warp(pointSpawn);
                                navMeshAgent.enabled = false;
                            };

                            obj.SetNavObstacle();

                            CameraHandler.OnSetCharacter(obj);
                            CinemachineBrain brain = Camera.GetComponent<CinemachineBrain>();
                            if (brain != null)
                            {
                                // if (brain.ActiveVirtualCamera is CinemachineVirtualCamera activeCam)
                                // {
                                // }
                                cinemachineCamera.Follow = obj.objectTargetCamera.transform;
                                cinemachineCamera.LookAt = obj.objectTargetCamera.transform;
                            }

                            UiTopSide.crossHair.OnSetTarget(obj);
                            if (_gameManager.Settings.autoTakeEnemy)
                            {
                                UiTopSide.crossHair.gameObject.SetActive(false);
                            }
                        }

                        obj.Init(configMachine, data);

                        if (obj.Camera != null)
                        {
                            obj.Camera.gameObject.SetActive(false);
                        }

                        machines.Add(obj);
                        // team.machines.Add(obj);
                    }

                    //.Completed += (AsyncOperationHandle<GameObject> handle) => LoadedAsset(handle, configMachine, data, node);
                }
            }
        }

        // установка настроек для индикаторов машин на карте.
        // устанавливаем машину с камерой
        BaseMachine targetIndicator = machines.Find(m => !m.MachineLevelData.isBot);
        if (targetIndicator != null)
        {
            for (int i = 0; i < indicators.Count; i++)
            {
                IndicatorMachine ind = indicators[i].GetComponentInChildren<IndicatorMachine>();
                if (ind != null)
                {
                    ind.OnSetTarget(targetIndicator);
                }
            }
        }

        // создание списков машин для каждой машины.
        foreach (var machine in machines)
        {
            machine.AreaSearch.OnSynMachineList();
        }

        // spawn bonuses.
        // List<GridTileNode> vacantNodes = mapManager.gridTileHelper.GetEmptyNodes().OrderBy(t => UnityEngine.Random.value).ToList();
        // for (int i = 0; i < 15; i++)
        // {
        //     GameBonus configB = Helpers.GetProbabilityItem<GameBonus>(_gameManager.LevelConfig.bonuses).Item;
        //     OnSpawnBonus(vacantNodes[i], configB);
        // }
    }

    // public void OnSetActiveCamera(Camera value)
    // {
    //     if (value == null)
    //     {
    //         _camera = GameObject.FindGameObjectWithTag("CameraGame").GetComponent<Camera>();
    //     }
    //     else
    //     {
    //         // _camera = value;
    //     }
    // }

    public void OnRemoveMachine(BaseMachine _mach)
    {
        OnRemoveIndicator(_mach.Indicator);

        if (machines.Contains(_mach))
        {
            machines.Remove(_mach);
        }
    }

    public void OnAddIndicator(IndicatorMachine im)
    {
        if (!indicators.Contains(im))
        {
            indicators.Add(im);
        }
    }

    public void OnRemoveIndicator(IndicatorMachine im)
    {
        if (indicators.Contains(im))
        {
            im.DestroyGameObject();
            indicators.Remove(im);
        }
    }

    public void OnSpawnBonus(GridTileNode node, GameBonus configBonus)
    {
        var gObject = Instantiate(
            configBonus.prefabMap,
            node.position,
            Quaternion.identity,
            objectSpawnBonuses.transform
        );

        BaseBonus obj = gObject.GetComponent<BaseBonus>();
        obj.Init(configBonus);
    }

    
    public async UniTask CreateECS(List<RemoveVoxel> needCreateElements, float radiusExplode, Vector3 direction, Vector3 normal, Transform container)
    {
        if (!cancelToken.IsCancellationRequested)
        {
            float startTime = Time.realtimeSinceStartup;

            needCreateElements = needCreateElements.OrderBy(t => UnityEngine.Random.value).ToList();
            List<ECSDataSpawn> listData = new List<ECSDataSpawn>();
            var maxCount = Mathf.Min(
                GameManager.Instance.Settings.DebugSettings.mode == AppMode.Mobile ? GameManager.Instance.Settings.countMaxCreateVoxelsByStepMobile : GameManager.Instance.Settings.countMaxCreateVoxelsByStep,
                needCreateElements.Count
            );

            for (int i = 0; i < maxCount; i++)
            {
                var reflexDirection = Vector3.Reflect(direction, normal);
                var rot = Quaternion.Euler(
                    UnityEngine.Random.Range(0, 90),
                    UnityEngine.Random.Range(0, 90),
                    UnityEngine.Random.Range(0, 90)
                );
                var dir = rot * reflexDirection;
                listData.Add(new ECSDataSpawn
                {
                    color = needCreateElements[i].color,
                    direction = i % 60 != 0 ?  dir.normalized : UnityEngine.Random.onUnitSphere.normalized, // UnityEngine.Random.onUnitSphere,
                    forceAmount = UnityEngine.Random.Range(radiusExplode, 150 * radiusExplode),
                    lifetimeRemaining = UnityEngine.Random.Range(.3f, 1f),
                    position = container.TransformPoint(needCreateElements[i].position),
                    scale = GameManager.Instance.Settings.scaleObjects
                });
            }

            // Debug.Log($"Time CreateGravityECS: {(Time.realtimeSinceStartup - startTime) * 1000f} ms. \r\nCount  = {listData.Count}");
            // Debug.Log($"CreateGravityECS: {listData.Count}");

            // await UniTask.NextFrame();
            ECSManager.UpdateDataDots(listData).Forget();
            // Debug.Log($"Time CreateESC: {(Time.realtimeSinceStartup - startTime) * 1000f} ms, CreateECS: {maxCount}");
        }
    }

    //     public void LoadedAsset(AsyncOperationHandle<GameObject> handle, GameMachine configMachine, MachineLevelData data, GridTileNode node)
    //     {
    //         if (handle.Status == AsyncOperationStatus.Succeeded)
    //         {
    //             BaseMachine obj = handle.Result.GetComponent<BaseMachine>();
    //             if (obj != null)
    //             {
    //                 // Debug.Log($"load {obj.name}/{team.index}");
    //                 if (data.isBot)
    //                 {
    //                     obj.GetComponent<PlayerController>().enabled = false;
    //                     obj.GetComponent<PlayerInput>().enabled = false;
    //                     obj.GetComponentInChildren<Light2D>().enabled = false;
    //                     obj.Areol.SetActive(false);
    //                     obj.GetComponent<CameraFollow>().enabled = false;
    //                     obj.GetComponent<StateController>().enabled = true;
    //                     obj.OnSetConfig(configMachine, data);
    //                     obj.SetOccupiedNode(node);
    //                     // obj.transform.position = _transform;
    //                 }
    //                 else
    //                 {
    //                     obj.GetComponent<PlayerController>().enabled = true;
    //                     obj.GetComponent<PlayerInput>().enabled = true;
    //                     obj.GetComponentInChildren<Light2D>().enabled = true;
    //                     obj.Areol.SetActive(true);
    //                     obj.GetComponent<CameraFollow>().enabled = true;
    //                     obj.GetComponent<StateController>().enabled = false;
    //                     obj.OnSetConfig(configMachine, data);
    //                     obj.SetOccupiedNode(node);
    //                 }

    //                 // machines.Add(obj);
    //                 // team.machines.Add(obj);
    //             }
    //         }
    //         else
    //         {
    //             Debug.LogError($"Error Load prefab::: {handle.Status}");
    //         }
    //     }

}
