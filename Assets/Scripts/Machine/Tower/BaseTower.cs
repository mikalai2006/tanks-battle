using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Mikalai2006.Voxel;
using UnityEngine;
using UnityEngine.Rendering;


#if UNITY_EDITOR
using UnityEditor; 
#endif

public class BaseTower : MonoBehaviour, IColored
{
    protected GameManager _gameManager => GameManager.Instance;
    // [SerializeField] private Image _spriteSector;
    // [SerializeField] private RectTransform _rectSector;
    // [SerializeField] GameObject SectorGO;
    [SerializeField] GameObject MuzzlesBox;
    [SerializeField] GameObject Wrapper;
    [SerializeField] List<BaseMuzzle> muzzles;
    public List<BaseMuzzle> Muzzles => muzzles;
    [SerializeField] private SortingGroup sortingGroup;
    public GameTowerOption Option {get; private set ;}
    protected BaseMachine Machine;
    [SerializeField] private BaseMachine _objectTarget;
    public BaseMachine ObjectTarget => _objectTarget;
    // protected float distanceAttack;
    // public float DistanceAttack => distanceAttack;
    [SerializeField] protected DataTower _data;
    public DataTower Data => _data;
    protected BaseTower parent;
    public BaseTower Parent => parent;
    [SerializeField] private LayerMask ignoreMask;
    [SerializeField] private Vector3 targetPoint;
    [SerializeField] private Light PointLight;
    [SerializeField] private Light DirectionLight;
    
    // public float offset = 0;
    [SerializeField] protected VoxelMeshRender voxelMeshRender;

    [Space(5)]
    [Header("Сервисные опции")]
    Vector2 screenCenterPoint;
    Ray ray;
    Vector3 direction;
    Vector3 point;
    RaycastHit raycastHit;

#region Unity methods
    void Awake()
    {
        _data = new();
    }

    void Start()
    {
        // StartCoroutine(Follow());
    }
    
