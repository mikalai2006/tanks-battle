using UnityEngine;
using Cysharp.Threading.Tasks;
using Mikalai2006.Voxel;
using System.Collections.Generic;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor; 
#endif

public abstract class BaseMuzzle : MonoBehaviour, IColored
{
    protected GameManager _gameManager => GameManager.Instance;
    // [SerializeField] private Animator _animator;
    protected BaseMachine Machine;
    [SerializeField] protected GameObject Wrapper;
    [SerializeField] GameObject SectorGO;
    [SerializeField] private Image _spriteSector;
    [SerializeField] private RectTransform _rectSector;
    [SerializeField] protected GameObject pivot;
    [SerializeField] protected GameObject pointEffects;
    public GameObject PointEffects => pointEffects;
    [SerializeField] protected GameMuzzleOption Option;
    [SerializeField] protected GameMuzzle Config => Option.Config;
    // [SerializeField] protected SpriteRenderer sprite;
    // protected ParticleSystem[] particlesBoom;
    [SerializeField] protected DataMuzzle _data;
    public DataMuzzle Data => _data;
    [SerializeField] protected BaseTower Tower;
    [SerializeField] protected GameObject MaxDistanceObject;
    public GameObject trajectoryGO;
    public TrajectoryRenderer trajectoryRenderer;
    public GameObject decal;
    [SerializeField] protected VoxelMeshRender voxelMeshRender;
    [SerializeField] private LayerMask ignoreMask;
    [SerializeField] protected Light SpotLight;
    [Tooltip("Задержка выстрела ствола (для орудия с несколькими стволами, чтобы создать эффект последовательности)")]
    protected float delayTime = 0.2f;
    [SerializeField] protected float Delay;
    [SerializeField] protected bool isBusy;
    public bool IsBusy => isBusy;
    protected float distanceAttack;
    protected Vector3 targetPosition;
    protected System.Threading.CancellationTokenSource cancelToken;

    #region Unity methods
    void Awake()
    {
        _data = new();
        cancelToken = new System.Threading.CancellationTokenSource();
    }

    void OnDestroy()
    {
        cancelToken.Cancel();
        cancelToken.Dispose();
    }

    public virtual void FixedUpdate()
    {
        if (!Machine)
        {
            return;
        }

        transform.rotation = Quaternion.Euler(transform.localEulerAngles.x, Tower.transform.eulerAngles.y, Tower.transform.eulerAngles.z);

        // if (_data.timeBeforeShot <= Delay && isBusy)
        // {
        //     isBusy = false;
        // }
        
        // считаем время до выстрела.
        if (_data.timeBeforeShot > 0)
        {
            OnSetTimeBetweenShot(Mathf.Max(_data.timeBeforeShot - Time.deltaTime, 0));
        }
        // else
        // {
        //     // if (_data.index == Tower.Muzzles.Count - 1)
        //     // {
        //     //     Tower.SetBusy(false);
        //     // }
        //     SetBusy(false);
        // }

        // выстрел для бота.
        if (!isBusy && Machine.MachineLevelData.isBot && _data.isShot) // !Tower.IsBusy()
        {
            OnShot();
        }

        // // обновляем время до выстрела
        // if (isBusy || Data.isShot)
        // {
        //     if (_data.timeBeforeShot > 0)
        //     {
        //         OnSetTimeBetweenShot(_data.timeBeforeShot - Time.deltaTime);
        //     }
        // } else
        // {
        //     if (_data.timeBeforeShot > Delay)
        //     {
        //         OnSetTimeBetweenShot(_data.timeBeforeShot - Time.deltaTime);
        //     }
        // }

        // // произвести выстрел, если время истекло
        // if (_data.isShot && _data.timeBeforeShot <= 0)
        // {
        //     OnGoShot();
        // }


        // // TODO: сделать берется бонус - добавляется, ставим таймер и по завершении удаляем бонус
        // // устанавливаем дистанцию атаки.
        // // проверяем наличие бонуса дистанции атаки.
        // DataBonus bonusDistanceAttack = null;
        // Machine.Data.bonuses.TryGetValue(TypeBonus.DistanceAttack, out bonusDistanceAttack);
        // if (bonusDistanceAttack != null)
        // {
        //     distanceAttack = Config.distanceAttack + bonusDistanceAttack.value;//* (1 / Machine.Wrapper.transform.localScale.x);
        //     OnSetSizeSector(distanceAttack);
        // }
        // else
        // {
        //     distanceAttack = Data.distanceAttack;
        //     OnSetSizeSector(distanceAttack);
        // }

        // Отслеживание противников.
        if (Tower.ObjectTarget)
        {
            // считаем дистанцию до врага, если дистанция меньше или равна указанной в параметрах - атакуем врага.
            float dist = Vector3.Distance(Tower.ObjectTarget.transform.position, transform.position);
            
            // if (Machine.MachineLevelData.isBot)
            // {
            //     Debug.Log($"dist={dist}, distanceAttack={distanceAttack}");
            // }
            
            if (dist <= distanceAttack)
            {
                OnAttackTarget();
                // // SetIsShot(true);
                // if (Application.isEditor)
                // {
                //     Badge.OnSetNameText(dist.ToString());
                // }
            }
        }
        
        // изменяем угол сектора и его размер.
        if (Tower.ObjectTarget != null && (!Machine.MachineLevelData.isBot || !Tower.ObjectTarget.MachineLevelData.isBot))
        {
            OnSetAngleSector(Mathf.Max(5, Mathf.Abs(Mathf.DeltaAngle(Tower.Data.angleTower, Tower.Data.currentAngleTower))));
        }
    }
    #endregion

