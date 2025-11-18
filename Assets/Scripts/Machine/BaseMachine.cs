using System;
using System.Collections.Generic;
using System.Linq;
using Mikalai2006.Voxel;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public abstract class BaseMachine : MonoBehaviour
{
    // public static event Action<BaseMachine> OnChangeData;
    // public static event System.Action<BaseMachine> OnChangeHPs;
    [SerializeField] protected LevelManager levelManager;
    public LevelManager LevelManager => levelManager;
    protected GameManager _gameManager => GameManager.Instance;
    public AudioSource AudioSource;
    [HideInInspector] public GameMachine Config;
    [HideInInspector] public MachineLevelData MachineLevelData;
    [HideInInspector] public StateController stateController;

    [Space(5)]
    [Header("Wrappers")]
    public GameObject Wrapper;
    public GameObject WrapperTools;
    [SerializeField] protected GameObject BodyWrapper;
    [SerializeField] protected GameObject TowerWrapper;
    [SerializeField] public GameObject MuzzleWrapper;
    [SerializeField] public GameObject CaterpillarWrapper;

    [Space(5)]
    [Header("Elements vehicle")]
    [SerializeField] BaseBody body;
    public BaseBody Body => body;
    [SerializeField] List<BaseTower> towers;
    public List<BaseTower> Towers => towers;
    [SerializeField] List<BaseCaterpillar> caterpillars;
    public List<BaseCaterpillar> Caterpillars => caterpillars;


    [Space(5)]
    [Header("Data")]
    public bool isVisible;
    [SerializeField] protected int offset = 90;
    public int OffsetRotate => offset;
    [SerializeField] protected DataMachine data = new();
    public DataMachine Data => data;
    [SerializeField] protected GridTileNode occupiedNode;
    // public GridTileNode OccupiedNode => occupiedNode;


    [Space(5)]
    [Header("Other")]
    [SerializeField] private GameObject _objAreol;
    public GameObject Areol => _objAreol;
    // [SerializeField] private BaseMachine _objectTarget;
    // public BaseMachine ObjectTarget => _objectTarget;
    [SerializeField] protected Rigidbody rb;
    public Rigidbody Rb => rb;
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
    [SerializeField] private IndicatorMachine _indicator;
    public IndicatorMachine Indicator => _indicator;
    [SerializeField] HealthBarController HealthBar;
    public NavMeshAgent navMeshAgent;
    public NavMeshObstacle navMeshObstacle;
    public bool IsMove => Body != null && Body.IsMove;

#region Unity methods
    public virtual void Awake()
    {
        areaMove = GetComponentInChildren<AreaMove>();
        rb = GetComponent<Rigidbody>();
        HealthBar = GetComponentInChildren<HealthBarController>();
        stateController = GetComponent<StateController>();

        data = new();
    }

    void Update()
    {
        // var occupiedNodes = levelManager.mapManager.gridTileHelper.GetAllGridNodes()
        //     .Where(n => n.OccupiedUnit != null)
        //     .ToList();
        // if (occupiedNodes.Count > 0)
        // {
        //     Debug.Log($"OccupiedNodes = {occupiedNodes.Count()}/{occupiedNodes[0].ToString()}");
        // }


        // else
        // {
        //     // если можно стрелять.
        //     if (Data.isShot)
        //     {
        //         // у машины-цели подсвечиваем зону.
        //         ObjectTarget.AreaAttack.OnSetColor(_gameManager.Settings.colorAreaAttackAttack);
        //         // у себя сектор обстрела.
        //         Tower.OnSetColorSector(_gameManager.Settings.colorAreaAttackAttack);
        //     }

        // }

        // // обновляем время последнего выстрела.
        // if (data.timeAfterLastShot <= Config.timeDelayNextMuzzle)
        // {
        //     data.timeAfterLastShot += Time.deltaTime;
        // }

        if (MachineLevelData.isBot)
        {
            // проверяем видим ли компонент.
            Plane[] planes = GeometryUtility.CalculateFrustumPlanes(levelManager.Camera.isActiveAndEnabled ? levelManager.Camera : Camera);
            if (GeometryUtility.TestPlanesAABB(planes, areaMove.Collider.bounds))
            {
                Vector3 dir = (transform.position - levelManager.Camera.transform.position).normalized;
                float distance = Vector3.Distance(levelManager.Camera.transform.position, transform.position);

                // Debug.DrawLine(levelManager.Camera.transform.position, transform.position, Color.blue);
                if (Physics.Raycast(levelManager.Camera.transform.position, dir, out RaycastHit hit, distance, LayerMask.GetMask("Wall", "Machine") & ~LayerMask.GetMask("AreaSearch")))
                {
                    
                    // Debug.Log($"hit{hit.collider.name}");
                    if (hit.transform != transform)
                    {
                        isVisible = false;
                    } else
                    {
                        isVisible = true;
                    }
                    Debug.DrawRay(levelManager.Camera.transform.position, dir * distance, Color.yellow);
                } else
                {
                    Debug.DrawRay(levelManager.Camera.transform.position, dir * distance, Color.white);
                }
            } else
            {
                isVisible = false;
            }

            if (isVisible) {
                _indicator.gameObject.SetActive(false);
                isVisible = true;
            }
            else
            {
                _indicator.gameObject.SetActive(true);
                isVisible = false;
            }
        }


        // // считаем время действия бонусов.
        // for (int i = 0; i < Data.bonuses.Count; i++)
        // {
        //     Data.bonuses.ElementAt(i).Value.time -= Time.deltaTime;

        //     if (Data.bonuses.ElementAt(i).Value.time <= 0)
        //     {
        //         TypeBonus key = Data.bonuses.ElementAt(i).Key;
        //         Data.bonuses.Remove(key);

        //         if (levelManager.UiTopSide.Target == this)
        //         {
        //             levelManager.UiTopSide.OnRemoveUIBonus(key);
        //         }
        //     }
        // }
    }

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
    public void OnCollision(Vector3 _pointCollision, bool isDrawMesh, GameObject explodeGameObject, int damageRadius, Vector3 direction, Vector3 normal)
    {
        // for (int i = 0; i < voxelMeshRender.Containers.Length; i++)
        // {
        // }
        if (Body)
        {
            Body.OnCollision(_pointCollision, isDrawMesh, explodeGameObject, damageRadius, direction, normal);
        }

        for (int i = 0; i < Towers.Count; i++)
        {
            Towers[i].OnCollision(_pointCollision, isDrawMesh, explodeGameObject, damageRadius, direction, normal);
        }

        for (int i = 0; i < Caterpillars.Count; i++)
        {
            Caterpillars[i].OnCollision(_pointCollision, isDrawMesh, explodeGameObject, damageRadius, direction, normal);
        }
    }

    public void OnSetIndicator(IndicatorMachine im)
    {
        _indicator = im;
    }


    public void Init(GameMachine _config, MachineLevelData dataInput)
    {
        LevelManager _levelManager = GameObject.FindGameObjectWithTag("LevelManager")?.GetComponent<LevelManager>();
        if (_levelManager != null)
        {
            levelManager = _levelManager;
        }

        Config = _config;

        // устанавливаем масштаб для машины.
        var scale = new Vector3(_gameManager.Settings.scaleObjects, _gameManager.Settings.scaleObjects, _gameManager.Settings.scaleObjects);
        if (Config.customScale > 0)
        {
            scale = new Vector3(Config.customScale, Config.customScale, Config.customScale);
        }
        Wrapper.transform.localScale = scale;

        MachineLevelData = dataInput;

        // Badge.Init(MachineLevelData);

        // устанавливаем звук мотора.
        AudioSource.clip = Config.soundMove;
        AudioSource.Play();
        
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
        }

        OnSetHP(1); // Config.hp

        if (stateController.enabled == false)
        {
            HealthBar.enabled = false;
        }

        data.timeBeforeAddTarget = stateController.enabled
            ? UnityEngine.Random.Range(_gameManager.Settings.timeBeforeAddTarget.x, _gameManager.Settings.timeBeforeAddTarget.y)
            : 0;

        // устанавливаем настройки для области атаки.
        areaSearch.Init(Config);

        // инициализируем компоненты машины
        if (Config.body)
        {
            body = Instantiate(Config.body.prefab, BodyWrapper.transform);
            body.Init(this);

        }

        // init caterpillars.
        for (int i = 0; i < Config.catterpillars.Count; i++)
        {
            GameCaterpillarOption _catConfig = Config.catterpillars.ElementAt(i);
            var _cat = Instantiate(_catConfig.Config.prefab, CaterpillarWrapper.transform);
            _cat.Init(this, _catConfig, i);
            caterpillars.Add(_cat);
        }

        // init towers.
        var parentTowers = Config.towers.FindAll(t => !t.isChildren);
        for (int i = 0; i < parentTowers.Count; i++)
        {
            GameTowerOption _optConfig = parentTowers.ElementAt(i);
            var _tow = Instantiate(_optConfig.Config.prefab, TowerWrapper.transform);
            _tow.Init(this, _optConfig, 10 + i);
            towers.Add(_tow);

            if (_optConfig.children.Count > 0)
            {
                for (int j = 0; j < _optConfig.children.Count; j++)
                {
                    GameTowerOption _optChildConfig = Config.towers.Find(t => t.ido == _optConfig.children.ElementAt(j));
                    if (_optChildConfig != null)
                    {
                        var _towChild = Instantiate(_optChildConfig.Config.prefab, _tow.transform);
                        _towChild.OnSetParent(_tow);
                        _towChild.Init(this, _optChildConfig, 10 + i + j);
                        towers.Add(_towChild);
                    }
                }
            }
        }

        // // установка герба.
        // Sprite logo = _gameManager.Settings.gerbs.Find(l => l.name == dataInput.gerbId);
        // body.OnSetSpriteGerb(logo);

        // test.
        // Badge.OnSetNameText(Data.speed.ToString());

        RefreshHP();
    }

    public void OnSetHP(float hp)
    {
        // data.hp = hp;
        if (HealthBar)
        {
            HealthBar.UpdateHealth(hp);
        }

        // Badge.OnChangeData(this);

        if (!stateController.enabled)
        {
            levelManager.UiTopSide.OnChangeData(this);
        }
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
        int countVoxels = 0;
        int countVoxelsDestructed = 0;

        if (Body)
        {
            ContainerData value = Body.RefreshHP();
            countVoxels += value.countVoxels;
            countVoxelsDestructed += value.countVoxelsDestructible;
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
        }

        Data.ContainerData.countVoxels = countVoxels;
        Data.ContainerData.countVoxelsDestructible = countVoxelsDestructed;

        Data.ContainerData.levelDestruction = (float)countVoxelsDestructed / countVoxels;

        OnSetHP(1 - Data.ContainerData.levelDestruction);
        // if (MachineLevelData.isBot)
        // {
        //     OnChangeHPs?.Invoke(this);
        // }
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

    #endregion



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
}