    void FixedUpdate()
    {
        if (Machine == null)
        {
            return;
        }

        // // проверяем наличие бонуса дистанции атаки.
        // DataBonus bonusDistanceAttack = null;
        // Machine.Data.bonuses.TryGetValue(TypeBonus.DistanceAttack, out bonusDistanceAttack);
        // if (bonusDistanceAttack != null)
        // {
        //     distanceAttack = Option.Config.distanceAttack + bonusDistanceAttack.value;
        //     OnSetSizeSector(distanceAttack);
        // }
        // else
        // {
        //     distanceAttack = Option.Config.distanceAttack;
        //     OnSetSizeSector(distanceAttack);
        // }


        // // изменяем угол сектора и его размер.
        // if (ObjectTarget != null && (!Machine.MachineLevelData.isBot || !ObjectTarget.MachineLevelData.isBot))
        // {
        //     OnSetAngleSector(Mathf.Max(5, Mathf.Abs(Mathf.DeltaAngle(Data.angleTower, Data.currentAngleTower))));
        // }


        if ((_gameManager.Settings.autoTakeEnemy || Machine.MachineLevelData.isBot) && Machine.AreaSearch.Targets.Count > 0)
        {
            foreach (KeyValuePair<BaseMachine, AreaSearchData> data in Machine.AreaSearch.Targets)
            {
                if (data.Value.isVisible)
                {
                    if (_objectTarget == null
                        || (
                        data.Key != _objectTarget
                        && data.Value.distance < Vector3.Distance(_objectTarget.transform.position, Machine.transform.position)
                        && data.Value.timeView >= Machine.Data.timeBeforeAddTarget
                        && _gameManager.Settings.takeNearEnemy
                        )
                    ) {
                        OnSetTarget(data.Key);
                    }
                }
            }
            // // если есть возможные цели
            // List<BaseMachine> _vacantTargets = Machine.AreaSearch.Targets
            //     .Where(t => t.Value.timeView >= Machine.Data.timeBeforeAddTarget || !Machine.MachineLevelData.isBot)
            //     .Select(t => t.Key)
            //     .ToList();
            // if (_vacantTargets.Count > 0) //  && !_objectTarget
            // {
            //     // вычисляем дистанцию до существующей цели
            //     float distanceExistTarget = _objectTarget ? Vector2.Distance(transform.position, _objectTarget.transform.position) : 0;
                
            //     // выбираем ближайшую из возможных
            //     float minDistance = 0;
            //     BaseMachine minDistanceMachine = null;
            //     for (int i = 0; i < _vacantTargets.Count; i++)
            //     {
            //         BaseMachine mach = _vacantTargets[i];

            //         if (!mach)
            //         {
            //             continue;
            //         }

            //         float dist = Vector2.Distance(transform.position, mach.transform.position);
            //         if (minDistance == 0 || (minDistance > dist && _gameManager.Settings.takeNearEnemy))
            //         {
            //             minDistance = dist;
            //             minDistanceMachine = mach;
            //         }
            //     }

            //     // если есть выбранная цель или выбранная ближе существующей
            //     if (minDistanceMachine != null && (distanceExistTarget == 0 || distanceExistTarget > minDistance))
            //     {
            //         OnSetTarget(minDistanceMachine);

            //         // if (stateController.enabled)
            //         // {
            //         //     stateController.ChangeState(stateController.chaseState);
            //         // }

            //     }
            // }
        }
        // else
        // {
        //     if (!Machine.MachineLevelData.isBot)
        //     {
        //         float distanceRay = DistanceAttack;
        //         float offsetRay = Machine.AreaMove.transform.localScale.x;
        //         Vector3 dirTower = new Vector2(Mathf.Cos(Data.currentAngleTower * Mathf.Deg2Rad), Mathf.Sin(Data.currentAngleTower * Mathf.Deg2Rad));
        //         Vector3 startRay = transform.position + offsetRay * dirTower;
        //         RaycastHit2D hit = Physics2D.Raycast(startRay, dirTower, distanceRay, 1 << 7);

        //         // string str = "";
        //         // for (int i = 0; i < hits.Length; i++)
        //         // {
        //         //     var hit = hits[i];
        //         if (hit && !hit.collider.CompareTag("TilemapWithCollider") && dirTower != Vector3.zero)
        //         {
        //             // Debug.Log($"hit {hit.collider}, {startRay}, {Data.directionTower}, {Config.distanceSearch}");


        //             float dist = Vector3.Distance(hit.collider.transform.position, transform.position);
        //             // str += hit.collider.gameObject.name;
        //             // str += " dist=" + dist + "(" + distanceRay + ")";
        //             // if (dist <= distanceRay)
        //             // {
        //             BaseMachine bm = hit.collider.GetComponentInParent<BaseMachine>();
        //             AreaMove amove = hit.collider.GetComponent<AreaMove>();

        //             if (bm && amove && bm != this && bm != ObjectTarget)
        //             {
        //                 // OnSetTarget(bm);
        //                 // if (stateController)
        //                 // {
        //                 //     stateController.ChangeState(stateController.chaseState);
        //                 //     stateController.chaseState.OnSetEnemy(bm);
        //                 // }
        //                 OnSetTarget(bm);
        //                 // if (_gameManager.Settings.DebugSettings.gizmo)
        //                 // Debug.DrawRay(startRay, dirTower * distanceRay, Color.yellow);
        //             }
        //         }
        //         // else
        //         // {
        //         //     OnSetTarget(null);
        //         //     Debug.DrawRay(startRay, dirTower * distanceRay, Color.magenta);
        //         // }
        //         // Debug.Log($"collision {str}");
        //     }
        // }

        // если машина, которая была в цели для башни, стала невидимой - убираем ее из цели
        if (_objectTarget != null && !Machine.AreaSearch.Targets[_objectTarget].isVisible)
        {
            OnSetTarget(null);
        }

        // // если нет цели, отменяем выстрелы
        // if (ObjectTarget == null)
        // {
        //     SetIsShot(false);
        // }


        // Отслеживание противников.
        if (ObjectTarget)
            {
                // Если башня вращается, начинаем поворот в сторону врага.
                if (Option.isRotate)
                {
                    // var direction = ObjectTarget.transform.position - transform.position;

                    // // // your actual heading as upwards parameter
                    // // Quaternion lookRotationTower = Quaternion.LookRotation(Vector3.forward, directionVectorTower);
                    // float angleInRadians = Mathf.Atan2(direction.y, direction.x);

                    if (Machine.MachineLevelData.isBot || _gameManager.Settings.autoTakeEnemy)
                    {
                        OnSetAngleTower(ObjectTarget.transform.position + new Vector3(0,6 * ObjectTarget.Wrapper.transform.localScale.y,0), true, Time.deltaTime); //angleInRadians * Mathf.Rad2Deg
                    }
                }
                // если башня не вращается - указываем ей угол поворота = углу поворота базы машины (мгновенно).
                // else
                // {
                //     OnSetAngleTower(Parent == null ? Machine.Data.angleBody : parent.Data.currentAngleTower, Option.isRotate);
                // }

                // // считаем дистанцию до врага, если дистанция меньше или равна указанной в параметрах - атакуем врага.
                // float dist = Vector3.Distance(ObjectTarget.transform.position, transform.position);
                // if (dist <= DistanceAttack)
                // {
                //     OnAttackTarget();
                //     // // SetIsShot(true);
                //     // if (Application.isEditor)
                //     // {
                //     //     Badge.OnSetNameText(dist.ToString());
                //     // }
                // }
                // else
                // {
                //     OnViewTarget(ObjectTarget);
                //     // OnSetTarget(null);
                // }
            }
            // else
            // {
            //     // if (_gameManager.Settings.rotateTowerByBody || !Option.isRotate)
            //     // {
            //     //     OnSetAngleTower(Parent == null ? Machine.Data.angleBody : parent.Data.currentAngleTower, Option.isRotate);
            //     // }
            //     // // else
            //     // // {
            //     // //     if (Data.angleTower != Data.angleTowerByBody)
            //     // //     {
            //     // //         Data.angleTowerByBody = Data.angleTower = Tower.transform.rotation.eulerAngles.z; //body.transform.localEulerAngles.z - (body.transform.localEulerAngles.z - tower.transform.localEulerAngles.z);
            //     // //     }
            //     // //     // OnSetAngleTower(Data.angleTower);
            //     // //     Tower.transform.rotation = Quaternion.Euler(0, 0, Data.angleTower);
            //     // //     // Debug.Log($"set angle {Data.angleTower}");
            //     // // }
            // }
    }