    public void Init(BaseMachine _machine, BaseTower tower, GameMuzzleOption option, int index, DataDetail dataMuzzle)
    {
        Option = option;

        Tower = tower;

        Machine = _machine;
        
        Data.timeBetweenShot = Config.timeBetweenShot;
        Data.distanceAttack = Config.distanceAttack;// * (1 / Machine.Wrapper.transform.localScale.x);
        Data.speedBullet = Option.Config.speedBullet;

        if (Tower.Parent == null && _gameManager.LevelConfig != null )
        {
            if (_gameManager.LevelConfig.light < 1)
            {
                SpotLight.enabled = !Machine.MachineLevelData.isBot;
            } else
            {
                SpotLight.enabled = false;
            } 
        } else
        {
            SpotLight.gameObject.SetActive(false);
        }

        _data.index = index;

        // MeshConfig meshConfig = Config.MeshConfig;
        // meshConfig.sOVoxelData.Pivot = new Vector3(meshConfig.sOVoxelData.Pivot.x,meshConfig.sOVoxelData.Pivot.y,1);

        voxelMeshRender.OnSetConfigMeshGenerator(Config.MeshConfig);

        if (Machine.MachineLevelData != null && Machine.MachineLevelData.data != null)
        {
            voxelMeshRender.SetData(Machine.MachineLevelData.data, dataMuzzle);
        }
        // sprite.color = Config.color;
        // particlesBoom = particlesBoomGameObject.GetComponentsInChildren<ParticleSystem>();

        Delay = _data.index * delayTime;
        
        OnSetTimeBetweenShot(Delay);
        // OnStopShot();

        transform.localPosition = Option.offsetMuzzle;

        pointEffects.transform.localPosition = new Vector3(Config.MeshConfig.sOVoxelData.Bounds.x + _gameManager.Settings.DebugSettings.muzzleOffsetEffectPoint.x, _gameManager.Settings.DebugSettings.muzzleOffsetEffectPoint.y, _gameManager.Settings.DebugSettings.muzzleOffsetEffectPoint.z);

        pivot.transform.localPosition = new Vector3(-Config.MeshConfig.sOVoxelData.Bounds.x, 0, 0);

        MaxDistanceObject.transform.localPosition = new Vector3(Data.distanceAttack * (1 / _gameManager.Settings.scaleObjects), 0, 0);

        // transform.localRotation = Quaternion.Euler(0, 0, 0);
        
        trajectoryGO.SetActive(!Machine.MachineLevelData.isBot);

        SectorGO.transform.localPosition = new Vector3(
            SectorGO.transform.localPosition.x,
            -(transform.position.y * (1 / Machine.Config.customScale)) + 0.05f,
            SectorGO.transform.localPosition.z
        );

        // SectorGO = Instantiate(_gameManager.Settings.sectorVoxel, transform.position, Quaternion.Euler(-90,0,0), Machine.WrapperTools.transform);
        // _rectSector = SectorGO.GetComponentInChildren<RectTransform>();
        // _spriteSector = SectorGO.GetComponentInChildren<Image>();

        distanceAttack = Data.distanceAttack;
        OnSetSizeSector(distanceAttack);
    }


