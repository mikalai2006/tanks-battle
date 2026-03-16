using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

public abstract class BaseMachine : MonoBehaviour, IHealthed
{
    // public static event Action<BaseMachine> OnChangeData;
    // public static event System.Action<BaseMachine> OnChangeHPs;
    [SerializeField] protected LevelManager levelManager;
    public LevelManager LevelManager => levelManager;
    protected GameManager _gameManager => GameManager.Instance;
    public AudioSource AudioSource;
    [HideInInspector] public GameMachine Config;
    public MachineLevelData MachineLevelData;
    [HideInInspector] public StateController stateController;

    [Space(5)]
    [Header("Wrappers")]
    public GameObject Wrapper;
    public GameObject WrapperTools;
    [SerializeField] protected GameObject BodyWrapper;
    [SerializeField] protected GameObject TowerWrapper;
    [SerializeField] public GameObject MuzzleWrapper;
    [SerializeField] public GameObject CaterpillarWrapper;
    // [SerializeField] List<BaseMuzzle> muzzles;
    // public List<BaseMuzzle> Muzzles => muzzles;

    [Space(5)]
    [Header("Elements vehicle")]
    [SerializeField] BaseBody body;
    public BaseBody Body => body;
    [SerializeField] List<BaseTower> towers;
    public List<BaseTower> Towers => towers;
    [SerializeField] List<BaseCaterpillar> caterpillars;
    public List<BaseCaterpillar> Caterpillars => caterpillars;
    [SerializeField] List<BaseWheel> wheels;
    public List<BaseWheel> Wheels => wheels;


    [Space(5)]
    [Header("Data")]
    public bool inCamera;
    public bool isVisible;
    private bool _isSleep;
    public bool IsSleep
    {
        get => _isSleep;
        set
        {
            if (_isSleep != value)
            {
                _isSleep = value;
            }
        }
    }
    [SerializeField] protected int offset = 90;
    public int OffsetRotate => offset;
    [SerializeField] protected DataMachine data = new();
    public DataMachine Data => data;
    [SerializeField] protected GridTileNode occupiedNode;
    // public GridTileNode OccupiedNode => occupiedNode;


    [Space(5)]
    [Header("Other")]
    // [SerializeField] private GameObject _objAreol;
    // public GameObject Areol => _objAreol;
    // [SerializeField] private BaseMachine _objectTarget;
    // public BaseMachine ObjectTarget => _objectTarget;
    [SerializeField] protected Rigidbody rb;
    public Rigidbody Rb => rb;
    [SerializeField] private GameObject areaMoveGameObject;
    public GameObject AreaMoveGameObject => areaMoveGameObject;
    [SerializeField] private AreaMove areaMove;
    public AreaMove AreaMove => areaMove;
    [SerializeField] private AreaSearch areaSearch;
    public AreaSearch AreaSearch => areaSearch;
    [SerializeField] Camera _camera;
    public Camera Camera => _camera;
    public GameObject objectTargetCamera;
    // public Badge Badge;

    [Space(5)]
    [Header("Можно скрыть эти опции")]
    // [SerializeField] private IndicatorMachine _indicator;
    // public IndicatorMachine Indicator => _indicator;
    [SerializeField] private IndicatorManager _indicatorManager;
    public IndicatorManager IndicatorManager => _indicatorManager;
    [SerializeField] HealthBarController HealthBar;
    public NavMeshAgent navMeshAgent;
    public NavMeshObstacle navMeshObstacle;
    public bool IsMove => Body != null && Body.IsMove;

    [Space(5)]
    [Header("Сервисные опции")]
    Vector3 dirRayCamera;
    float distanceRayCamera;
    Plane[] planes;
    RaycastHit hitRayCamera;
    bool isRunningCoroutineCheckVisible;
    private CancellationTokenSource cancelTokenSource;

#region Unity methods
    public virtual void Awake()
    {
        cancelTokenSource = new CancellationTokenSource();

        areaMove = GetComponentInChildren<AreaMove>();
        HealthBar = GetComponentInChildren<HealthBarController>();
        stateController = GetComponent<StateController>();

        data = new();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        if (levelManager != null)
        {
            planes = GeometryUtility.CalculateFrustumPlanes(levelManager.Camera.isActiveAndEnabled ? levelManager.Camera : Camera);
        }
    }

  private void OnDestroy()
  {
    if (!cancelTokenSource.Token.IsCancellationRequested)
    {
      cancelTokenSource.Cancel();
      cancelTokenSource.Dispose();
    }
  }

    // void Update()
    // {
    //     // // вращение колес, если машина движется.
    //     // if (IsMove && Wheels.Count > 0)
    //     // {
    //     //     for (int i = 0; i < Wheels.Count; i++)
    //     //     {
    //     //         Wheels[i].transform.Rotate(Vector3.right, 5f * Body.Data.speed * Time.deltaTime);
    //     //     }
    //     // }

    //     // var occupiedNodes = levelManager.mapManager.gridTileHelper.GetAllGridNodes()
    //     //     .Where(n => n.OccupiedUnit != null)
    //     //     .ToList();
    //     // if (occupiedNodes.Count > 0)
    //     // {
    //     //     Debug.Log($"OccupiedNodes = {occupiedNodes.Count()}/{occupiedNodes[0].ToString()}");
    //     // }


