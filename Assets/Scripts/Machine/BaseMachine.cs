using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

public abstract class BaseMachine : MonoBehaviour
{
    // public static event Action<BaseMachine> OnChangeData;
    private LevelManager levelManager;
    public LevelManager LevelManager => levelManager;
    GameManager _gameManager => GameManager.Instance;
    [SerializeField] public GameMachine Config;
    [SerializeField] public MachineLevelData MachineLevelData;
    [SerializeField] public StateController stateController;
    [SerializeField] private GridTileNode occupiedNode;
    public GridTileNode OccupiedNode => occupiedNode;
    [SerializeField] List<BaseMuzzle> muzzles;
    public List<BaseMuzzle> Muzzles => muzzles;
    [SerializeField] BaseCaterpillar caterpillar;
    public BaseCaterpillar Caterpillar => caterpillar;
    [SerializeField] BaseTower tower;
    public BaseTower Tower => tower;
    [SerializeField] BaseBody body;
    public BaseBody Body => body;
    [SerializeField] private GameObject _objAreol;
    public GameObject Areol => _objAreol;
    [SerializeField] private DataMachine data;
    public DataMachine Data => data;
    [SerializeField] private BaseMachine _objectTarget;
    public BaseMachine ObjectTarget => _objectTarget;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private AreaMove areaMove;
    public AreaMove AreaMove => areaMove;
    [SerializeField] private AreaAttack areaAttack;
    public AreaAttack AreaAttack => areaAttack;
    public Badge Badge;
    [SerializeField] private int offset = 90;

    [Space(5)]
    [Header("Можно скрыть эти опции")]
    [SerializeField] private Renderer rd;
    [SerializeField] private IndicatorMachine _indicator;
    public IndicatorMachine Indicator => _indicator;


    void Awake()
    {
        // tower = GetComponentInChildren<BaseTower>();
        // body = GetComponentInChildren<BaseBody>();
        // muzzle = GetComponentInChildren<BaseMuzzle>();
        // caterpillar = GetComponentInChildren<BaseCaterpillar>();
        stateController = GetComponent<StateController>();
        areaMove = GetComponentInChildren<AreaMove>();
        rd = body.GetComponentInChildren<Renderer>();
        rb = GetComponent<Rigidbody2D>();

        data = new();
    }

    void Start()
    {

        StartCoroutine(Follow());
    }

    public void OnSetIndicator(IndicatorMachine im)
    {
        _indicator = im;
    }

    public void OnSetTarget(BaseMachine target)
    {
        // если есть прошлый противник, устанавливаем, что невидим.
        if (_objectTarget != null)
        {
            OnNotViewTarget(_objectTarget);
        }

        // новому противнику устанавливаем, что видим.
        if (target != null)
        {
            OnViewTarget(target);
        }

        _objectTarget = target;
    }

    public void OnSetConfig(GameMachine _config, MachineLevelData dataInput)
    {
        LevelManager _levelManager = GameObject.FindGameObjectWithTag("LevelManager")?.GetComponent<LevelManager>();
        if (_levelManager != null)
        {
            levelManager = _levelManager;
        }

        Config = _config;

        MachineLevelData = dataInput;

        Badge.OnSetNameText(MachineLevelData.name);

        // установка герба.
        Sprite logo = _gameManager.Settings.gerbs.Find(l => l.name == dataInput.logo);
        body.OnSetSpriteGerb(logo);

        // установка основных параметров.
        OnSetSpeed(Config.speed);
        OnSetHP(Config.hp);
        OnSetSpeedRotateTower(Config.speedRotateTower);
        OnSetAngleTower(0);
        OnSetAngleBody(0);
        data.timeBeforeAddTarget = MachineLevelData.isBot
            ? UnityEngine.Random.Range(_gameManager.Settings.timeBeforeAddTarget.x, _gameManager.Settings.timeBeforeAddTarget.y)
            : 0;

        // устанавливаем настройки для области атаки.
        OnNotViewTarget(null);
        areaAttack.OnSetSize(Config.distanceSearch * 2);

        // инициализируем компоненты машины
        tower.OnSetSizeSector(Config.distanceAttack * 2);
        body.Init(this);
        tower.Init(this);
        for (int i = 0; i < muzzles.Count; i++)
        {
            BaseMuzzle _muz = muzzles.ElementAt(i);
            _muz.Init(this, i);
            data.muzzles.Add(_muz);
        }

        // test.
        // Badge.OnSetNameText(Data.speed.ToString());
    }