    public void SetBusy(bool status)
    {
        isBusy = status;
    }

    /// <summary>
    /// Инициируем выстрел
    /// </summary>
    /// <param name="target"></param>
    public void OnShot()
    {
        // if (Tower.IsBusy())
        if (IsBusy)
        {
            return;
        }

        SetBusy(true);

        // SetIsShot(true);
        OnGoShot(cancelToken).Forget();
    }

    public void SetIsShot(bool status)
    {
        _data.isShot = status;

        if (!status)
        {
            OnStopShot();
        }
    }

    /// <summary>
    /// Функция остановки стрельбы из дула.
    /// </summary>
    public void OnStopShot()
    {
        // OnResetTimeTimeBetweenShot();
        OnSetTimeBetweenShot(_data.index * (Data.timeBetweenShot / 2));
    }

    public void OnSetColorSector(Color color)
    {
        _spriteSector.color = color;
    }

    public void OnSetAngleSector(float angle)
    {
        // fillAmount: 1 - 360град.
        // fillAmount: x - angle
        // fillAmount: x = 1 * angle / 360.
        // rectTransform = -(fillAmount * 360) / 2
        var fillAmount = angle / 360;
        _spriteSector.fillAmount = fillAmount;
        _rectSector.localEulerAngles = new Vector3(_rectSector.localEulerAngles.x, _rectSector.localEulerAngles.y, -angle / 2);
    }

    public void OnSetSizeSector(float size)
    {
        if (!_rectSector)
        {
            return;
        }

        size = size * (1 / Machine.Wrapper.transform.localScale.z);

        _rectSector.sizeDelta = Vector2.Lerp(_rectSector.sizeDelta, new Vector2(size * 2, size * 2), _gameManager.Settings.speedChangeAreaSize * Time.deltaTime);

        // _rectSector.sizeDelta = new Vector2(size, size);
    }

    public void OnSetAngle(Quaternion rotation, Vector3 point, float speed)
    {
        transform.localRotation = Quaternion.Lerp(
            transform.localRotation,
            Quaternion.Euler(rotation.eulerAngles.x, transform.localEulerAngles.y, transform.localEulerAngles.z),
            speed
        );

        // Vector3 speedForce = transform.forward * 20000 / 30;
        // trajectoryRenderer.ShowTrajectory(pointEffects.transform.position, speedForce);
        if (
            (_gameManager.Settings.playerOptions.showTrajectory && !Machine.MachineLevelData.isBot)
            ||
            (_gameManager.Settings.playerOptions.showOtherTrajectory && Machine.MachineLevelData.isBot)
        )
        {
            if (!trajectoryRenderer.gameObject.activeSelf) {
                trajectoryRenderer.gameObject.SetActive(true);
            }
            trajectoryRenderer.ShowStretchTrajectory(pointEffects.transform.position, point);
        } else
        {
            if (trajectoryRenderer.gameObject.activeSelf) {
                trajectoryRenderer.gameObject.SetActive(false);
            }
        }
    }