    //     // else
    //     // {
    //     //     // если можно стрелять.
    //     //     if (Data.isShot)
    //     //     {
    //     //         // у машины-цели подсвечиваем зону.
    //     //         ObjectTarget.AreaAttack.OnSetColor(_gameManager.Settings.colorAreaAttackAttack);
    //     //         // у себя сектор обстрела.
    //     //         Tower.OnSetColorSector(_gameManager.Settings.colorAreaAttackAttack);
    //     //     }

    //     // }

    //     // // обновляем время последнего выстрела.
    //     // if (data.timeAfterLastShot <= Config.timeDelayNextMuzzle)
    //     // {
    //     //     data.timeAfterLastShot += Time.deltaTime;
    //     // }

    //     // if (MachineLevelData.isBot)
    //     // {
    //     //     if (planes != null) {
    //     //         // проверяем видим ли компонент.            
    //     //         if (GeometryUtility.TestPlanesAABB(planes, areaMove.Collider.bounds))
    //     //         {
    //     //             dirRayCamera = (transform.position - levelManager.Camera.transform.position).normalized;
    //     //             distanceRayCamera = Vector3.Distance(levelManager.Camera.transform.position, transform.position);

    //     //             // Debug.DrawLine(levelManager.Camera.transform.position, transform.position, Color.blue);
    //     //             if (Physics.Raycast(levelManager.Camera.transform.position, dirRayCamera, out hitRayCamera, distanceRayCamera, LayerMask.GetMask("Wall", "Machine") & ~LayerMask.GetMask("AreaSearch")))
    //     //             {
                        
    //     //                 // Debug.Log($"hit{hit.collider.name}");
    //     //                 if (hitRayCamera.transform != transform)
    //     //                 {
    //     //                     isVisible = false;
    //     //                 } else
    //     //                 {
    //     //                     isVisible = true;
    //     //                 }
    //     //                 // Debug.DrawRay(levelManager.Camera.transform.position, dir * distance, Color.yellow);
    //     //             }
    //     //             // else
    //     //             // {
    //     //             //     Debug.DrawRay(levelManager.Camera.transform.position, dir * distance, Color.white);
    //     //             // }
    //     //         } else
    //     //         {
    //     //             isVisible = false;
    //     //         }
    //     //     } else
    //     //     {
    //     //         Debug.LogWarning("Not found planes!");
    //     //     }
    //     //     // if (isVisible) {
    //     //     //     _indicator.gameObject.SetActive(false);
    //     //     //     isVisible = true;
    //     //     // }
    //     //     // else
    //     //     // {
    //     //     //     _indicator.gameObject.SetActive(true);
    //     //     //     isVisible = false;
    //     //     // }
    //     // }


    //     // // считаем время действия бонусов.
    //     // for (int i = 0; i < Data.bonuses.Count; i++)
    //     // {
    //     //     Data.bonuses.ElementAt(i).Value.time -= Time.deltaTime;

    //     //     if (Data.bonuses.ElementAt(i).Value.time <= 0)
    //     //     {
    //     //         TypeBonus key = Data.bonuses.ElementAt(i).Key;
    //     //         Data.bonuses.Remove(key);

    //     //         if (levelManager.UiTopSide.Target == this)
    //     //         {
    //     //             levelManager.UiTopSide.OnRemoveUIBonus(key);
    //     //         }
    //     //     }
    //     // }
    // }


    // void OnCollisionEnter(Collision collision)
    // {

    //     Container voxelContainer = collision.collider.GetComponent<Container>();
    //     if (voxelContainer != null)
    //     {
    //         Debug.Log($"<color=#FFA500FF>OnCollisionEnter {collision.collider.name}<{collision.contacts}></color>");
    //     }
    // }

    // void OnTriggerEnter(Collider collider)
    // {
    //     Container voxelContainer = collider.GetComponent<Container>();
    //     if (voxelContainer != null)
    //     {
    //         Debug.Log($"<color=#FFA500FF>OnTriggerEnter {collider.name}<{collider.ClosestPoint(transform.position)}></color>");
    //     }
    // }

    #endregion

    /// <summary>
    /// Функция обходит все комплектующие машины и проверяет на воксели в локальной точке (точке сопрокосновения со снарядом).
    /// </summary>
    /// <param name="_pointCollision">Точка контакта</param>
    /// <param name="isDrawMesh">Рисовать ли измененный меш</param>
    /// <param name="explodeGameObject"></param>
    /// <param name="damageRadius">Радиус уничтожения вокселей</param>
    public void OnCollision(BaseMachine ktoStrelyal, Vector3 _pointCollision, bool isDrawMesh, GameObject explodeGameObject, int damageRadius, Vector3 direction, Vector3 normal)
    {
        // for (int i = 0; i < voxelMeshRender.Containers.Length; i++)
        // {
        // }
        if (Body)
        {
            Body.OnCollision(ktoStrelyal, _pointCollision, isDrawMesh, explodeGameObject, damageRadius, direction, normal);
        }

        for (int i = 0; i < Towers.Count; i++)
        {
            Towers[i].OnCollision(ktoStrelyal, _pointCollision, isDrawMesh, explodeGameObject, damageRadius, direction, normal);
        }

        for (int i = 0; i < Caterpillars.Count; i++)
        {
            Caterpillars[i].OnCollision(ktoStrelyal, _pointCollision, isDrawMesh, explodeGameObject, damageRadius, direction, normal);
        }
        
        for (int i = 0; i < Wheels.Count; i++)
        {
           Wheels[i].OnCollision(ktoStrelyal, _pointCollision, isDrawMesh, explodeGameObject, damageRadius, direction, normal);
        }
    }