    public void Move(Vector2 moveDirection)
    {
        OnSetDirectionMove(moveDirection);

        // OnSetNameText(moveDirection.ToString());
        // transform.Translate(moveDirection * speed * Time.deltaTime);
        // rb.MovePosition((Vector2)transform.position + (moveDirection * speed * Time.deltaTime));
        DataBonus bonusSpeed = null;
        Data.bonuses.TryGetValue(TypeBonus.Speed, out bonusSpeed);
        rb.linearVelocity = moveDirection * (Data.speed + (bonusSpeed != null ? bonusSpeed.value : 0));

        var directionVector = (transform.position - Data.position).normalized;

        Quaternion lookRotation = Quaternion.LookRotation(Vector3.forward, directionVector);

        OnSetAngleBody(lookRotation.eulerAngles.z);

        Data.position = transform.position;

        Caterpillar.Move();

        Vector3Int posTile = levelManager.mapManager.Map.WorldToCell(transform.position);
        GridTileNode node = levelManager.mapManager.gridTileHelper.GetNode(posTile);
        SetOccupiedNode(node);
    }

    public void SetOccupiedNode(GridTileNode node)
    {
        if (node.OccupiedUnit != null)
        {
            return;
        }

        if (occupiedNode != null)
        {
            occupiedNode.SetOcuppiedUnit(null);
        }

        node.SetOcuppiedUnit(this);
        occupiedNode = node;
        // Debug.Log($"OccupiedNode = {OccupiedNode.ToString()}");
    }

    public void Stop()
    {
        rb.linearVelocity = Vector2.zero;

        // _textName.text = _speed.ToString();

        Caterpillar.Stop();
    }

    public void SetIsShot(bool status)
    {
        data.isShot = status;

        if (!status)
        {
            for (int i = 0; i < muzzles.Count; i++)
            {
                Muzzles[i].OnStopShot();
            }
        }
    }


    public void OnSetSpeed(float speed)
    {
        data.speed = speed;
    }

    public void OnSetDirectionMove(Vector3 direction)
    {
        data.directionMove = direction;
    }

    public void OnDrawAnimateText(string text)
    {
        // Создаем текст с уроном
        TextDamage obText = Lean.Pool.LeanPool.Spawn(_gameManager.Settings.prefabTextDamage, levelManager.objectSpawnText.transform);
        if (obText)
        {
            obText.Init(this);
            obText.OnSetText(text);
        }
    }

    public void OnAddDamage(float v)
    {
        data.hp -= v;
        Badge.OnChangeData(this);
        Tower.OnChangeData();
        Indicator.OnChangeData();
        Tower.transform.eulerAngles = new Vector3(0, 0, Tower.transform.eulerAngles.z + UnityEngine.Random.Range(-v * _gameManager.Settings.koofChangeAngleTower, v * _gameManager.Settings.koofChangeAngleTower));
        Body.OnChangeData();

        if (data.hp <= 0)
        {
            data.speed = 0;
            Stop();
            if (_objectTarget)
            {
                _objectTarget.OnSetTarget(null);
                OnSetTarget(null);
            }
            levelManager.OnRemoveMachine(this);
            Destroy(gameObject);
        }
    }

    public void OnSetHP(float hp)
    {
        data.hp = hp;
        Badge.OnChangeData(this);
    }