    public void OnSetRotation(Vector3 pointCenterScreen, float speedRotate)
    {
        ChangePosition();
        // var offset = Machine.LevelManager.Camera.WorldToScreenPoint(new Vector3(0,0,Option.offsetMuzzle.z));
        // Debug.Log($"forward={transform.forward}");
        // Vector2 screenCenterPoint = new Vector2(Screen.width / 2f - offset.x, Screen.height / 2f);
        // Vector3 centerScreenWithOffsetMuzzle = Machine.LevelManager.Camera.ScreenToWorldPoint(screenCenterPoint);
        // MaxDistanceObject.transform.position = new Vector3(MaxDistanceObject.transform.position.x,pointCenterScreen.y,MaxDistanceObject.transform.position.z);

        // бросаем линию вперед на расстояние атаки.
        RaycastHit hit;
        Vector3 distanceAndDirection = transform.forward * 500f;
        Vector3 endPoint = MaxDistanceObject.transform.position; // pointEffects.transform.position + distanceAndDirection;
        Vector3 castPoint;
        if (Physics.Linecast(pointEffects.transform.position, endPoint, out hit, ~(ignoreMask)))
        {
            _data.pointTarget = hit.point;
            castPoint = pointCenterScreen;
        }
        else
        {
            _data.pointTarget = MaxDistanceObject.transform.position; //transform.position + distanceAndDirection;
            castPoint = pointCenterScreen;
        };

        decal.transform.position = _data.pointTarget - distanceAndDirection.normalized * 0.1f;
        decal.transform.rotation = Quaternion.LookRotation(-distanceAndDirection.normalized);

        var directionLook = castPoint - pivot.transform.position;

        Quaternion lookRotation = Quaternion.LookRotation(directionLook, Vector3.up);
        OnSetAngle(lookRotation, _data.pointTarget, speedRotate);

        // Debug.DrawLine(pivot.transform.position, pointCenterScreen, Color.yellow);
        // var offset = Machine.LevelManager.Camera.WorldToScreenPoint(new Vector3(0,0,Option.offsetMuzzle.z));

        // decal.transform.position = pointCenterScreen - pointCenterScreen.normalized * 0.1f;
        // decal.transform.rotation = Quaternion.LookRotation(-pointCenterScreen.normalized);

        // var direction = pointCenterScreen - Option.offsetMuzzle - pivot.transform.position;
        // Quaternion lookRotation = Quaternion.LookRotation(direction);

        // OnSetAngle(lookRotation, pointCenterScreen, speedRotate);
        // SetIsShot(false);
    }


    protected void OnSetTimeBetweenShot(float time)
    {
        _data.timeBeforeShot = time;
        // Badge.OnChangeData(this);
    }


    // protected void OnResetTimeTimeBetweenShot()
    // {
    //     OnSetTimeBetweenShot(Config.timeBetweenShot - (_data.index * Delay)); //  + (_data.index * (Config.timeBetweenShot / 2f))
    // }