    public void OnSetIndicatorManager(IndicatorManager im)
    {
        _indicatorManager = im;
    }
    // public void OnSetIndicator(IndicatorMachine im)
    // {
    //     _indicator = im;
    // }

    // public void Initialize(MachineLevelData machineLevelData, GameMachine config)
    // {
    //     // Инициализируем менеджер уровня.
    //     LevelManager _levelManager = GameObject.FindGameObjectWithTag("LevelManager")?.GetComponent<LevelManager>();
    //     if (_levelManager != null)
    //     {
    //         levelManager = _levelManager;
    //     }

    //     // Сохраняем конфиг.
    //     Config = config;

    //     // Сохраняем данные машины для уровня.
    //     MachineLevelData = machineLevelData;

    //     // Сохраняем данные для частей машины.
    //     DataMachine = machineLevelData.data;

    //     // устанавливаем масштаб для машины.
    //     var scale = new Vector3(_gameManager.Settings.scaleObjects, _gameManager.Settings.scaleObjects, _gameManager.Settings.scaleObjects);
    //     if (Config.customScale > 0)
    //     {
    //         scale = new Vector3(Config.customScale, Config.customScale, Config.customScale);
    //     }
    //     Wrapper.transform.localScale = scale;

    //     // Если машина реального игрока, помечаем ее как видимая.
    //     if (!MachineLevelData.isBot)
    //     {
    //         isVisible = true;
    //     }

    // }