    public void OnSetSpeedRotateTower(float rotateTower)
    {
        data.speedRotateTower = rotateTower;
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

    /// <summary>
    /// Устанавливает, что машина попала в зону поиска врага.
    /// </summary>
    /// <param name="target">Машина, которая заметила</param>
    public void OnViewTarget(BaseMachine target)
    {
        if (_gameManager.Settings.drawAreaForBot || !target.MachineLevelData.isBot || !MachineLevelData.isBot)
        {
            target.areaAttack.OnSetColor(_gameManager.Settings.colorAreaAttackViewed);

            tower.OnSetColorSector(_gameManager.Settings.colorAreaAttackViewed);
        }

        SetIsShot(false);
    }

    /// <summary>
    /// Устанавливает, что машина попала в зону атаки врага.
    /// </summary>
    public void OnAttackTarget()
    {
        // смотрим дистанцию между машинами и выставляем статус, что можно стрелять,
        // если дистанция больше чем расстояние на котором запрещено стрелять
        float distance = Vector2.Distance(transform.position, _objectTarget.transform.position);
        if (distance > _gameManager.Settings.distanceDisableAttack)
        {
            if (_gameManager.Settings.drawAreaForBot || !ObjectTarget.MachineLevelData.isBot || !MachineLevelData.isBot)
            {
                _objectTarget.areaAttack.OnSetColor(_gameManager.Settings.colorAreaAttackAttack);

                tower.OnSetColorSector(_gameManager.Settings.colorAreaAttackAttack);
            }

            SetIsShot(true);
        }
    }

    /// <summary>
    /// Устанавливает, что машина вышла из зоны поиска врага.
    /// </summary>
    /// <param name="lastTarget">Последняя машина, которая видела текущую машину</param>
    public void OnNotViewTarget(BaseMachine lastTarget)
    {
        if (_gameManager.Settings.drawAreaForBot || (lastTarget != null && !lastTarget.MachineLevelData.isBot) || !MachineLevelData.isBot)
        {
            tower.OnSetColorSector(_gameManager.Settings.colorAreaAttackDefault);

            if (lastTarget)
            {
                lastTarget.areaAttack.OnSetColor(_gameManager.Settings.colorAreaAttackDefault);
            }
            else
            {
                areaAttack.OnSetColor(_gameManager.Settings.colorAreaAttackDefault);
            }
        }
    }

    /// <summary>
    /// Устанавливает фактический угол поворота башни.
    /// 
    /// </summary>
    /// <param name="angle">Угол, должени читаться из transform</param>
    public void OnSetCurrentAngleTower(float angle)
    {
        data.currentAngleTower = angle;
    }

    /// <summary>
    /// Устанавливает угол поворота башни из расчетов направления и поворота базы (body).
    /// </summary>
    /// <param name="angle">угол</param>
    public void OnSetAngleTower(float angle)
    {
        data.angleTower = angle;

        DataBonus bonusSpeedTower = null;
        Data.bonuses.TryGetValue(TypeBonus.SpeedTower, out bonusSpeedTower);
        //Tower.transform.rotation = Quaternion.Euler(0, 0, angle);
        Tower.transform.rotation = Quaternion.Lerp(
            Tower.transform.rotation,
            Quaternion.Euler(0, 0, angle),
            (data.speedRotateTower + (bonusSpeedTower != null ? bonusSpeedTower.value : 0)) * Time.deltaTime
        );

        OnSetDirectionTower(angle);

        // устанавливаем угол - разницу углов поворота башни и базы
        // Data.angleTowerByBody = Body.transform.localEulerAngles.z - Tower.transform.localEulerAngles.z;
        // Muzzle.transform.rotation = Quaternion.Lerp(Muzzle.transform.rotation, Quaternion.Euler(0, 0, lookRotationTower.eulerAngles.z + 90), .05f);
    }

    public void OnSetDirectionTower(float angle)
    {
        data.directionTower = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
    }

    public void OnSetAngleBody(float angle)
    {

        data.angleBody = angle + offset;

        Body.transform.rotation = Quaternion.Euler(0, 0, angle + offset);
        _objAreol.transform.rotation = Quaternion.Euler(0, 0, angle + offset);
        Caterpillar.transform.rotation = Quaternion.Euler(0, 0, angle + offset);

    }

    IEnumerator Follow()
    {
        for (; ; ) //while(true)
        {                
            if (_objectTarget)
            {
                // TOWER
                var direction = _objectTarget.transform.position - transform.position;

                // // your actual heading as upwards parameter
                // Quaternion lookRotationTower = Quaternion.LookRotation(Vector3.forward, directionVectorTower);
                float angleInRadians = Mathf.Atan2(direction.y, direction.x);

                OnSetAngleTower(angleInRadians * Mathf.Rad2Deg);

                float dist = Vector3.Distance(_objectTarget.transform.position, transform.position);
                if (dist <= Config.distanceAttack)
                {
                    OnAttackTarget();
                    // // SetIsShot(true);
                    // if (Application.isEditor)
                    // {
                    //     Badge.OnSetNameText(dist.ToString());
                    // }
                }
                else
                {
                    OnViewTarget(_objectTarget);
                    // OnSetTarget(null);
                }

            }
            else
            {
                if (_gameManager.Settings.rotateTowerByBody)
                {
                    OnSetAngleTower(data.angleBody);
                }
                // else
                // {
                //     if (Data.angleTower != Data.angleTowerByBody)
                //     {
                //         Data.angleTowerByBody = Data.angleTower = Tower.transform.rotation.eulerAngles.z; //body.transform.localEulerAngles.z - (body.transform.localEulerAngles.z - tower.transform.localEulerAngles.z);
                //     }
                //     // OnSetAngleTower(Data.angleTower);
                //     Tower.transform.rotation = Quaternion.Euler(0, 0, Data.angleTower);
                //     // Debug.Log($"set angle {Data.angleTower}");
                // }
            }
            yield return new WaitForSeconds(1f / 100);
        }
    }


    void Update()
    {
        // если есть возможные цели
        List<BaseMachine> _vacantTargets = AreaAttack.Targets
            .Where(t => t.Value >= data.timeBeforeAddTarget || !MachineLevelData.isBot)
            .Select(t => t.Key)
            .ToList();
        if (_vacantTargets.Count > 0) //  && !_objectTarget
        {
            // вычисляем дистанцию до существующей цели
            float distanceExistTarget = _objectTarget ? Vector2.Distance(transform.position, _objectTarget.transform.position) : 0;

            // выбираем ближайшую из возможных
            float minDistance = 0;
            BaseMachine minDistanceMachine = null;
            for (int i = 0; i < _vacantTargets.Count; i++)
            {
                BaseMachine mach = _vacantTargets[i];

                if (!mach)
                {
                    continue;
                }

                float dist = Vector2.Distance(transform.position, mach.transform.position);
                if (minDistance == 0 || minDistance > dist)
                {
                    minDistance = dist;
                    minDistanceMachine = mach;
                }
            }

            // если есть выбранная цель или выбранная ближе существующей
            if (minDistanceMachine != null && (distanceExistTarget == 0 || distanceExistTarget > minDistance))
            {
                OnSetTarget(minDistanceMachine);

                // if (stateController.enabled)
                // {
                //     stateController.ChangeState(stateController.chaseState);
                // }

            }
        }

        // если нет конечного автомата, проверяем есть ли противники в зоне досягаемости
        // и если нет - убираем цель
        if (AreaAttack.Targets.Count == 0)
        {
            OnSetTarget(null);
        }

        // var occupiedNodes = levelManager.mapManager.gridTileHelper.GetAllGridNodes()
        //     .Where(n => n.OccupiedUnit != null)
        //     .ToList();
        // if (occupiedNodes.Count > 0)
        // {
        //     Debug.Log($"OccupiedNodes = {occupiedNodes.Count()}/{occupiedNodes[0].ToString()}");
        // }


        // если нет цели, отменяем выстрелы
        if (ObjectTarget == null)
        {
            SetIsShot(false);
        }
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

        // провверяем видим ли компонент.
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(levelManager.Camera);
        if (GeometryUtility.TestPlanesAABB(planes , areaMove.Collider.bounds))
        // if (rd.isVisible == false)
        {
            _indicator.gameObject.SetActive(false);
        }
        else
        {
            _indicator.gameObject.SetActive(true);
        }
    }

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
}