    void Update()
    {
        if (!Machine || !Machine.LevelManager)
        {
            return;
        }

        // наводим башню на центр экрана (если не активирован режим автонаводки)
        if (Machine.MachineLevelData.isBot == false && !_gameManager.Settings.autoTakeEnemy)
        {
            screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
            ray = Machine.LevelManager.Camera.ScreenPointToRay(screenCenterPoint);

            if (Physics.Raycast(ray, out raycastHit, 999f, ~(ignoreMask)))
            {
                direction = raycastHit.point - transform.position;
                point = raycastHit.point;
            }
            else
            {
                point = ray.GetPoint(999f); // Machine.levelManager.CameraHandler.TargetLook.transform.position;
                direction = point - transform.position; //Machine.levelManager.CameraHandler.TargetLook.transform.position - transform.position;
            }
            // Debug.DrawRay(ray.origin, direction, Color.cyan);

            // point = Machine.levelManager.CameraHandler.TargetLook.transform.position;
            // Debug.DrawLine(point, transform.position, Color.white);
            // Debug.Log($"ray {hit2.point}, direction={direction}");
            // Quaternion lookRotation = Quaternion.LookRotation(direction);
            OnSetAngleTower(point, true, Time.deltaTime);
            // Debug.DrawRay(Machine.LevelManager.Camera.transform.position, point - Machine.LevelManager.Camera.transform.position, Color.magenta);
            // targetPoint = point;
        }
    }

    void LateUpdate()
    {
        // Записываем в данные угол поворота башни.
        if (Data.currentAngleTower != transform.eulerAngles.y)
        {
            OnSetCurrentAngleTower(transform.rotation.eulerAngles.y);
        }
    }

#endregion

    public void Init(BaseMachine baseMachine, GameTowerOption optConfig, int index)
    {
        Option = optConfig;

        Machine = baseMachine;

        if (!Parent && _gameManager.LevelConfig != null)
        {
            DirectionLight.intensity = _gameManager.LevelConfig.light;
            DirectionLight.enabled = false;//!Machine.MachineLevelData.isBot;
            if (_gameManager.LevelConfig.light < 1)
            {
                PointLight.enabled = true; // Machine.MachineLevelData.isBot;
            } else
            {
                PointLight.enabled = false;
            } 
        } else
        {
            DirectionLight.gameObject.SetActive(false);
            PointLight.gameObject.SetActive(false);
        }

        voxelMeshRender.OnSetConfigMeshGenerator(Option.Config.MeshConfig);

        if (Machine.MachineLevelData != null && Machine.MachineLevelData.colorsModify != null && Machine.MachineLevelData.colorsModify.Count > 0)
        {
            // Debug.Log($"set colorsModify {Machine.MachineLevelData.colorsModify.Count}");
            voxelMeshRender.SetColorsModify(Machine.MachineLevelData.colorsModify);
        }

        OnSetSpeedRotateTower(optConfig.Config.speedRotateTower);
        // OnSetAngleTower(0);

        // sortingGroup.sortingOrder = index;

        // OnChangeData();

        // _sprite.color = Option.colorTower;

        // _damageSprite.size = _sprite.size;

        // _sprite.sprite = Option.Config.spriteTower;

        // distanceAttack = Option.Config.distanceAttack;

        // OnSetSizeSector(distanceAttack);

        transform.localPosition = new Vector3(Option.offsetTower.x, Option.offsetTower.y, Option.offsetTower.z);

        // инициализируем компоненты машины
        for (int i = 0; i < optConfig.muzzles.Count; i++)
        {
            GameMuzzleOption _mConfig = optConfig.muzzles.ElementAt(i);
            var _muz = Instantiate(_mConfig.Config.prefab, MuzzlesBox.transform); // Machine.MuzzleWrapper.transform
            _muz.Init(baseMachine, this, _mConfig, i);
            muzzles.Add(_muz);
        }

        OnNotViewTarget(null);

        // SectorGO.transform.localPosition = new Vector3(
        //     SectorGO.transform.localPosition.x,
        //     -(transform.position.y * (1 / Machine.Config.customScale)) + 0.05f,
        //     SectorGO.transform.localPosition.z
        // );

        // ChangePosition(baseMachine);
    }