    public void Init(GameMachine _config, MachineLevelData dataInput, Vector3 scale = default)
    {
        LevelManager _levelManager = GameObject.FindGameObjectWithTag("LevelManager")?.GetComponent<LevelManager>();
        if (_levelManager != null)
        {
            levelManager = _levelManager;
        }

        Config = _config;

        // устанавливаем масштаб для машины.
        if (scale == default)
        {
            scale = new Vector3(_gameManager.Settings.scaleObjects, _gameManager.Settings.scaleObjects, _gameManager.Settings.scaleObjects);
            if (Config.customScale > 0)
            {
                scale = new Vector3(Config.customScale, Config.customScale, Config.customScale);
            }
        }
        Wrapper.transform.localScale = scale;

        MachineLevelData = dataInput;

        // Badge.Init(MachineLevelData);

        // // устанавливаем звук мотора.
        // AudioSource.clip = Config.soundMove;
        // AudioSource.Play();
        if (!MachineLevelData.isBot)
        {
            isVisible = true;
        }
        
        // if (stateController.enabled)
        // {
        //     rb.isKinematic = true;
        // }

        if (navMeshAgent.enabled)
        {
            navMeshAgent.angularSpeed = 0;
            navMeshAgent.updateRotation = false;
            // navMeshAgent.speed = Config.speed * 0.01f;
            navMeshAgent.updatePosition = false;
        }


        if (HealthBar)
        {
            HealthBar.SetHealth(1, 1);
            GameSceneEvents.SetHP?.Invoke(this);
        }

        OnSetHP(1); // Config.hp

        if (HealthBar && stateController.enabled == false)
        {
            HealthBar.gameObject.SetActive(false);
        }

        data.timeBeforeAddTarget = stateController.enabled
            ? UnityEngine.Random.Range(_gameManager.Settings.timeBeforeAddTarget.x, _gameManager.Settings.timeBeforeAddTarget.y)
            : 0;

        // устанавливаем настройки для области атаки.
        areaSearch.Init(Config);

        // Инициализируем детали машины.
        InitDetails(dataInput);

        // for (int i = 0; i < Config.catterpillars.Count; i++)
        // {
        //     GameCaterpillarOption _catConfig = Config.catterpillars.ElementAt(i);
        //     DataDetail dataCaterpillar = MachineLevelData.data.dataDetails.FirstOrDefault(t => t.nameConfig == _catConfig.Config.name && t.number == i);

        //     var _cat = Instantiate(_catConfig.Config.prefab, CaterpillarWrapper.transform);
        //     _cat.Init(this, _catConfig, i, dataCaterpillar);
        //     caterpillars.Add(_cat);
        // }

        // // init wheels.
        // for (int i = 0; i < Config.wheels.Count; i++)
        // {
        //     GameWheelOption _whConfig = Config.wheels.ElementAt(i);
        //     DataDetail dataWheel = MachineLevelData.data.dataDetails.FirstOrDefault(t => t.nameConfig == _whConfig.Config.name && t.number == i);

        //     var _wh = Instantiate(_whConfig.Config.prefab, CaterpillarWrapper.transform);
        //     _wh.Init(this, _whConfig, i, dataWheel);
        //     wheels.Add(_wh);
        // }

        // // инициализируем компоненты машины
        // if (Config.body != null)
        // {
        //     DataDetail dataBody = MachineLevelData.data.dataDetails.FirstOrDefault(t => t.nameConfig == Config.body.Config.name && t.number == 0);

        //     body = Instantiate(Config.body.Config.prefab, BodyWrapper.transform);
        //     body.Init(this, dataBody);
        // }

        // // init towers.
        // var parentTowers = Config.towers.FindAll(t => !t.isChildren);
        // for (int i = 0; i < parentTowers.Count; i++)
        // {
        //     GameTowerOption _optConfig = parentTowers.ElementAt(i);
        //     DataDetail dataTower = MachineLevelData.data.dataDetails.FirstOrDefault(t => t.nameConfig == _optConfig.Config.name && t.number == i);

        //     var _tow = CreateTower(_optConfig, dataTower, TowerWrapper.transform);

        //     // // создаем дуло.
        //     // for (int m = 0; m < _optConfig.muzzles.Count; m++)
        //     // {
        //     //     GameMuzzleOption _mConfig = _optConfig.muzzles.ElementAt(m);
        //     //     DataDetail dataMuzzle = MachineLevelData.data.dataDetails.FirstOrDefault(t => t.nameConfig == _mConfig.Config.name && t.number == m);

        //     //     var _muz = Instantiate(_mConfig.Config.prefab, _tow.MuzzlesBox.transform); // Machine.MuzzleWrapper.transform
        //     //     _muz.Init(this, _tow, _mConfig, m, dataMuzzle);
        //     //     muzzles.Add(_muz);
        //     // }


        //     if (_optConfig.children.Count > 0)
        //     {
        //         for (int j = 0; j < _optConfig.children.Count; j++)
        //         {
        //             GameTowerOption _optChildConfig = Config.towers.Find(t => t.ido == _optConfig.children.ElementAt(j));
        //             DataDetail dataTowerChild = MachineLevelData.data.dataDetails.FirstOrDefault(t => t.nameConfig == _optChildConfig.Config.name && t.number == j);

        //             if (_optChildConfig != null)
        //             {
                        
        //                 var _towChild = CreateTower(_optChildConfig, dataTowerChild, _tow.transform);
        //                 _towChild.OnSetParent(_tow);
                        
        //                 // // создаем дуло.
        //                 // for (int m = 0; m < _optChildConfig.muzzles.Count; m++)
        //                 // {
        //                 //     GameMuzzleOption _mConfig = _optChildConfig.muzzles.ElementAt(m);
        //                 //     DataDetail dataMuzzle = MachineLevelData.data.dataDetails.FirstOrDefault(t => t.nameConfig == _mConfig.Config.name && t.number == m);

        //                 //     var _muz = Instantiate(_mConfig.Config.prefab, _towChild.MuzzlesBox.transform); // Machine.MuzzleWrapper.transform
        //                 //     _muz.Init(this, _towChild, _mConfig, m, dataMuzzle);
        //                 //     muzzles.Add(_muz);
        //                 // }
        //             }
        //         }
        //     }
        // }

        // // установка герба.
        // Sprite logo = _gameManager.Settings.gerbs.Find(l => l.name == dataInput.gerbId);
        // body.OnSetSpriteGerb(logo);

        // test.
        // Badge.OnSetNameText(Data.speed.ToString());

        // RefreshHP();
    }

