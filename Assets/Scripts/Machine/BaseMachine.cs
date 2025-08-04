using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class BaseMachine : MonoBehaviour
{
    // public static event Action<BaseMachine> OnChangeData;
    public LevelManager levelManager;
    public LevelManager LevelManager => levelManager;
    GameManager _gameManager => GameManager.Instance;
    public AudioSource AudioSource;
    [SerializeField] public GameMachine Config;
    [SerializeField] public MachineLevelData MachineLevelData;
    [SerializeField] public StateController stateController;
    [SerializeField] private GridTileNode occupiedNode;
    public GridTileNode OccupiedNode => occupiedNode;
    [SerializeField] GameObject CaterpillarBox;
    [SerializeField] List<BaseCaterpillar> caterpillars;
    public List<BaseCaterpillar> Caterpillars => caterpillars;
    [SerializeField] GameObject TowerBox;
    [SerializeField] List<BaseTower> towers;
    public List<BaseTower> Towers => towers;
    [SerializeField] BaseBody body;
    public BaseBody Body => body;
    [SerializeField] private GameObject _objAreol;
    public GameObject Areol => _objAreol;
    [SerializeField] private DataMachine data = new();
    public DataMachine Data => data;
    [SerializeField] private BaseMachine _objectTarget;
    public BaseMachine ObjectTarget => _objectTarget;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private AreaMove areaMove;
    public AreaMove AreaMove => areaMove;
    [SerializeField] private AreaSearch areaSearch;
    public AreaSearch AreaSearch => areaSearch;
    // public Badge Badge;
    public bool isVisible;
    [SerializeField] private bool isMove;
    public bool IsMove => isMove;
    [SerializeField] private int offset = 0;

    [Space(5)]
    [Header("Можно скрыть эти опции")]
    [SerializeField] private IndicatorMachine _indicator;
    public IndicatorMachine Indicator => _indicator;

    [SerializeField] HealthBarController HealthBar;


    void Awake()
    {
        // tower = GetComponentInChildren<BaseTower>();
        // body = GetComponentInChildren<BaseBody>();
        // muzzle = GetComponentInChildren<BaseMuzzle>();
        // caterpillar = GetComponentInChildren<BaseCaterpillar>();
        stateController = GetComponent<StateController>();
        areaMove = GetComponentInChildren<AreaMove>();
        rb = GetComponent<Rigidbody>();

        HealthBar = GetComponentInChildren<HealthBarController>();

        data = new();
    }

    // void Start()
    // {

    //     StartCoroutine(Follow());
    // }

    public void OnSetIndicator(IndicatorMachine im)
    {
        _indicator = im;
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

        // Badge.Init(MachineLevelData);

        // устанавливаем звук мотора.
        AudioSource.clip = Config.soundMove;
        AudioSource.Play();

        // установка герба.
        Sprite logo = _gameManager.Settings.gerbs.Find(l => l.name == dataInput.gerbId);
        body.OnSetSpriteGerb(logo);

        // установка основных параметров.
        OnSetSpeed(Config.speed);

        OnSetHP(Config.hp);
        if (HealthBar)
        {
            HealthBar.SetHealth(Config.hp, Config.hp);
        }
        OnSetAngleBody(0);
        data.timeBeforeAddTarget = MachineLevelData.isBot
            ? UnityEngine.Random.Range(_gameManager.Settings.timeBeforeAddTarget.x, _gameManager.Settings.timeBeforeAddTarget.y)
            : 0;

        // устанавливаем настройки для области атаки.
        areaSearch.Init(Config);

        // инициализируем компоненты машины
        body.Init(this);

        // init caterpillars.
        for (int i = 0; i < Config.catterpillars.Count; i++)
        {
            GameCaterpillarOption _catConfig = Config.catterpillars.ElementAt(i);
            var _cat = Instantiate(_catConfig.Config.prefab, CaterpillarBox.transform);
            _cat.Init(this, _catConfig, i);
            caterpillars.Add(_cat);
        }

        // init towers.
        var parentTowers = Config.towers.FindAll(t => !t.isChildren);
        Debug.Log($"parentTowers={parentTowers.Count}");
        for (int i = 0; i < parentTowers.Count; i++)
        {
            GameTowerOption _optConfig = parentTowers.ElementAt(i);
            var _tow = Instantiate(_optConfig.Config.prefab, TowerBox.transform);
            _tow.Init(this, _optConfig, 10 + i);
            towers.Add(_tow);

            if (_optConfig.children.Count > 0)
            {
                for (int j = 0; j < _optConfig.children.Count; j++)
                {
                    GameTowerOption _optChildConfig = Config.towers.Find(t => t.ido == _optConfig.children.ElementAt(j));
                    if (_optChildConfig != null)
                    {
                        var _towChild = Instantiate(_optChildConfig.Config.prefab, TowerBox.transform);
                        _towChild.Init(this, _optChildConfig, 10 + i + j);
                        _towChild.OnSetParent(_tow);
                        towers.Add(_towChild);
                    }
                }
            }
        }

        // test.
        // Badge.OnSetNameText(Data.speed.ToString());
    }


    public void Move(Vector2 _moveDirection)
    {
        isMove = true;
        
        Vector3 moveDirection = new Vector3(_moveDirection.x, 0, _moveDirection.y).normalized;

        OnSetDirectionMove(moveDirection);

        // OnSetNameText(moveDirection.ToString());
        // transform.Translate(moveDirection * speed * Time.deltaTime);
        // rb.MovePosition((Vector2)transform.position + (moveDirection * speed * Time.deltaTime));
        DataBonus bonusSpeed = null;
        Data.bonuses.TryGetValue(TypeBonus.Speed, out bonusSpeed);
        rb.linearVelocity = moveDirection * (Data.speed + (bonusSpeed != null ? bonusSpeed.value : 0));

        //rb.AddForce(moveDirection* (Data.speed * rb.mass + (bonusSpeed != null ? bonusSpeed.value : 0)), ForceMode.Force);

        // var directionVector = (transform.position - Data.position).normalized;
        // var movement = new Vector3(directionVector.x, 0f, directionVector.y);

        // Quaternion lookRotation = Quaternion.LookRotation(movement, Vector3.up);

        // Debug.Log($"{lookRotation.eulerAngles}, {lookRotation.x}, {lookRotation.y}, {lookRotation.z}");
        // OnSetAngleBody(lookRotation.eulerAngles.y);

        OnSetAngleBody(moveDirection);

        Data.position = transform.position;

        for (int i = 0; i < Caterpillars.Count; i++)
        {  
            Caterpillars[i].Move();
        }
        // for (int i = 0; i < wheels.Count; i++)
        // {   
        //     wheels[i].transform.Rotate(Vector3.right, (20f * Data.speed) * Time.deltaTime);
        // }

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
        isMove = false;

        rb.linearVelocity = Vector2.zero;

        // _textName.text = _speed.ToString();

        for (int i = 0; i < Caterpillars.Count; i++)
        {   
            Caterpillars[i].Stop();
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

    public void OnAddDamage(float v)
    {
        data.hp -= v;
        if (HealthBar)
        {
            HealthBar.UpdateHealth(data.hp);
        }

        // Badge.OnChangeData(this);

            if (!MachineLevelData.isBot)
            {
                levelManager.UiTopSide.OnChangeData(this);
            }

        for (int i = 0; i < Towers.Count; i++)
        {
            Towers[i].OnChangeData();
            Towers[i].OnDamageEffect(v);
        }

        Indicator.OnChangeData();
        Body.OnChangeData();

        if (data.hp <= 0)
        {
            data.speed = 0;

            Stop();
            
            AudioSource.Stop();

            for (int i = 0; i < Towers.Count; i++)
            {
                Towers[i].PreDestroy();
            }

            levelManager.OnRemoveMachine(this);

            Destroy(gameObject);
        }
    }

    public void OnSetHP(float hp)
    {
        data.hp = hp;
        if (HealthBar)
        {
            HealthBar.UpdateHealth(data.hp);
        }

        // Badge.OnChangeData(this);

            if (!MachineLevelData.isBot)
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

    public void OnSetAngleBody(Vector3 direction)
    {
        Body.transform.forward = direction;
        CaterpillarBox.transform.forward = direction;

        // var rot = Body.transform.rotation;
        // _objAreol.transform.localEulerAngles = new Vector3(90, rot.eulerAngles.y, rot.eulerAngles.z);
        _objAreol.transform.forward = direction;

        data.angleBody = Body.transform.eulerAngles.y;

        // Debug.Log($"Current angle body: {data.angleBody}, euler={Body.transform.eulerAngles}");

        // Body.transform.rotation = Quaternion.Euler(0, angle + offset, 0);
        // // TowerBox.transform.rotation = Quaternion.Euler(0, 0, angle + offset);
        // _objAreol.transform.rotation = Quaternion.Euler(0, angle + offset, 0);
        // CaterpillarBox.transform.rotation = Quaternion.Euler(0, angle + offset, 0);

        // for (int i = 0; i < Towers.Count; i++)
        // {
        //     Towers[i].ChangePosition(this);
        // }
    }


    public void OnSetAngleBody(float angle)
    {


        Body.transform.rotation = Quaternion.Euler(0, angle + offset, 0);
        // TowerBox.transform.rotation = Quaternion.Euler(0, 0, angle + offset);
        _objAreol.transform.rotation = Quaternion.Euler(0, angle + offset, 0);
        CaterpillarBox.transform.rotation = Quaternion.Euler(0, angle + offset, 0);

        
        data.angleBody = Body.transform.eulerAngles.y;

        // for (int i = 0; i < Towers.Count; i++)
        // {
        //     Towers[i].ChangePosition(this);
        // }
    }

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


    void Update()
    {
        if (Data.currentAngleBody != Body.transform.localEulerAngles.y)
        {
            Data.currentAngleBody = Body.transform.localEulerAngles.y;
        }
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

        // проверяем видим ли компонент.
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(levelManager.Camera);
        if (GeometryUtility.TestPlanesAABB(planes, areaMove.Collider.bounds))
        // if (rd.isVisible == false)
        {
            _indicator.gameObject.SetActive(false);
            isVisible = true;
        }
        else
        {
            _indicator.gameObject.SetActive(true);
            isVisible = false;
        }

        // включаем или выключаем звук мотора.
        if (isVisible)
        {
            if (!AudioSource.isPlaying)
            {
                AudioSource.Play();
            }

            if (isMove)
            {
                AudioSource.volume = 0.5f;
            }
            else
            {
                AudioSource.volume = 0.1f;
            }
        }
        else
        {
            AudioSource.Stop();
        }

        // считаем время действия бонусов.
        for (int i = 0; i < Data.bonuses.Count; i++)
        {
            Data.bonuses.ElementAt(i).Value.time -= Time.deltaTime;

            if (Data.bonuses.ElementAt(i).Value.time <= 0)
            {
                TypeBonus key = Data.bonuses.ElementAt(i).Key;
                Data.bonuses.Remove(key);

                if (levelManager.UiTopSide.Target == this)
                {
                    levelManager.UiTopSide.OnRemoveUIBonus(key);
                }
            }
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