    public void SetBusy(bool status)
    {
        for (int i = 0; i < Muzzles.Count; i++)
        {
            Muzzles[i].SetBusy(status);
        }
    }

    public bool IsBusy()
    {
        var result = true;

        for (int i = 0; i < Muzzles.Count; i++)
        {
            if (!Muzzles[i].IsBusy)
            {
                result = false;
            }
        }

        return result;
    }

    // public void OnChangeData()
    // {
    //     // Color col = Color.white;
    //     // col.a = 1f - Mathf.Min(1f, Machine.Data.hp * 100f / Machine.Config.hp * 0.01f);

    //     // _damageSprite.color = col;
    // }

    
    public void OnSetSpeedRotateTower(float rotateTower)
    {
        _data.speedRotateTower = rotateTower;
    }



    /// <summary>
    /// Устанавливает фактический угол поворота башни.
    /// 
    /// </summary>
    /// <param name="angle">Угол, должени читаться из transform</param>
    public void OnSetCurrentAngleTower(float angle)
    {
        _data.currentAngleTower = angle;
    }

    /// <summary>
    /// Устанавливает угол поворота башни из расчетов направления и поворота базы (body).
    /// </summary>
    public void OnSetAngleTower(Vector3 point, bool bySpeed = true, float deltaTime = 0)
    {
        Vector3 direction = point - transform.position;

        Quaternion lookRotation = Quaternion.LookRotation(direction);
        float angle = lookRotation.eulerAngles.y; // + offset;
        // angle = angle + Machine.levelManager.Camera.transform.rotation.eulerAngles.y;
        _data.angleTower = angle;

        
        // float distance = Vector3.Distance(targetPoint, point);
        // // Debug.Log($"_data.angleTower = {_data.angleTower}:::{_data.currentAngleTower}");
        // TODO
        // if (Math.Truncate(_data.angleTower) != Math.Truncate(_data.currentAngleTower))
        if (true)
        {
            OnSetDirectionTower(angle);

            float speedRotation = 1;
            if (Option.isRotate)
            {
                DataBonus bonusSpeedTower = null;
                Machine.Data.bonuses.TryGetValue(TypeBonus.SpeedTower, out bonusSpeedTower);
                //Tower.transform.rotation = Quaternion.Euler(0, 0, angle);

                // Machine.WrapperCamera.transform.rotation = Quaternion.Lerp(
                //     transform.rotation,
                //     Quaternion.Euler(0, angle, 0),
                //     bySpeed ? (_data.speedRotateTower + (bonusSpeedTower != null ? bonusSpeedTower.value : 0)) * Time.deltaTime : 1
                // );
                speedRotation = bySpeed ? (_data.speedRotateTower * (_gameManager.Settings.DebugSettings.mode == AppMode.Mobile ? 1 : 1) + (bonusSpeedTower != null ? bonusSpeedTower.value : 0)) * deltaTime : 1;
                // Debug.Log($"rotation speed={speedRotation}-{transform.rotation}");
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    Quaternion.Euler(0, angle, 0),
                    speedRotation
                );
                
            } else
            {
                transform.rotation = Quaternion.Euler(
                    Parent.transform.eulerAngles.x,
                    Parent.transform.eulerAngles.y,
                    Parent.transform.eulerAngles.z
                );
            }

            for (int j = 0; j < Muzzles.Count; j++)
            {
                Muzzles[j].OnSetRotation(point, speedRotation);
            }


        }
        // устанавливаем угол - разницу углов поворота башни и базы
        // Data.angleTowerByBody = Body.transform.localEulerAngles.z - Tower.transform.localEulerAngles.z;
        // Muzzle.transform.rotation = Quaternion.Lerp(Muzzle.transform.rotation, Quaternion.Euler(0, 0, lookRotationTower.eulerAngles.z + 90), .05f);
    }

    // public void OnSetAngleTower(Vector3 direction, bool bySpeed = true)
    // {
    //     Quaternion lookRotation = Quaternion.LookRotation(direction);
    //     var angleY = lookRotation.eulerAngles.y;

    //     // angle = angle + Machine.levelManager.Camera.transform.rotation.eulerAngles.y;
    //     _data.angleTower = angleY;

    //     OnSetDirectionTower(angleY);

    //     DataBonus bonusSpeedTower = null;
    //     Machine.Data.bonuses.TryGetValue(TypeBonus.SpeedTower, out bonusSpeedTower);
    //     //Tower.transform.rotation = Quaternion.Euler(0, 0, angle);

    //     // Machine.WrapperCamera.transform.rotation = Quaternion.Lerp(
    //     //     transform.rotation,
    //     //     Quaternion.Euler(0, angle, 0),
    //     //     bySpeed ? (_data.speedRotateTower + (bonusSpeedTower != null ? bonusSpeedTower.value : 0)) * Time.deltaTime : 1
    //     // );
    //     var speedRotate = bySpeed ? (_data.speedRotateTower + (bonusSpeedTower != null ? bonusSpeedTower.value : 0)) * Time.deltaTime : 1;
    //     transform.rotation = Quaternion.Lerp(
    //         transform.rotation,
    //         Quaternion.Euler(0, angleY, 0),
    //         speedRotate
    //     );

    //     for (int i = 0; i < Muzzles.Count; i++)
    //     {
    //         // Muzzles.ElementAt(i).OnSetAngle(lookRotation.eulerAngles.x, speedRotate);
    //         Muzzles.ElementAt(i).OnSetRotation(lookRotation, speedRotate);
    //     }
    // }
    

    public void OnSetDirectionTower(float angle)
    {
        _data.directionTower = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 0, Mathf.Sin(angle * Mathf.Deg2Rad));
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


    /// <summary>
    /// Устанавливает, что машина попала в зону поиска врага.
    /// </summary>
    /// <param name="target">Машина, которая заметила</param>
    public void OnViewTarget(BaseMachine target)
    {
        if (_gameManager.Settings.drawAreaForBot || !target.MachineLevelData.isBot || !Machine.MachineLevelData.isBot)
        {
            target.AreaSearch.OnSetColor(_gameManager.Settings.colorAreaAttackViewed);

            for (int i = 0; i < Muzzles.Count; i++)
            {
                Muzzles[i].OnSetColorSector(_gameManager.Settings.colorAreaAttackViewed);
                if (target == null) {
                    Muzzles[i].SetIsShot(false);
                }
            }
            // OnSetColorSector(_gameManager.Settings.colorAreaAttackViewed);
        }

    }

    /// <summary>
    /// Устанавливает, что машина вышла из зоны поиска врага.
    /// </summary>
    /// <param name="lastTarget">Последняя машина, которая видела текущую машину</param>
    public void OnNotViewTarget(BaseMachine lastTarget)
    {
        if (_gameManager.Settings.drawAreaForBot || (lastTarget != null && !lastTarget.MachineLevelData.isBot) || !Machine.MachineLevelData.isBot)
        {
            for (int i = 0; i < Muzzles.Count; i++)
            {
                Muzzles[i].OnSetColorSector(_gameManager.Settings.colorAreaAttackDefault);
                
                if (lastTarget) {
                    Muzzles[i].SetIsShot(false);
                }
            }
            // OnSetColorSector(_gameManager.Settings.colorAreaAttackDefault);

            if (lastTarget)
            {
                lastTarget.AreaSearch.OnSetColor(_gameManager.Settings.colorAreaAttackDefault);
            }
            else
            {
                Machine.AreaSearch.OnSetColor(_gameManager.Settings.colorAreaAttackDefault);
            }
        }
    }

    // /// <summary>
    // /// Устанавливает, что машина попала в зону атаки врага.
    // /// </summary>
    // public void OnAttackTarget()
    // {
    //     // смотрим дистанцию между машинами и выставляем статус, что можно стрелять,
    //     // если дистанция больше чем расстояние на котором запрещено стрелять
    //     float distance = Vector2.Distance(transform.position, _objectTarget.transform.position) * (1 / _gameManager.Settings.scaleObjects);
    //     Debug.Log($"distance= {distance}, {_gameManager.Settings.distanceDisableAttack}, {distance > _gameManager.Settings.distanceDisableAttack}");

    //     if (distance > _gameManager.Settings.distanceDisableAttack)
    //     {
    //         if (_gameManager.Settings.drawAreaForBot || !ObjectTarget.MachineLevelData.isBot || !Machine.MachineLevelData.isBot)
    //         {
    //             // _objectTarget.areaSearch.OnSetColor(_gameManager.Settings.colorAreaAttackAttack);

    //             // for (int i = 0; i < Towers.Count; i++)
    //             // {
    //             //     Towers[i].OnSetColorSector(MachineLevelData.isBot ? _gameManager.Settings.colorSectorAttack : _gameManager.Settings.colorSectorPlayerAttack);
    //             // }
    //             OnSetColorSector(Machine.MachineLevelData.isBot ? _gameManager.Settings.colorSectorAttack : _gameManager.Settings.colorSectorPlayerAttack);
    //         }
    //         // // если углы поворота башни и угла до цели, попадают в диапазон углов стрельба.
    //         // if (Helpers.IsBetween(-_gameManager.Settings.angleStartShot, _gameManager.Settings.angleStartShot, Mathf.DeltaAngle(Data.angleTower, Data.currentAngleTower)))
    //         // {
    //         //     SetIsShot(true);
    //         // }

    //         // TODO
    //         if (Machine.MachineLevelData.isBot || _gameManager.Settings.autoShot)
    //         {
    //             float distanceRay = ConfigMuzzle.distanceAttack * (1 / _gameManager.Settings.scaleObjects) ; //DistanceAttack;
    //             float offsetRay = Machine.AreaMove.transform.localScale.x;
    //             Vector3 dirTower = transform.forward; // new Vector3(Mathf.Cos(Data.currentAngleTower * Mathf.Deg2Rad), Mathf.Sin(Data.currentAngleTower * Mathf.Deg2Rad));
    //             Vector3 startRay = transform.position + offsetRay * dirTower;
    //             RaycastHit hit; // = Physics.Raycast(startRay, dirTower, distanceRay, 1 << 7);

    //             // string str = "";
    //             // for (int i = 0; i < hits.Length; i++)
    //             // {
    //             //     var hit = hits[i];
    //             // if (hit && !hit.collider.CompareTag("TilemapWithCollider")) //  && dirTower != Vector3.zero
    //             // {
    //             if (Physics.Raycast(startRay, dirTower, out hit, distanceAttack, ~(ignoreMask)))
    //             {
    //                 Debug.Log($"hit {hit.collider}, {startRay}, {Data.directionTower}, {Option.Config.distanceSearch}");


    //                 float dist = Vector3.Distance(hit.collider.transform.position, transform.position);
    //                 // str += hit.collider.gameObject.name;
    //                 // str += " dist=" + dist + "(" + distanceRay + ")";
    //                 // if (dist <= distanceRay)
    //                 // {
    //                 BaseMachine bm = hit.collider.GetComponentInParent<BaseMachine>();
    //                 AreaMove amove = hit.collider.GetComponent<AreaMove>();

    //                 if (bm && amove && bm != this)
    //                 {
    //                     // OnSetTarget(bm);
    //                     // if (stateController)
    //                     // {
    //                     //     stateController.ChangeState(stateController.chaseState);
    //                     //     stateController.chaseState.OnSetEnemy(bm);
    //                     // }
    //                     SetIsShot(true);
    //                     Debug.DrawRay(startRay, dirTower * distanceRay, Color.yellow);
    //                 }
    //                 else
    //                 {
    //                     SetIsShot(false);
    //                     Debug.DrawRay(startRay, dirTower * distanceRay, Color.black);
    //                 }
    //             }
    //             else
    //             {
    //                 SetIsShot(false);
    //                 Debug.DrawRay(startRay, dirTower * distanceRay, Color.magenta);
    //             }
    //             // Debug.Log($"collision {hit}({hit.point}): {str}");
    //             // else
    //             // {
    //             //     Debug.DrawRay(startRay, dirTower * distanceRay, Color.magenta);
    //             //     // OnSetTarget(null);
    //             // }
                
    //         }
    //     }
    // }


    public void OnShot()
    {
        for (int j = 0; j < Muzzles.Count; j++)
        {
            Muzzles.ElementAt(j).OnShot();
        }
    }

    // IEnumerator Follow()
    // {
    //     for (; ; ) //while(true)
    //     {
    //         if (ObjectTarget)
    //         {
    //             // Если башня вращается, начинаем поворот в сторону врага.
    //             if (Option.isRotate)
    //             {
    //                 var direction = ObjectTarget.transform.position - transform.position;

    //                 // // your actual heading as upwards parameter
    //                 // Quaternion lookRotationTower = Quaternion.LookRotation(Vector3.forward, directionVectorTower);
    //                 float angleInRadians = Mathf.Atan2(direction.y, direction.x);

    //                 if (Machine.MachineLevelData.isBot || _gameManager.Settings.autoTakeEnemy)
    //                 {
    //                     OnSetAngleTower(angleInRadians * Mathf.Rad2Deg);
    //                 }
    //             }
    //             // если башня не вращается - указываем ей угол поворота = углу поворота базы машины (мгновенно).
    //             else
    //             {
    //                 OnSetAngleTower(Parent == null ? Machine.Data.angleBody : parent.Data.currentAngleTower, Option.isRotate);
    //             }

    //             // считаем дистанцию до врага, если дистанция меньше или равна указанной в параметрах - атакуем врага.
    //             float dist = Vector3.Distance(ObjectTarget.transform.position, transform.position);
    //             if (dist <= DistanceAttack)
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
    //                 OnViewTarget(ObjectTarget);
    //                 // OnSetTarget(null);
    //             }
    //         }
    //         else
    //         {
    //             if (_gameManager.Settings.rotateTowerByBody || !Option.isRotate)
    //             {
    //                 OnSetAngleTower(Parent == null ? Machine.Data.angleBody : parent.Data.currentAngleTower, Option.isRotate);
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

    /// <summary>
    /// Функция расчета ХР для башни машины.
    /// </summary>
    /// <returns>кол-во всех вокселей и кол-во разрушенных, ХР - от 0 до 1</returns>
   public ContainerData RefreshHP()
    {
        var result = new ContainerData();

        result.countVoxels += voxelMeshRender.Config.sOVoxelData.countVoxels;

        if (voxelMeshRender.Containers != null)
        {
            for (int i = 0; i < voxelMeshRender.Containers.Length; i++)
            {
                result.countVoxelsDestructible += voxelMeshRender.Containers[i].ContainerData.countVoxelsDestructible;
                // result.countVoxels += voxelMeshRender.Containers[i].ContainerData.countVoxels;
                // Debug.Log($"_containerData: {voxelMeshRender.Containers[i].ContainerData.countVoxels}/{voxelMeshRender.Containers[i].ContainerData.countVoxelsDestructible}");
            }
        }

        result.levelDestruction = (float)result.countVoxelsDestructible / result.countVoxels;

        _data.containerData = result;

        return result;
    }

    public void PreDestroy()
    {
        if (_objectTarget)
        {
            for (int i = 0; i < _objectTarget.Towers.Count; i++)
            {
                _objectTarget.Towers[i].OnSetTarget(null);
            }

            OnSetTarget(null);
        }
    }

    public void OnPointer(Vector3 _pointPointer)
    {
        for (int i = 0; i < voxelMeshRender.Containers.Length; i++)
        {
            Vector3 localPoint = voxelMeshRender.Containers[i].transform.InverseTransformPoint(_pointPointer);

            if (voxelMeshRender.Containers[i].PointInCollider(_pointPointer))
            {
                // voxelMeshRender.Containers[i].ExposionVoxels(ktoStrelyal, localPoint, isDrawMesh, explodeGameObject, damageRadius, direction, normal).Forget();
                Vector3Int pos = Helpers.RoundVector3(localPoint);

                Voxel voxel = voxelMeshRender.Containers[i].GetVoxelMinDistance(pos);
                
                Debug.Log($"<color=purple>Tower OnPointer: {_pointPointer}:::{localPoint}:::{pos}|||{voxel.type}-{voxel.ID}-{voxel.color}</color>");
            }
        }
    }

    public void OnCollision(BaseMachine ktoStrelyal, Vector3 _pointCollision, bool isDrawMesh, GameObject explodeGameObject, int damageRadius, Vector3 direction, Vector3 normal)
    {
        List<UniTask> tasks = new List<UniTask>();
        for (int i = 0; i < voxelMeshRender.Containers.Length; i++)
        {
            var el = voxelMeshRender.Containers[i];
            if (el.IsDestructible())
            {
                Vector3 localPoint = el.transform.InverseTransformPoint(_pointCollision);
                if (el.PointInCollider(_pointCollision))
                {
                    Debug.Log($"<color=blue>Tower OnCollision: {_pointCollision} / {el}</color>");
                    tasks.Add(el.ExposionVoxels(Machine, localPoint, isDrawMesh, explodeGameObject, damageRadius, direction, normal));
                }
            }
        }
        UniTask.WhenAll(tasks).Forget();

        for (int j = 0; j < Muzzles.Count; j++)
        {
            Muzzles[j].OnCollision(ktoStrelyal, _pointCollision, isDrawMesh, explodeGameObject, damageRadius, direction, normal);
        }
    }

    public void ChangePosition(BaseMachine baseMachine)
    {
        if (!baseMachine.Body)
        {
            return;
        }
        // // if (Parent != null) {
        // //     return;
        // // }
        // // x' = (x - x₀) * cos(α) - (y - y₀) * sin(α) + x₀
        // // y' = (x - x₀) * sin(α) + (y - y₀) * cos(α) + y₀
        // // Где:
        // // (x, y) - исходные координаты точки;
        // // (x₀, y₀) - координаты центра поворота;
        // // α - угол поворота в радианах;
        // // (x', y') - новые координаты точки после поворота.
        // var angle = Parent == null ? baseMachine.Body.transform.rotation.eulerAngles.y : Parent.transform.rotation.eulerAngles.y; // parent.Data.currentAngleTower;
        // // var offsetParentTower = Vector2.zero; // Parent == null ? Vector2.zero : parent.Option.offsetTower;
        var point = Option.offsetTower;
        // // var x1 = (point.x - offsetParentTower.x) * Mathf.Cos(angle * Mathf.Deg2Rad) - (point.z - offsetParentTower.y) * Mathf.Sin(angle * Mathf.Deg2Rad) + offsetParentTower.x;
        // // var z1 = (point.x - offsetParentTower.x) * Mathf.Sin(angle * Mathf.Deg2Rad) + (point.z - offsetParentTower.y) * Mathf.Cos(angle * Mathf.Deg2Rad) + offsetParentTower.y;
        // var x1 = point.x + Mathf.Cos(angle) * 1;
        // var z1 = point.z + Mathf.Sin(angle) * 1;
        // var n = baseMachine.Body.transform.TransformPoint(point);
        transform.position = Parent == null ? baseMachine.Body.transform.TransformPoint(point) : Parent.transform.TransformPoint(point);
    }

    public void OnDamageEffect(float v)
    {
        if (Option.isRotate)
        {
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y + UnityEngine.Random.Range(-v * _gameManager.Settings.koofChangeAngleTower, v * _gameManager.Settings.koofChangeAngleTower), 0);
        }
    }

    public void OnSetParent(BaseTower tow)
    {
        parent = tow;
    }

    
    public void ReDraw(List<ColorsModify> colors)
    {
        voxelMeshRender.UploadedAllMeshes(colors);
    }

    public FillData OnFill(Vector3 _pointPointer)
    {
        FillData output = new FillData();

        for (int i = 0; i < voxelMeshRender.Containers.Length; i++)
        {
            if (voxelMeshRender.Containers[i].PointInCollider(_pointPointer))
            {
                Vector3 localPoint = voxelMeshRender.Containers[i].transform.InverseTransformPoint(_pointPointer);
                // voxelMeshRender.Containers[i].ExposionVoxels(ktoStrelyal, localPoint, isDrawMesh, explodeGameObject, damageRadius, direction, normal).Forget();
                Vector3Int pos = Helpers.RoundVector3(localPoint);

                Voxel voxel = voxelMeshRender.Containers[i].GetVoxelMinDistance(pos);

                if (!voxel.color.Equals(Color.clear))
                {
                    SubmeshesData submeshesData = Option.Config.MeshConfig.sOVoxelData.GetVoxelGroup(voxel.position);

                    Color32 groupColor32 = submeshesData.color;

                    output.voxelGroupData = submeshesData;
                
                    Debug.Log($"<color=purple>Body[{Machine.MachineLevelData.id}] OnPointer: {_pointPointer}:::{localPoint}:::{pos}|||{groupColor32}-{voxel.color}</color>");
                }
            }
        }
        return output;
    }
    
#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Color color = _gameManager ? _gameManager.Settings.DebugSettings.gizmoTowersColor : Color.yellow;
        bool isDraw = _gameManager ? _gameManager.Settings.DebugSettings.gizmoTowersForwards : true;
        float length = _gameManager ? _gameManager.Settings.DebugSettings.gizmoTowersLength : 30;
        if (isDraw)
        {
            Gizmos.color = color;
            Gizmos.DrawRay(transform.position, transform.forward * length);
            // Gizmos.DrawGUITexture(new Rect(new Vector3(0,0,0), new Vector2(10,2)));
            Vector3 textPosition = transform.position + transform.forward * 1;
            Handles.Label(textPosition, "tower forward");
        }
    }
#endif
}