    public void InitDetails(MachineLevelData dataInput)
    {
        // Получаем все конфиги всех деталей.
        var allCaterpillars = _gameManager.ResourceSystem.GetAllCaterpillar();
        var allWheels = _gameManager.ResourceSystem.GetAllWheel();
        var allMuzzles = _gameManager.ResourceSystem.GetAllMuzzles();
        var allBodys = _gameManager.ResourceSystem.GetAllBody();

        // init caterpillars.
        var allConfigCaterpillars = dataInput.data.dataDetails.FindAll(x => x.type == VehicleDetailType.Caterpillar);
        for (int i = 0; i < allConfigCaterpillars.Count; i++)
        {
            GameCaterpillar _catConfig = allCaterpillars.First(x => x.name == allConfigCaterpillars.ElementAt(i).nameConfig);
            DataDetail dataCaterpillar = allConfigCaterpillars.ElementAt(i); //dataInput.data.dataDetails.FirstOrDefault(t => t.nameConfig == _catConfig.Config.name && t.number == i);

            var _cat = Instantiate(_catConfig.prefab, CaterpillarWrapper.transform);
            _cat.Init(this, _catConfig, i, dataCaterpillar);
            caterpillars.Add(_cat);
        }

        // init wheels.
        var allConfigWheels = dataInput.data.dataDetails.FindAll(x => x.type == VehicleDetailType.Wheel);
        for (int i = 0; i < allConfigWheels.Count; i++)
        {
            GameWheel _whConfig = allWheels.First(x => x.name == allConfigWheels.ElementAt(i).nameConfig);
            DataDetail dataWheel = allConfigWheels.ElementAt(i); //dataInput.data.dataDetails.FirstOrDefault(t => t.nameConfig == _whConfig.Config.name && t.number == i);

            var _wh = Instantiate(_whConfig.prefab, CaterpillarWrapper.transform);
            _wh.Init(this, _whConfig, i, dataWheel);
            wheels.Add(_wh);
        }

        // инициализируем компоненты машины
        var allConfigBodys = dataInput.data.dataDetails.FindAll(x => x.type == VehicleDetailType.Body);
        for (int i = 0; i < allConfigBodys.Count; i++)
        {
            GameBody _bConfig = allBodys.First(x => x.name == allConfigBodys.ElementAt(i).nameConfig);
            DataDetail dataBody = allConfigBodys.ElementAt(i);//dataInput.data.dataDetails.FirstOrDefault(t => t.nameConfig == _bConfig.Config.name && t.number == 0);

            body = Instantiate(_bConfig.prefab, BodyWrapper.transform);
            body.Init(this, dataBody);
        }

        // init towers.
        var allDataTowers = dataInput.data.dataDetails.FindAll(t => t.type == VehicleDetailType.Tower && string.IsNullOrEmpty(t.parentId));
        for (int i = 0; i < allDataTowers.Count; i++)
        {
            DataDetail dataTower = allDataTowers.ElementAt(i); //dataInput.data.dataDetails.FirstOrDefault(t => t.nameConfig == _optConfig.Config.name && t.number == i);

            var _tow = CreateTower(dataTower, TowerWrapper.transform);

            // // создаем дуло.
            // for (int m = 0; m < _optConfig.muzzles.Count; m++)
            // {
            //     GameMuzzleOption _mConfig = _optConfig.muzzles.ElementAt(m);
            //     DataDetail dataMuzzle = MachineLevelData.data.dataDetails.FirstOrDefault(t => t.nameConfig == _mConfig.Config.name && t.number == m);

            //     var _muz = Instantiate(_mConfig.Config.prefab, _tow.MuzzlesBox.transform); // Machine.MuzzleWrapper.transform
            //     _muz.Init(this, _tow, _mConfig, m, dataMuzzle);
            //     muzzles.Add(_muz);
            // }

            var allDataTowersNested = dataInput.data.dataDetails.FindAll(t => t.type == VehicleDetailType.Tower && !string.IsNullOrEmpty(t.parentId) && t.parentId == allDataTowers.ElementAt(i).ido);
            if (allDataTowersNested.Count > 0)
            {
                for (int j = 0; j < allDataTowersNested.Count; j++)
                {
                    // GameTowerOption _optChildConfig = Config.towers.First(x => x.Config.name == allDataTowersNested.ElementAt(j).nameConfig);
                    DataDetail dataTowerChild = allDataTowersNested.ElementAt(j); // dataInput.data.dataDetails.FirstOrDefault(t => t.nameConfig == _optChildConfig.Config.name && t.number == allConfigTowersNested.ElementAt(j).number);

                    var _towChild = CreateTower(dataTowerChild, _tow.transform);
                    _towChild.OnSetParent(_tow);
                    
                    // // создаем дуло.
                    // for (int m = 0; m < _optChildConfig.muzzles.Count; m++)
                    // {
                    //     GameMuzzleOption _mConfig = _optChildConfig.muzzles.ElementAt(m);
                    //     DataDetail dataMuzzle = MachineLevelData.data.dataDetails.FirstOrDefault(t => t.nameConfig == _mConfig.Config.name && t.number == m);

                    //     var _muz = Instantiate(_mConfig.Config.prefab, _towChild.MuzzlesBox.transform); // Machine.MuzzleWrapper.transform
                    //     _muz.Init(this, _towChild, _mConfig, m, dataMuzzle);
                    //     muzzles.Add(_muz);
                    // }
                }
            }
        }
    }

    public void ReDraw(List<ColorsModify> colors)
    {
        Debug.Log($"ReDraw {name}");

        Body.ReDraw(colors);

        for (int i = 0; i < Towers.Count; i++)
        {
            Towers[i].ReDraw(colors);
            for (int j = 0; j < Towers[i].Muzzles.Count; j++)
            {
                Towers[i].Muzzles[j].ReDraw(colors);
            }
        }
        for (int i = 0; i < Caterpillars.Count; i++)
        {
            Caterpillars[i].ReDraw(colors);
        }
        for (int i = 0; i < Wheels.Count; i++)
        {
            Wheels[i].ReDraw(colors);
        }
    }

    // public void ReDraw(StateMachinePlayerData stateMachinePlayerData)
    // {
    //     Debug.Log($"ReDraw {name} with dataDetails");


    // }


    public void OnSetHP(float hp)
    {
        // data.hp = hp;
        if (HealthBar)
        {
            HealthBar.UpdateHealth(hp);
        }

        // Badge.OnChangeData(this);

        // if (!stateController.enabled && levelManager != null)
        // {
        //     levelManager.UiTopSide.OnChangeData(this);
        // }
    }


    public void OnAddBonus(GameBonus configBonus)
    {
        if (data.bonuses.ContainsKey(configBonus.typeBonus))
        {
            data.bonuses[configBonus.typeBonus] = new()
            {
                id = configBonus.name.ToString(),
                time = configBonus.time,
                value = configBonus.value,
            };
        }
        else
        {
            data.bonuses.Add(configBonus.typeBonus, new()
            {
                id = configBonus.name.ToString(),
                time = configBonus.time,
                value = configBonus.value,
            }
            );
        }
    }

