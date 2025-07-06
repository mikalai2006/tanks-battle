using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class BaseTower : MonoBehaviour
{
    protected GameManager _gameManager = GameManager.Instance;
    [SerializeField] private SpriteRenderer _sprite;
    [SerializeField] private Image _spriteSector;
    [SerializeField] private RectTransform _rectSector;
    [SerializeField] private SpriteRenderer _damageSprite;
    [SerializeField] GameObject MuzzlesBox;
    [SerializeField] GameObject Wrapper;
    [SerializeField] List<BaseMuzzle> muzzles;
    public List<BaseMuzzle> Muzzles => muzzles;
    [SerializeField] private SortingGroup sortingGroup;
    GameTowerOption Option;
    protected BaseMachine Machine;
    [SerializeField] private BaseMachine _objectTarget;
    public BaseMachine ObjectTarget => _objectTarget;
    protected float distanceAttack;
    public float DistanceAttack => distanceAttack;
    [SerializeField] protected DataTower _data = new();
    public DataTower Data => _data;
    protected BaseTower parent;
    public BaseTower Parent => parent;

    void Start()
    {
        // StartCoroutine(Follow());
    }

    public void Init(BaseMachine baseMachine, GameTowerOption optConfig, int index)
    {
        Option = optConfig;

        Machine = baseMachine;

        OnSetSpeedRotateTower(optConfig.Config.speedRotateTower);
        OnSetAngleTower(0);

        sortingGroup.sortingOrder = index;

        OnChangeData();

        _sprite.color = Option.colorTower;

        _damageSprite.size = _sprite.size;

        distanceAttack = Option.Config.distanceAttack;

        OnSetSizeSector(distanceAttack);

        _sprite.sprite = Option.Config.spriteTower;

        transform.localPosition = new Vector3(Option.offsetTower.x, Option.offsetTower.y);


        // инициализируем компоненты машины
        for (int i = 0; i < optConfig.muzzles.Count; i++)
        {
            GameMuzzleOption _mConfig = optConfig.muzzles.ElementAt(i);
            var _muz = Instantiate(_mConfig.Config.prefab, MuzzlesBox.transform);
            _muz.Init(baseMachine, this, _mConfig, i);
            muzzles.Add(_muz);
        }

        OnNotViewTarget(null);

        ChangePosition(baseMachine);
    }

    public void OnChangeData()
    {
        Color col = Color.white;
        col.a = 1f - Mathf.Min(1f, Machine.Data.hp * 100f / Machine.Config.hp * 0.01f);

        _damageSprite.color = col;
    }

    public void OnSetAngleSector(float angle)
    {
        // fillAmount: 1 - 360град.
        // fillAmount: x - angle
        // fillAmount: x = 1 * angle / 360.
        // rectTransform = -(fillAmount * 360) / 2
        var fillAmount = angle / 360;
        _spriteSector.fillAmount = fillAmount;
        _rectSector.localEulerAngles = new Vector3(_rectSector.localEulerAngles.x, _rectSector.localEulerAngles.y, -fillAmount * 360 / 2);
    }
    
    public void OnSetSpeedRotateTower(float rotateTower)
    {
        _data.speedRotateTower = rotateTower;
    }

    public void OnSetColorSector(Color color)
    {
        _spriteSector.color = color;
    }

    public void OnSetSizeSector(float size)
    {
        _rectSector.sizeDelta = Vector2.Lerp(_rectSector.sizeDelta, new Vector2(size * 2, size * 2), _gameManager.Settings.speedChangeAreaSize * Time.deltaTime);

        // _rectSector.sizeDelta = new Vector2(size, size);
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
    /// <param name="angle">угол</param>
    public void OnSetAngleTower(float angle, bool bySpeed = true)
    {
        _data.angleTower = angle;

        DataBonus bonusSpeedTower = null;
        Machine.Data.bonuses.TryGetValue(TypeBonus.SpeedTower, out bonusSpeedTower);
        //Tower.transform.rotation = Quaternion.Euler(0, 0, angle);
       transform.rotation = Quaternion.Lerp(
            transform.rotation,
            Quaternion.Euler(0, 0, angle),
            bySpeed ? (_data.speedRotateTower + (bonusSpeedTower != null ? bonusSpeedTower.value : 0)) * Time.deltaTime : 1
        );

        OnSetDirectionTower(angle);

        // устанавливаем угол - разницу углов поворота башни и базы
        // Data.angleTowerByBody = Body.transform.localEulerAngles.z - Tower.transform.localEulerAngles.z;
        // Muzzle.transform.rotation = Quaternion.Lerp(Muzzle.transform.rotation, Quaternion.Euler(0, 0, lookRotationTower.eulerAngles.z + 90), .05f);
    }
    

    public void OnSetDirectionTower(float angle)
    {
        _data.directionTower = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
    }

    public void SetIsShot(bool status)
    {
        _data.isShot = status;

        if (!status)
        {
            // for (int i = 0; i < muzzles.Count; i++)
            // {
            //     Muzzles[i].OnStopShot();
            // }
        }
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

            // for (int i = 0; i < Towers.Count; i++)
            // {
            //     Towers[i].OnSetColorSector(_gameManager.Settings.colorAreaAttackViewed);
            // }
            OnSetColorSector(_gameManager.Settings.colorAreaAttackViewed);
        }

        SetIsShot(false);
    }

    /// <summary>
    /// Устанавливает, что машина вышла из зоны поиска врага.
    /// </summary>
    /// <param name="lastTarget">Последняя машина, которая видела текущую машину</param>
    public void OnNotViewTarget(BaseMachine lastTarget)
    {
        if (_gameManager.Settings.drawAreaForBot || (lastTarget != null && !lastTarget.MachineLevelData.isBot) || !Machine.MachineLevelData.isBot)
        {
            // for (int i = 0; i < Towers.Count; i++)
            // {
            //     Towers[i].OnSetColorSector(_gameManager.Settings.colorAreaAttackDefault);
            // }
            OnSetColorSector(_gameManager.Settings.colorAreaAttackDefault);

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
            if (_gameManager.Settings.drawAreaForBot || !ObjectTarget.MachineLevelData.isBot || !Machine.MachineLevelData.isBot)
            {
                // _objectTarget.areaSearch.OnSetColor(_gameManager.Settings.colorAreaAttackAttack);

                // for (int i = 0; i < Towers.Count; i++)
                // {
                //     Towers[i].OnSetColorSector(MachineLevelData.isBot ? _gameManager.Settings.colorSectorAttack : _gameManager.Settings.colorSectorPlayerAttack);
                // }
                OnSetColorSector(Machine.MachineLevelData.isBot ? _gameManager.Settings.colorSectorAttack : _gameManager.Settings.colorSectorPlayerAttack);
            }
            // // если углы поворота башни и угла до цели, попадают в диапазон углов стрельба.
            // if (Helpers.IsBetween(-_gameManager.Settings.angleStartShot, _gameManager.Settings.angleStartShot, Mathf.DeltaAngle(Data.angleTower, Data.currentAngleTower)))
            // {
            //     SetIsShot(true);
            // }

            // TODO
            if (Machine.MachineLevelData.isBot || _gameManager.Settings.autoShot)
            {
                float distanceRay = DistanceAttack;
                float offsetRay = Machine.AreaMove.transform.localScale.x;
                Vector3 dirTower = new Vector2(Mathf.Cos(Data.currentAngleTower * Mathf.Deg2Rad), Mathf.Sin(Data.currentAngleTower * Mathf.Deg2Rad));
                Vector3 startRay = transform.position + offsetRay * dirTower;
                RaycastHit2D hit = Physics2D.Raycast(startRay, dirTower, distanceRay, 1 << 7);

                // string str = "";
                // for (int i = 0; i < hits.Length; i++)
                // {
                //     var hit = hits[i];
                if (hit && !hit.collider.CompareTag("TilemapWithCollider")) //  && dirTower != Vector3.zero
                {
                    // Debug.Log($"hit {hit.collider}, {startRay}, {Data.directionTower}, {Config.distanceSearch}");


                    float dist = Vector3.Distance(hit.collider.transform.position, transform.position);
                    // str += hit.collider.gameObject.name;
                    // str += " dist=" + dist + "(" + distanceRay + ")";
                    // if (dist <= distanceRay)
                    // {
                    BaseMachine bm = hit.collider.GetComponentInParent<BaseMachine>();
                    AreaMove amove = hit.collider.GetComponent<AreaMove>();

                    if (bm && amove && bm != this)
                    {
                        // OnSetTarget(bm);
                        // if (stateController)
                        // {
                        //     stateController.ChangeState(stateController.chaseState);
                        //     stateController.chaseState.OnSetEnemy(bm);
                        // }
                        SetIsShot(true);
                        Debug.DrawRay(startRay, dirTower * distanceRay, Color.yellow);
                    }
                    else
                    {
                        SetIsShot(false);
                        Debug.DrawRay(startRay, dirTower * distanceRay, Color.magenta);
                    }
                }
                else
                {
                    SetIsShot(false);
                    Debug.DrawRay(startRay, dirTower * distanceRay, Color.magenta);
                }
                // Debug.Log($"collision {hit}({hit.point}): {str}");
                // else
                // {
                //     Debug.DrawRay(startRay, dirTower * distanceRay, Color.magenta);
                //     // OnSetTarget(null);
                // }
            }
        }
    }


    void Update()
    {
        // Записываем в данные угол поворота башни.
        if (Data.angleTower != Data.currentAngleTower)
        {
            OnSetCurrentAngleTower(transform.localEulerAngles.z);
        }

        // проверяем наличие бонуса дистанции атаки.
        DataBonus bonusDistanceAttack = null;
        Machine.Data.bonuses.TryGetValue(TypeBonus.DistanceAttack, out bonusDistanceAttack);
        if (bonusDistanceAttack != null)
        {
            distanceAttack = Option.Config.distanceAttack + bonusDistanceAttack.value;
            OnSetSizeSector(distanceAttack);
        }
        else
        {
            distanceAttack = Option.Config.distanceAttack;
            OnSetSizeSector(distanceAttack);
        }


        // изменяем угол сектора и его размер.
        if (ObjectTarget != null && (!Machine.MachineLevelData.isBot || !ObjectTarget.MachineLevelData.isBot))
        {
            OnSetAngleSector(Mathf.Max(5, Mathf.Abs(Mathf.DeltaAngle(Data.angleTower, Data.currentAngleTower))));
        }


        if (_gameManager.Settings.autoTakeEnemy || Machine.MachineLevelData.isBot)
        {
            // если есть возможные цели
            List<BaseMachine> _vacantTargets = Machine.AreaSearch.Targets
                .Where(t => t.Value >= Machine.Data.timeBeforeAddTarget || !Machine.MachineLevelData.isBot)
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
                    if (minDistance == 0 || (minDistance > dist && _gameManager.Settings.takeNearEnemy))
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
        }
        else
        {
            if (!Machine.MachineLevelData.isBot)
            {
                float distanceRay = DistanceAttack;
                float offsetRay = Machine.AreaMove.transform.localScale.x;
                Vector3 dirTower = new Vector2(Mathf.Cos(Data.currentAngleTower * Mathf.Deg2Rad), Mathf.Sin(Data.currentAngleTower * Mathf.Deg2Rad));
                Vector3 startRay = transform.position + offsetRay * dirTower;
                RaycastHit2D hit = Physics2D.Raycast(startRay, dirTower, distanceRay, 1 << 7);

                // string str = "";
                // for (int i = 0; i < hits.Length; i++)
                // {
                //     var hit = hits[i];
                if (hit && !hit.collider.CompareTag("TilemapWithCollider") && dirTower != Vector3.zero)
                {
                    // Debug.Log($"hit {hit.collider}, {startRay}, {Data.directionTower}, {Config.distanceSearch}");


                    float dist = Vector3.Distance(hit.collider.transform.position, transform.position);
                    // str += hit.collider.gameObject.name;
                    // str += " dist=" + dist + "(" + distanceRay + ")";
                    // if (dist <= distanceRay)
                    // {
                    BaseMachine bm = hit.collider.GetComponentInParent<BaseMachine>();
                    AreaMove amove = hit.collider.GetComponent<AreaMove>();

                    if (bm && amove && bm != this && bm != ObjectTarget)
                    {
                        // OnSetTarget(bm);
                        // if (stateController)
                        // {
                        //     stateController.ChangeState(stateController.chaseState);
                        //     stateController.chaseState.OnSetEnemy(bm);
                        // }
                        OnSetTarget(bm);
                        Debug.DrawRay(startRay, dirTower * distanceRay, Color.yellow);
                    }
                }
                // else
                // {
                //     OnSetTarget(null);
                //     Debug.DrawRay(startRay, dirTower * distanceRay, Color.magenta);
                // }
                // Debug.Log($"collision {str}");
            }
        }

        // если нет конечного автомата, проверяем есть ли противники в зоне досягаемости
        // и если нет - убираем цель
        if (Machine.AreaSearch.Targets.Count == 0)
        {
            OnSetTarget(null);
        }

        // если нет цели, отменяем выстрелы
        if (ObjectTarget == null)
        {
            SetIsShot(false);
        }

        // синхронизируем позицию башни
        ChangePosition(Machine);

        // Отслеживание противников.
        if (ObjectTarget)
            {
                // Если башня вращается, начинаем поворот в сторону врага.
                if (Option.isRotate)
                {
                    var direction = ObjectTarget.transform.position - transform.position;

                    // // your actual heading as upwards parameter
                    // Quaternion lookRotationTower = Quaternion.LookRotation(Vector3.forward, directionVectorTower);
                    float angleInRadians = Mathf.Atan2(direction.y, direction.x);

                    if (Machine.MachineLevelData.isBot || _gameManager.Settings.autoTakeEnemy)
                    {
                        OnSetAngleTower(angleInRadians * Mathf.Rad2Deg);
                    }
                }
                // если башня не вращается - указываем ей угол поворота = углу поворота базы машины (мгновенно).
                else
                {
                    OnSetAngleTower(Parent == null ? Machine.Data.angleBody : parent.Data.currentAngleTower, Option.isRotate);
                }
                
                // считаем дистанцию до врага, если дистанция меньше или равна указанной в параметрах - атакуем врага.
                float dist = Vector3.Distance(ObjectTarget.transform.position, transform.position);
                if (dist <= DistanceAttack)
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
                    OnViewTarget(ObjectTarget);
                    // OnSetTarget(null);
                }
            }
            else
            {
                if (_gameManager.Settings.rotateTowerByBody || !Option.isRotate)
                {
                    OnSetAngleTower(Parent == null ? Machine.Data.angleBody : parent.Data.currentAngleTower, Option.isRotate);
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

    public void ChangePosition(BaseMachine baseMachine)
    {
        // if (Parent != null) {
        //     return;
        // }
        // x' = (x - x₀) * cos(α) - (y - y₀) * sin(α) + x₀
        // y' = (x - x₀) * sin(α) + (y - y₀) * cos(α) + y₀
        // Где:
        // (x, y) - исходные координаты точки;
        // (x₀, y₀) - координаты центра поворота;
        // α - угол поворота в радианах;
        // (x', y') - новые координаты точки после поворота.
        var angle = Parent == null ? baseMachine.Data.angleBody : parent.Data.currentAngleTower;
        var offsetParentTower = Vector2.zero; // Parent == null ? Vector2.zero : parent.Option.offsetTower;
        var point = Option.offsetTower;
        var x1 = (point.x - offsetParentTower.x) * Mathf.Cos(angle * Mathf.Deg2Rad) - (-point.y - offsetParentTower.y) * Mathf.Sin(angle * Mathf.Deg2Rad) + offsetParentTower.x;
        var y1 = (point.x - offsetParentTower.x) * Mathf.Sin(angle * Mathf.Deg2Rad) + (-point.y - offsetParentTower.y) * Mathf.Cos(angle * Mathf.Deg2Rad) + offsetParentTower.y;
        transform.localPosition = new Vector3(x1,y1);
    }

    public void OnDamageEffect(float v)
    {
        if (Option.isRotate)
        {
            transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z + UnityEngine.Random.Range(-v * _gameManager.Settings.koofChangeAngleTower, v * _gameManager.Settings.koofChangeAngleTower));
        }
    }

    public void OnSetParent(BaseTower tow)
    {
        parent = tow;
    }
}