    async public virtual UniTask OnGoShot(System.Threading.CancellationTokenSource cancelToken)
    {
        if (!cancelToken.IsCancellationRequested)
        {
            // Machine.OnResetTimeAfterLastShot(this);

            if (!Machine)
            {
                return;
            }

            if (Machine.isVisible)
            {
                _gameManager.audioManager.PlayClipEffect(Config.soundShot);
            }

            await UniTask.Delay(System.TimeSpan.FromSeconds(Delay), cancellationToken: cancelToken.Token);
            // // for (int i = 0; i < particlesBoom.Length; i++)
            // // {
            // //     particlesBoom[i].gameObject.SetActive(true);
            // // }
            // if (_animator)
            // {
            //     _animator.SetTrigger("shot");
            // }

            // // TODO Effect stretch fire muzzle
            // GameObject objEffect = Lean.Pool.LeanPool.Spawn(Config.fireEffect, Machine.LevelManager.objectSpawnEffect.transform, false);
            // objEffect.transform.position = pointEffects.transform.position;

            // ParticleSystem[] particles = objEffect.transform.GetChild(0).GetComponentsInChildren<ParticleSystem>();
            // if (particles.Length > 0)
            // {
            //     for (int i = 0; i < particles.Length; i++)
            //     {
            //         var main = particles[i].main;
            //         var rend = particles[i].GetComponent<ParticleSystemRenderer>();
            //         rend.material = Config.material; //gameObject.GetComponent<MeshRenderer>().material;
            //     }
            // }
            // objEffect.transform.eulerAngles = new Vector3(0, Tower.transform.eulerAngles.z, 0);
            // Lean.Pool.LeanPool.Despawn(objEffect, 2);

            // // OnSetTimeBetweenShot(Config.timeBetweenShot);

            // await UniTask.Delay(System.TimeSpan.FromSeconds(Delay));

            // if (!Machine.MachineLevelData.isBot)
            // {
            //     SetIsShot(false);
            // }
        }
    }

    
    /// <summary>
    /// Устанавливает, что машина попала в зону атаки врага.
    /// </summary>
    public void OnAttackTarget()
    {
        // смотрим дистанцию между машинами и выставляем статус, что можно стрелять,
        // если дистанция больше чем расстояние на котором запрещено стрелять
        float distance = Vector2.Distance(transform.position, Tower.ObjectTarget.transform.position);
// Debug.Log($"distance={distance},_gameManager.Settings.distanceDisableAttack={_gameManager.Settings.distanceDisableAttack}");
        if (distance > _gameManager.Settings.distanceDisableAttack)
        {
            if (_gameManager.Settings.drawAreaForBot || !Tower.ObjectTarget.MachineLevelData.isBot || !Machine.MachineLevelData.isBot)
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
                float distanceRay = distanceAttack;
                float offsetRay = 1; //Machine.AreaMove.transform.localScale.x;
                Vector3 dirTower = transform.forward; // new Vector3(Mathf.Cos(Data.currentAngleTower * Mathf.Deg2Rad), Mathf.Sin(Data.currentAngleTower * Mathf.Deg2Rad));
                Vector3 startRay = transform.position + offsetRay * dirTower;
                RaycastHit hit; // = Physics.Raycast(startRay, dirTower, distanceRay, 1 << 7);

                // string str = "";
                // for (int i = 0; i < hits.Length; i++)
                // {
                //     var hit = hits[i];
                // if (hit && !hit.collider.CompareTag("TilemapWithCollider")) //  && dirTower != Vector3.zero
                // {
                if (Physics.Raycast(startRay, dirTower, out hit, distanceAttack, ~(ignoreMask)))
                {


                    // float dist = Vector3.Distance(hit.collider.transform.position, transform.position);
                    // str += hit.collider.gameObject.name;
                    // str += " dist=" + dist + "(" + distanceRay + ")";
                    // if (dist <= distanceRay)
                    // {
                    BaseMachine bm = hit.collider.GetComponentInParent<BaseMachine>();
                    // AreaMove amove = hit.collider.GetComponentInParent<AreaMove>();

                    // if (!Machine.MachineLevelData.isBot || !Tower.ObjectTarget.MachineLevelData.isBot)
                    // {
                    //     Debug.Log($"hit {hit.collider}, {startRay}, {Tower.Data.directionTower}");
                    //     if (bm)
                    //     {
                            
                    //     Debug.Log($"bm={bm.name},");
                    //     }
                    // }

                    if (bm && bm != this)
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
                        Debug.DrawRay(startRay, dirTower * distanceRay, Color.black);
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

    public void OnCollision(BaseMachine ktoStrelyal, Vector3 _pointCollision, bool isDrawMesh, GameObject explodeGameObject, int damageRadius, Vector3 direction, Vector3 normal)
    {
        List<UniTask> tasks = new List<UniTask>(0);
        for (int i = 0; i < voxelMeshRender.Containers.Length; i++)
        {
            if (voxelMeshRender.Containers[i].IsDestructible())
            {
                Vector3 localPoint = voxelMeshRender.Containers[i].transform.InverseTransformPoint(_pointCollision);
                if (voxelMeshRender.Containers[i].PointInCollider(_pointCollision))
                {
                    Debug.Log($"<color=blue>Muzzle OnCollision: {_pointCollision} / {localPoint}</color>");
                    tasks.Add(voxelMeshRender.Containers[i].ExposionVoxels(ktoStrelyal, localPoint, isDrawMesh, explodeGameObject, damageRadius, direction, normal));
                }
            }
        }
        UniTask.WhenAll(tasks).Forget();
    }

    /// <summary>
    /// Функция расчета ХР для ствола машины.
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

    public void ChangePosition()
    {
        // if (!Tower)
        // {
        //     return;
        // }

        // var point = Option.offsetMuzzle;
        // var position = Tower.transform.TransformPoint(point);
        
        // transform.localPosition = Tower.transform.InverseTransformPoint(position);
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
                    SubmeshesData submeshesData = Config.MeshConfig.sOVoxelData.GetVoxelGroup(voxel.position);

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
        Color color = _gameManager ? _gameManager.Settings.DebugSettings.gizmoMuzzleColor : Color.yellow;
        bool isDraw = _gameManager ? _gameManager.Settings.DebugSettings.gizmoMuzzlesForwards : true;
        float length = _gameManager ? _gameManager.Settings.DebugSettings.gizmoMuzzleLength : 30;
        if (isDraw)
        {
            Gizmos.color = color;
            Gizmos.DrawRay(transform.position, transform.forward * length);
            // Gizmos.DrawGUITexture(new Rect(new Vector3(0,0,0), new Vector2(10,2)));
            Vector3 textPosition = transform.position + transform.forward * 1;
            Handles.Label(textPosition, "muzzle forward");
        }

        if (Application.isPlaying)
        {
            if (_gameManager.Settings.DebugSettings.gizmoTrajectory && Data != null)
            {
                Gizmos.color = _gameManager.Settings.DebugSettings.gizmoTrajectoryColor;
                Gizmos.DrawLine(pointEffects.transform.position, _data.pointTarget);
                Handles.Label(pointEffects.transform.position + transform.forward, "trajectory");
            }

            if (_gameManager.Settings.DebugSettings.gizmoMuzzleDistanseAttack && Data != null)
            {
                // Gizmos.color = _gameManager.Settings.DebugSettings.gizmoMuzzleDistanseAttackColor;
                // Gizmos.DrawSphere(pointEffects.transform.position, Machine.Wrapper.transform.localScale.x * Data.distanceAttack);
                Handles.color = _gameManager.Settings.DebugSettings.gizmoMuzzleDistanseAttackColor;
                var diffAngle = Tower.Data.angleTower - Tower.Data.currentAngleTower;
                Quaternion rotation = Quaternion.Euler(0, -diffAngle / 2, 0); 
                Vector3 rotatedForward = rotation * transform.forward;

                Handles.DrawSolidArc(
                    pointEffects.transform.position,
                    transform.up,
                    rotatedForward,
                    diffAngle, // > 180 ? diffAngle - 360 : diffAngle,
                    distanceAttack
                );
                Handles.DrawWireDisc(
                    pointEffects.transform.position,
                    pointEffects.transform.up,
                    distanceAttack,
                    5f
                );
                Handles.Label(pointEffects.transform.position + transform.forward, diffAngle.ToString());
            }
        }
    }
#endif

    // bool AnimatorIsPlaying(string stateName) {
    //     return _animator.GetCurrentAnimatorStateInfo(0).length > _animator.GetCurrentAnimatorStateInfo(0).normalizedTime
    //         && _animator.GetCurrentAnimatorStateInfo(0).IsName(stateName);
    // }

    //  public void LoadedAsset(AsyncOperationHandle<GameObject> handle)
    // {
    //     if (handle.Status == AsyncOperationStatus.Succeeded)
    //     {
    //         BaseBullet obj = handle.Result.GetComponent<BaseBullet>();
    //         if (obj != null)
    //         {
    //             obj.OnInit(Machine, Tower, this, Config);
    //         }
    //     }
    //     else
    //     {
    //         Debug.LogError($"Error Load prefab::: {handle.Status}");
    //     }
    // }
}