    public void OnShot()
    {
        for (int i = 0; i < Towers.Count; i++)
        {
            BaseTower tower = Towers.ElementAt(i);

            if (tower.IsBusy())
            {
                continue;
            }

            tower.OnShot();
        }
    }


    /// <summary>
    /// Функция рассчитывает HP (от 0 до 1, как отношение разрушенных ко всем вокселям)
    /// для всех комплектующих машины.
    /// </summary>
    public void RefreshHP()
    {
        if (transform == null) return;

        int countVoxels = 0;
        int countVoxelsDestructed = 0;
        if (Body)
        {
            ContainerData value = Body.RefreshHP();
            countVoxels += value.countVoxels;
            countVoxelsDestructed += value.countVoxelsDestructible;
            
            if (Body.Data.containerData.levelDestruction == 1)
            {
                Body.gameObject.SetActive(false);
                body = null;
            }
        }

        for (int i = 0; i < Towers.Count; i++)
        {
            ContainerData value = Towers[i].RefreshHP();
            countVoxels += value.countVoxels;
            countVoxelsDestructed += value.countVoxelsDestructible;

            for (int j = 0; j < Towers[i].Muzzles.Count; j++)
            {
                ContainerData valueMuzze = Towers[i].Muzzles[j].RefreshHP();
                countVoxels += valueMuzze.countVoxels;
                countVoxelsDestructed += valueMuzze.countVoxelsDestructible;
            }
        }

        for (int i = 0; i < Caterpillars.Count; i++)
        {
            ContainerData value = Caterpillars[i].RefreshHP();
            countVoxels += value.countVoxels;
            countVoxelsDestructed += value.countVoxelsDestructible;

            if (Caterpillars[i].Data.containerData.levelDestruction == 1)
            {
                Caterpillars[i].gameObject.SetActive(false);
                Caterpillars.Remove(Caterpillars[i]);
            }
        }
        

        for (int i = 0; i < Wheels.Count; i++)
        {
            ContainerData value = Wheels[i].RefreshHP();
            countVoxels += value.countVoxels;
            countVoxelsDestructed += value.countVoxelsDestructible;
            
            if (Wheels[i].Data.containerData.levelDestruction == 1)
            {
                Wheels[i].gameObject.SetActive(false);
                Wheels.Remove(Wheels[i]);
            }
        }

        Data.ContainerData.countVoxels = countVoxels;
        Data.ContainerData.countVoxelsDestructible = countVoxelsDestructed;

        Data.ContainerData.levelDestruction = (float)countVoxelsDestructed / countVoxels;

        OnSetHP(1f - Data.ContainerData.levelDestruction);
        // Debug.Log($"setHealth: {name}: {1f - Data.ContainerData.levelDestruction}");
        // if (MachineLevelData.isBot)
        // {
        //     OnChangeHPs?.Invoke(this);
        // }

        // обновляем привязки и позиции элементов.
        if (Body != null) {
            Body.SetRelativePoints();
        }
        for (int i = 0; i < Towers.Count; i++)
        {
            Towers[i].SetRelativePoints();
            for (int j = 0; j < Towers[i].Muzzles.Count; j++)
            {
                Towers[i].Muzzles[j].SetRelativePoints();
            }
        }

        for (int i = 0; i < Caterpillars.Count; i++)
        {
            Caterpillars[i].SetRelativePoints();
        }

        for (int i = 0; i < Wheels.Count; i++)
        {
            Wheels[i].SetRelativePoints();
        }


        if (!MachineLevelData.isBot)
        {
            GameSceneEvents.RefreshHP?.Invoke(this);
        }
    }

    public void SetNavObstacle()
    {
        navMeshObstacle = gameObject.AddComponent<NavMeshObstacle>();
        navMeshObstacle.center = Vector3.zero;
        navMeshObstacle.shape = NavMeshObstacleShape.Capsule;
        navMeshObstacle.radius = 0.3f;
        navMeshObstacle.height = 0.5f;
    }

    public virtual void Move(Vector3 moveDirection)
    {
        if (wheels.Count == 0)
        {
            Stop();
            return;
        }

        if (!Body)
        {
            return;
        }

        Body.Move(moveDirection);
    }

    public virtual void Stop()
    {
        if (!Body)
        {
            return;
        }

        Body.Stop();
    }

    public void OnSaveDestroyVoxels(List<RemoveVoxel> voxels, DataDetail dataDetail)
    {
        if (!MachineLevelData.isBot)
        {
            _gameManager.StateManager.SaveDestroyVoxelsMachine(voxels, dataDetail);
        }
    }

    /// <summary>
    /// пересоздание башни
    /// </summary>
    /// <param name="gameTowerOption"></param>
    /// <param name="dataDetail"></param>
    public BaseTower CreateTower(DataDetail dataDetail, Transform parent)
    {
        var allTowers = _gameManager.ResourceSystem.GetAllTower();
        // Debug.Log($"dataDetail.nameConfig={dataDetail.nameConfig}");
        GameTower configTower = allTowers.First(t => t.name == dataDetail.nameConfig);

        var _tow = Instantiate(configTower.prefab, parent);
        _tow.Init(this, configTower, dataDetail);
        towers.Add(_tow);

        return _tow;
    }

