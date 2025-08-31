using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public abstract class BaseMachine : MonoBehaviour
{
    // public static event Action<BaseMachine> OnChangeData;
    [SerializeField] protected LevelManager levelManager;
    public LevelManager LevelManager => levelManager;
    protected GameManager _gameManager => GameManager.Instance;
    public AudioSource AudioSource;
    [HideInInspector] public GameMachine Config;
    [HideInInspector] public MachineLevelData MachineLevelData;
    [HideInInspector] public StateController stateController;

    [Space(5)]
    [Header("Wrappers")]
    [SerializeField] protected GameObject BodyWrapper;
    [SerializeField] protected GameObject TowerWrapper;
    [SerializeField] protected GameObject CaterpillarWrapper;

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
    [SerializeField] protected bool isMove;
    public bool IsMove => isMove;
    [SerializeField] protected int offset = 90;
    public int OffsetRotate => offset;
    [SerializeField] protected DataMachine data = new();
    public DataMachine Data => data;
    [SerializeField] protected GridTileNode occupiedNode;
    public GridTileNode OccupiedNode => occupiedNode;


    [Space(5)]
    [Header("Other")]
    [SerializeField] private GameObject _objAreol;
    public GameObject Areol => _objAreol;
    [SerializeField] private BaseMachine _objectTarget;
    public BaseMachine ObjectTarget => _objectTarget;
    [SerializeField] protected Rigidbody rb;
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

    #region Unity
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
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(levelManager.Camera.isActiveAndEnabled ? levelManager.Camera : Camera);
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
        if (isVisible && isMove)
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
    #endregion

    public void OnCollision(Vector3 _pointCollision, bool isDrawMesh, GameObject explodeGameObject, int damageRadius)
    {
        // for (int i = 0; i < voxelMeshRender.Containers.Length; i++)
        // {
        // }
        Body.OnCollision(_pointCollision, isDrawMesh, explodeGameObject, damageRadius);

        for (int i = 0; i < Towers.Count; i++)
        {
            Towers[i].OnCollision(_pointCollision, isDrawMesh, explodeGameObject, damageRadius);
            for (int j = 0; j < Towers[i].Muzzles.Count; j++)
            {
                Towers[i].Muzzles[j].OnCollision(_pointCollision, isDrawMesh, explodeGameObject, damageRadius);
            }
        }
        for (int i = 0; i < Caterpillars.Count; i++)
        {
            Caterpillars[i].OnCollision(_pointCollision, isDrawMesh, explodeGameObject, damageRadius);
        }
    }

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

        transform.localScale = new Vector3(_gameManager.Settings.scaleObjects, _gameManager.Settings.scaleObjects, _gameManager.Settings.scaleObjects);

        Config = _config;

        MachineLevelData = dataInput;

        // Badge.Init(MachineLevelData);

        // устанавливаем звук мотора.
        AudioSource.clip = Config.soundMove;
        AudioSource.Play();

        // установка основных параметров.
        OnSetSpeed(Config.speed);
        
        // if (stateController.enabled)
        // {
        //     rb.isKinematic = true;
        // }

        if (navMeshAgent.enabled)
        {
            navMeshAgent.angularSpeed = 0;
            navMeshAgent.updateRotation = false;
            navMeshAgent.speed = Config.speed * 0.2f;
            navMeshAgent.updatePosition = false;
        }

        OnSetHP(Config.hp);

        if (HealthBar)
        {
            HealthBar.SetHealth(Config.hp, Config.hp);
        }

        data.timeBeforeAddTarget = stateController.enabled
            ? UnityEngine.Random.Range(_gameManager.Settings.timeBeforeAddTarget.x, _gameManager.Settings.timeBeforeAddTarget.y)
            : 0;

        // устанавливаем настройки для области атаки.
        areaSearch.Init(Config);

        // инициализируем компоненты машины
        GameBody _bodyConfig = Config.body;
        var _body = Instantiate(_bodyConfig.prefab, BodyWrapper.transform);
        body = _body;
        body.Init(this);


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
        Debug.Log($"parentTowers={parentTowers.Count}");
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
                        var _towChild = Instantiate(_optChildConfig.Config.prefab, TowerWrapper.transform);
                        _towChild.Init(this, _optChildConfig, 10 + i + j);
                        _towChild.OnSetParent(_tow);
                        towers.Add(_towChild);
                    }
                }
            }
        }

        OnSetAngleBody(0);

        // // установка герба.
        // Sprite logo = _gameManager.Settings.gerbs.Find(l => l.name == dataInput.gerbId);
        // body.OnSetSpriteGerb(logo);

        // test.
        // Badge.OnSetNameText(Data.speed.ToString());
    }

    public virtual void Rotate(Vector2 moveDirection)
    {
        if (stateController.enabled)
        {
            if (navMeshAgent.velocity != Vector3.zero)
            {
                
            Quaternion targetRotation = Quaternion.LookRotation(navMeshAgent.velocity);

            Body.transform.rotation = CaterpillarWrapper.transform.rotation = Quaternion.Slerp(
                Body.transform.rotation,
                Quaternion.Euler(0, targetRotation.eulerAngles.y + OffsetRotate, 0),
                10f * Time.fixedDeltaTime
            );
            }
        }
        else
        {
            if (rb.linearVelocity != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(rb.linearVelocity);
                // Debug.Log($"Rotate::::: {targetRotation}");
                // Quaternion stepRotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 100 * Time.fixedDeltaTime);

                // rb.MoveRotation(stepRotation);
                Body.transform.rotation = CaterpillarWrapper.transform.rotation = Quaternion.Slerp(
                    Body.transform.rotation,
                    Quaternion.Euler(0, targetRotation.eulerAngles.y + OffsetRotate, 0),
                    10f * Time.fixedDeltaTime
                );

                // Debug.Log($"ROTATION::::: stepRotation={stepRotation}");
            }
        }
    }

    public virtual void Move(Vector2 _moveDirection)
    {
        isMove = true;

        if (stateController.enabled)
        {
            rb.linearVelocity = navMeshAgent.velocity;
            Rotate(_moveDirection);
        }
        else
        {

            Vector3 forward;
            Vector3 right;

            if (_gameManager.Settings.simpleMove)
            {
                forward = levelManager.cinemachineCamera.transform.forward;  //(transform.position - levelManager.cinemachineCamera.transform.position).normalized;
                right = levelManager.cinemachineCamera.transform.right;
            }
            else
            {
                forward = transform.forward;
                right = transform.right;
            }
            ;

            forward.Normalize();
            right.Normalize();

            Vector3 moveDirection = (forward * _moveDirection.y + right * _moveDirection.x).normalized;

            Rotate(moveDirection);

            OnSetDirectionMove(moveDirection);

            // OnSetNameText(moveDirection.ToString());
            // transform.Translate(moveDirection * speed * Time.deltaTime);
            DataBonus bonusSpeed = null;
            Data.bonuses.TryGetValue(TypeBonus.Speed, out bonusSpeed);
            var speed = Data.speed + (bonusSpeed != null ? bonusSpeed.value : 0);

            // kinematic.
            // rb.MovePosition((Vector3)transform.position + (moveDirection * speed * Time.deltaTime));

            // dynamic.
            rb.linearVelocity = moveDirection * (Data.speed + (bonusSpeed != null ? bonusSpeed.value : 0)) * _gameManager.Settings.scaleObjects;
            // if (rb.linearVelocity.magnitude < 50f)
            // {
            //     rb.AddRelativeForce(moveDirection * (100f * Data.speed + (bonusSpeed != null ? bonusSpeed.value : 0)), ForceMode.Impulse); //linearVelocity = moveDirection * (Data.speed + (bonusSpeed != null ? bonusSpeed.value : 0));
            // }
            // else
            // {
            // }
            //     Debug.Log($"Magnitude={rb.linearVelocity.magnitude}");

            //rb.AddForce(moveDirection* (Data.speed * rb.mass + (bonusSpeed != null ? bonusSpeed.value : 0)), ForceMode.Force);

            // var directionVector = (transform.position - Data.position).normalized;
            // var movement = new Vector3(directionVector.x, 0f, directionVector.y);

            // Quaternion lookRotation = Quaternion.LookRotation(movement, Vector3.up);

            // Debug.Log($"{lookRotation.eulerAngles}, {lookRotation.x}, {lookRotation.y}, {lookRotation.z}");
            // OnSetAngleBody(lookRotation.eulerAngles.y);

            // OnSetAngleBody(moveDirection);

            Data.position = transform.position;

            // for (int i = 0; i < Caterpillars.Count; i++)
            // {
            //     Caterpillars[i].Move();
            // }
            // for (int i = 0; i < wheels.Count; i++)
            // {   
            //     wheels[i].transform.Rotate(Vector3.right, (20f * Data.speed) * Time.deltaTime);
            // }

            // Vector3Int posTile = levelManager.mapManager.Map.WorldToCell(transform.position);
            // GridTileNode node = levelManager.mapManager.gridTileHelper.GetNode(posTile);
            // SetOccupiedNode(node);
        }
        
        for (int i = 0; i < Caterpillars.Count; i++)
        {
            Caterpillars[i].Move();
        }
    }

    public virtual void Stop()
    {
        isMove = false;

        if (!rb.isKinematic)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

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

    public void OnSetHP(float hp)
    {
        data.hp = hp;
        if (HealthBar)
        {
            HealthBar.UpdateHealth(data.hp);
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

    public void OnSetAngleBody(float angle)
    {


        Body.transform.rotation = Quaternion.Euler(0, angle + OffsetRotate, 0);
        // TowerBox.transform.rotation = Quaternion.Euler(0, 0, angle + offset);
        _objAreol.transform.rotation = Quaternion.Euler(0, angle + OffsetRotate, 0);
        CaterpillarWrapper.transform.rotation = Quaternion.Euler(0, angle + OffsetRotate, 0);


        data.angleBody = Body.transform.eulerAngles.y;

        // for (int i = 0; i < Towers.Count; i++)
        // {
        //     Towers[i].ChangePosition(this);
        // }
    }

    public void OnShot(InputAction.CallbackContext context)
    {
        for (int i = 0; i < Towers.Count; i++)
        {
            BaseTower bt = Towers.ElementAt(i);
            for (int j = 0; j < bt.Muzzles.Count; j++)
            {
                bt.Muzzles.ElementAt(j).OnShot(null);
            }
        }
    }



    #region Debug
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(Body.transform.position, Body.transform.forward * 50);
        Gizmos.color = Color.yellow;
        for (int i = 0; i < Towers.Count; i++)
        {
            Gizmos.DrawRay(Towers[i].transform.position, Towers[i].transform.forward * 30);
            Gizmos.color = Color.blue;
            for (int j = 0; j < Towers[i].Muzzles.Count; j++)
            {
                Gizmos.DrawRay(Towers[i].Muzzles[j].transform.position, Towers[i].Muzzles[j].transform.forward * 50);
            }
        }
    }
    #endregion
    
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

    public void OnAddDamage(float v)
    {
        data.hp -= v;
        if (HealthBar)
        {
            HealthBar.UpdateHealth(data.hp);
        }

        // Badge.OnChangeData(this);

        if (!stateController.enabled)
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

    public void OnSetAngleBody(Vector3 direction)
    {
        Body.transform.forward = direction;
        CaterpillarWrapper.transform.forward = direction;

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
}