    public void SetInCamera(bool status)
    {
        if (!MachineLevelData.isBot) return;

        inCamera = status;

        if (status == true && !isRunningCoroutineCheckVisible)
        {
            // StartCoroutine(CheckVisibleMachine());
            CheckVisibleMachine(cancelTokenSource.Token).Forget();
        } else
        {
            // Indicator.gameObject.SetActive(true);
            IndicatorManager.SetShowIndicator(this);
        }
    }

    /// <summary>
    /// Корутина определяет находится ли объект машины в прямой видимости.
    /// </summary>
    // IEnumerator CheckVisibleMachine()
    async UniTask CheckVisibleMachine(CancellationToken token)
    {
        isRunningCoroutineCheckVisible = true;

        // Indicator.gameObject.SetActive(false);
        IndicatorManager.SetHideIndicator(this);

        // Ключевое условие: работает, пока переменная true
        while (inCamera && !token.IsCancellationRequested)
        {
            // Проводим линию от наблюдателя к цели
            // Linecast возвращает true, если что-то попалось на пути
            // (Indicator.Target.Body.transform.position + new Vector3(0,1.4f,0))
            // LevelManager.Camera.transform.position
            // var startObject = Indicator.Target.Towers.First();
            var startObject = IndicatorManager.Target.Towers.First();
            if (Physics.Linecast(startObject.transform.position + (startObject.transform.forward * 0.5f), transform.position+ new Vector3(0,0.1f,0), out RaycastHit hit, ~LayerMask.GetMask("Bullet", "AreaSearch", "Nature"))) //LayerMask.GetMask("Wall", "Machine") & 
            {
                // Debug.DrawLine(startObject.transform.position + (startObject.transform.forward * .5f), transform.position+ new Vector3(0,0.1f,0), Color.yellow, 5);
                // Если объект, в который попали, - это наша цель, значит, она видна
                if (hit.transform == transform)
                {
                    // Indicator.gameObject.SetActive(false);
                    IndicatorManager.SetHideIndicator(this);
                } else
                {
                    // Если попали во что-то другое, цель скрыта
                    // Indicator.gameObject.SetActive(true);
                    IndicatorManager.SetShowIndicator(this);
                    // Debug.Log($"Попали в {hit.transform.name}");
                }
            }
            // // Если ничего не попалось, цель видна
            // Indicator.gameObject.SetActive(false);

            // Задержка или возврат управления, чтобы не зависнуть
            // yield return new WaitForSeconds(0.10f);
            await UniTask.Delay(System.TimeSpan.FromSeconds(0.50f), cancellationToken: token);
        }

        isRunningCoroutineCheckVisible = false;
    }


    #region  Delete
    // public void SetOccupiedNode(GridTileNode node)
    // {
    //     if (node.OccupiedUnit != null)
    //     {
    //         return;
    //     }

    //     if (occupiedNode != null)
    //     {
    //         occupiedNode.SetOcuppiedUnit(null);
    //     }

    //     node.SetOcuppiedUnit(this);
    //     occupiedNode = node;
    //     // Debug.Log($"OccupiedNode = {OccupiedNode.ToString()}");
    // }
    // public void OnDrawAnimateText(string text)
    // {
    //     // Создаем текст с уроном
    //     TextDamage obText = Lean.Pool.LeanPool.Spawn(_gameManager.Settings.prefabTextDamage, levelManager.objectSpawnText.transform);
    //     if (obText)
    //     {
    //         obText.Init(this);
    //         obText.OnSetColor(_gameManager.Settings.colorTextDamage);
    //         obText.OnSetText(text);
    //     }
    // }

    // public void OnAddDamage(float v)
    // {
    //     // data.hp -= v;
    //     if (HealthBar)
    //     {
    //         HealthBar.UpdateHealth(data.hp);
    //     }

    //     // Badge.OnChangeData(this);

    //     if (!stateController.enabled)
    //     {
    //         levelManager.UiTopSide.OnChangeData(this);
    //     }

    //     for (int i = 0; i < Towers.Count; i++)
    //     {
    //         Towers[i].OnChangeData();
    //         Towers[i].OnDamageEffect(v);
    //     }

    //     Indicator.OnChangeData();
    //     // Body.OnChangeData();

    //     if (data.ContainerData.levelDestruction <= 0)
    //     {
    //         data.speed = 0;

    //         Stop();

    //         AudioSource.Stop();

    //         for (int i = 0; i < Towers.Count; i++)
    //         {
    //             Towers[i].PreDestroy();
    //         }

    //         levelManager.OnRemoveMachine(this);

    //         Destroy(gameObject);
    //     }
    // }

    // IEnumerator Follow()
    // {
    //     for (; ; ) //while(true)
    //     {                
    //         if (_objectTarget)
    //         {
    //             // TOWER
    //             var direction = _objectTarget.transform.position - transform.position;

    //             // // your actual heading as upwards parameter
    //             // Quaternion lookRotationTower = Quaternion.LookRotation(Vector3.forward, directionVectorTower);
    //             float angleInRadians = Mathf.Atan2(direction.y, direction.x);

    //             if (MachineLevelData.isBot || _gameManager.Settings.autoTakeEnemy) {
    //                 OnSetAngleTower(angleInRadians * Mathf.Rad2Deg);
    //             }

    //             float dist = Vector3.Distance(_objectTarget.transform.position, transform.position);
    //             if (dist <= tower.DistanceAttack)
    //             {
    //                 OnAttackTarget();
    //                 // // SetIsShot(true);
    //                 // if (Application.isEditor)
    //                 // {
    //                 //     Badge.OnSetNameText(dist.ToString());
    //                 // }
    //             }
    //             else
    //             {
    //                 OnViewTarget(_objectTarget);
    //                 // OnSetTarget(null);
    //             }

    //         }
    //         else
    //         {
    //             if (_gameManager.Settings.rotateTowerByBody)
    //             {
    //                 OnSetAngleTower(data.angleBody);
    //             }
    //             // else
    //             // {
    //             //     if (Data.angleTower != Data.angleTowerByBody)
    //             //     {
    //             //         Data.angleTowerByBody = Data.angleTower = Tower.transform.rotation.eulerAngles.z; //body.transform.localEulerAngles.z - (body.transform.localEulerAngles.z - tower.transform.localEulerAngles.z);
    //             //     }
    //             //     // OnSetAngleTower(Data.angleTower);
    //             //     Tower.transform.rotation = Quaternion.Euler(0, 0, Data.angleTower);
    //             //     // Debug.Log($"set angle {Data.angleTower}");
    //             // }
    //         }
    //         yield return new WaitForSeconds(1f / 100);
    //     }
    // }



    // public void OnResetTimeAfterLastShot(BaseMuzzle lastShotMuzzle)
    // {
    //     data.timeAfterLastShot = 0;
    //     data.muzzleLastShot = lastShotMuzzle;
    // }



    // void FixedUpdate()
    // {
    //     // if (MachineLevelData.isBot)
    //     // {
    //         float distanceRay = Config.distanceSearch;
    //         float offsetRay = areaMove.transform.localScale.x;
    //         Vector3 startRay = transform.position + offsetRay * Data.directionTower;
    //         RaycastHit2D hit = Physics2D.Raycast(startRay, Data.directionTower, Config.distanceSearch);
    //             // Debug.DrawRay(startRay, Data.directionTower, Color.blue);

    //         if (hit && !hit.collider.CompareTag("TilemapWithCollider") && Data.directionTower != Vector3.zero)
    //         {
    //             // Debug.Log($"hit {hit.collider}, {startRay}, {Data.directionTower}, {Config.distanceSearch}");
    //             Debug.DrawRay(startRay, hit.collider.transform.position - transform.position, Color.green);

    //             float distance = Vector3.Distance(hit.collider.transform.position, transform.position);

    //             if (distance <= Config.distanceSearch)
    //             {
    //                 BaseMachine bm = hit.collider.GetComponentInParent<BaseMachine>();

    //                 if (bm && bm != _objectTarget)
    //                 {
    //                     OnSetTarget(bm);
    //                     if (stateController)
    //                     {
    //                         stateController.ChangeState(stateController.chaseState);
    //                         stateController.chaseState.OnSetEnemy(bm);
    //                     }
    //                 }
    //             }
    //             else
    //             {
    //                 OnSetTarget(null);
    //             }
    //         }
    //         else
    //         {
    //             Debug.DrawRay(startRay, distanceRay * Data.directionTower, Color.white);
    //             OnSetTarget(null);
    //         }
    //     // }
    // }
    // public void OnCollisionEnter(Collision collision)
    // {
    //     Debug.Log($"Oncollision baseMachine {collision.gameObject.name}");
    //     Body.OnCollision(collision.contacts[0].point, true, collision);
    // }

    // #region Debug
    // void OnDrawGizmos()
    // {
    //     if (!Application.isPlaying)
    //     {
    //         return;
    //     }

    //     if (_gameManager.Settings.DebugSettings.gizmoBodyForwards)
    //     {
    //         Gizmos.color = Color.green;
    //         Gizmos.DrawRay(Body.transform.position, Body.transform.forward * 50);
    //     }

    //     for (int i = 0; i < Towers.Count; i++)
    //     {
    //         if (_gameManager.Settings.DebugSettings.gizmoTowersForwards)
    //         {
    //             Gizmos.color = Color.yellow;
    //             Gizmos.DrawRay(Towers[i].transform.position, Towers[i].transform.forward * 30);
    //         }

    //         // if (_gameManager.Settings.DebugSettings.gizmoMuzzlesForwards)
    //         // {
    //         //     Gizmos.color = Color.blue;
    //         //     for (int j = 0; j < Towers[i].Muzzles.Count; j++)
    //         //     {
    //         //         Gizmos.DrawRay(Towers[i].Muzzles[j].transform.position, Towers[i].Muzzles[j].transform.forward * 50);
    //         //     }
    //         // }
    //     }
    // }
    // #endregion
    #endregion
}
