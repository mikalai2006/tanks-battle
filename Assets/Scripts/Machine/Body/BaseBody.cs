using Cysharp.Threading.Tasks;
using Mikalai2006.Voxel;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor; 
#endif

public class BaseBody : MonoBehaviour, IColored
{
    protected GameManager _gameManager => GameManager.Instance;
    [SerializeField] private SpriteRenderer _bodySprite;
    [SerializeField] private SpriteRenderer _bodyGerbSprite;
    [SerializeField] private SpriteRenderer _damageSprite;
    protected BaseMachine Machine;
    public GameBody Config {get; private set;}
    // public GameBodyOption Option {get; private set;}
    public DataDetail DataDetail {get; private set ;}
    [SerializeField] protected DataBody _data;
    public Vector3 Bounds {get; private set; }
    public DataBody Data => _data;
    [SerializeField] protected bool isMove;
    public bool IsMove => isMove;
    [SerializeField] protected VoxelMeshRender voxelMeshRender;

#region Unity methods
    void Awake()
    {
        _data = new();
    }
    
    // void FixedUpdate()
    // {
    //     // синхронизируем позицию каждой башни
    //     for (int i = 0; i < Machine.Towers.Count; i++)
    //     {
    //         Machine.Towers[i].ChangePosition(Machine);
    //     }
    // }

    void Update()
    {
        if (Machine == null || Machine.IsSleep)
        {
            return;
        }

        // синхронизируем позицию каждой башни
        for (int i = 0; i < Machine.Towers.Count; i++)
        {
            Machine.Towers[i].ChangePosition(Machine);
        }

        // запись угла поворота в данные.
        if (Data.currentAngleBody != transform.localEulerAngles.y)
        {
            Data.currentAngleBody = transform.localEulerAngles.y;
        }

        // включаем или выключаем звук мотора.
        if (Machine.isVisible && isMove)
        {
            if (!Machine.AudioSource.isPlaying)
            {
                Machine.AudioSource.Play();
            }

            if (isMove)
            {
                Machine.AudioSource.volume = 0.5f;
            }
            else
            {
                Machine.AudioSource.volume = 0.1f;
            }
        }
        else
        {
            Machine.AudioSource.Stop();
        }
    }
    #endregion

    public void Init(BaseMachine _machine, DataDetail dataBody)
    {
        Machine = _machine;

        // Option = Machine.Config.body;

        Config = Machine.Config.body.Config;

        DataDetail = dataBody;

        // устанавливаем звук мотора.
        Machine.AudioSource.clip = Config.soundMove;

        // OnChangeData();

        voxelMeshRender.OnSetConfigMeshGenerator(Config.MeshConfig);

        if (Machine.MachineLevelData != null && Machine.MachineLevelData.data != null)
        {
            voxelMeshRender.SetData(Machine.MachineLevelData.data, dataBody);
        }

        // _bodySprite.color = Machine.Config.colorBody;
        // _bodySprite.sprite = Machine.Config.body.spriteBody;
        
        // установка основных параметров.
        OnSetSpeed(Config.speed);
        
        OnSetAngleBody(0);

        SetRelativePoints();
    }

    
    /// <summary>
    /// Устанавливает точки привязки и позиции базовых элементов.
    /// </summary>
    void SetRelativePoints()
    {
        float y = 0f;
        
        if (Machine.Wheels.Count > 0) {
            var heights = Machine.Wheels.Select(x => x.transform.localPosition.y).ToArray();
            var maxHeight = Mathf.Max(heights);

            y += (float)maxHeight / 2f + 1f;
        }

        if (Machine.Caterpillars.Count > 0 && y <= 0f) {
            var heights = Machine.Caterpillars.Select(x => x.transform.localPosition.y).ToArray();
            var maxHeight = Mathf.Max(heights);

            y += (float)maxHeight / 2f + 1f;
        }

        // if (DataDetailBody != null)
        // {
        //     transform.localPosition = new Vector3(DataDetailBody.offset.x, DataDetailBody.offset.y, DataDetailBody.offset.z);
        // } else
        // {
        //     transform.localPosition = new Vector3(Option.offsetTower.x, Option.offsetTower.y, Option.offsetTower.z);
        // }

        y += Config.MeshConfig.sOVoxelData.Bounds.y / 2f;

        transform.localPosition = DataDetail.offset + new Vector3(0, y, 0);
    }
    

    public void OnSetSpeed(float speed)
    {
        _data.speed = speed;
        if (Machine.navMeshAgent != null)
        {
            Machine.navMeshAgent.speed = speed / 100;
        }
    }

    
    public void OnSetAngleBody(float angle)
    {
        transform.rotation = Quaternion.Euler(0, angle + Machine.OffsetRotate, 0);
        // TowerBox.transform.rotation = Quaternion.Euler(0, 0, angle + offset);
        // Machine.Areol.transform.rotation = Quaternion.Euler(0, angle + Machine.OffsetRotate, 0);
        Machine.CaterpillarWrapper.transform.rotation = Quaternion.Euler(0, angle + Machine.OffsetRotate, 0);

        _data.angleBody = transform.eulerAngles.y;

        // for (int i = 0; i < Towers.Count; i++)
        // {
        //     Towers[i].ChangePosition(this);
        // }
    }
    
    public void OnSetAngleBody(Vector3 direction)
    {
        transform.forward = direction;
        Machine.CaterpillarWrapper.transform.forward = direction;

        // var rot = Body.transform.rotation;
        // _objAreol.transform.localEulerAngles = new Vector3(90, rot.eulerAngles.y, rot.eulerAngles.z);
        // Machine.Areol.transform.forward = direction;

        _data.angleBody = transform.eulerAngles.y;

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
    
    // public void OnChangeData()
    // {
    //     // Color col = Color.white;
    //     // col.a = 1f - Mathf.Min(1f, Machine.Data.hp * 100f / Machine.Config.hp * 0.01f);

    //     // _damageSprite.color = col;
    // }

    
    public virtual void Rotate(Vector2 moveDirection)
    {
        float modifSpeedRotate = _gameManager.Settings.simpleMove ? 10f : 2.5f;
        if (Machine.stateController.enabled)
        {
            if (Machine.navMeshAgent.velocity != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(Machine.navMeshAgent.velocity);

                transform.rotation = Machine.CaterpillarWrapper.transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    Quaternion.Euler(0, targetRotation.eulerAngles.y + Machine.OffsetRotate, 0),
                    10f * Time.fixedDeltaTime
                );
            }
        }
        else
        {
            // Quaternion stepRotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 100 * Time.fixedDeltaTime);
            // rb.MoveRotation(stepRotation);
            
            if (_gameManager.Settings.simpleMove)
            {
                if (Machine.Rb.linearVelocity != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(Machine.Rb.linearVelocity);
                    // if (Machine.Rb.linearVelocity != Vector3.zero && _gameManager.Settings.simpleMove)
                    // // if (moveDirection != Vector2.zero)
                    // {
                    //     targetRotation = Quaternion.LookRotation(Machine.Rb.linearVelocity);
                    // }

                    transform.rotation = Machine.CaterpillarWrapper.transform.rotation = Quaternion.Slerp(
                        transform.rotation,
                        Quaternion.Euler(0, targetRotation.eulerAngles.y + Machine.OffsetRotate, 0),
                        modifSpeedRotate * Time.fixedDeltaTime
                    ); 
                }
            } else //(!_gameManager.Settings.simpleMove)
            {
                float rotationAmountY = moveDirection.x * 100 * Time.fixedDeltaTime;
                transform.Rotate(Vector3.up, rotationAmountY, Space.World);
                Machine.CaterpillarWrapper.transform.Rotate(Vector3.up, rotationAmountY, Space.World);
            }

                // Debug.Log($"ROTATION::::: stepRotation={stepRotation}");
        }
    }

    public virtual void Move(Vector2 _moveDirection)
    {
        isMove = true;

        if (Machine.stateController.enabled)
        {
            Machine.Rb.linearVelocity = Machine.navMeshAgent.velocity;
            Machine.navMeshAgent.nextPosition = transform.position;
            // Debug.Log($"navMeshAgent.velocity={navMeshAgent.velocity}");
            Rotate(_moveDirection);
        }
        else
        {

            Vector3 forward;
            Vector3 right;

            Vector3 moveDirection = Vector3.zero;
            if (_gameManager.Settings.simpleMove)
            {
                forward = Machine.LevelManager.cinemachineCamera.transform.forward;  //(transform.position - levelManager.cinemachineCamera.transform.position).normalized;
                right = Machine.LevelManager.cinemachineCamera.transform.right;

                moveDirection = (forward * _moveDirection.y + right * _moveDirection.x).normalized;
            }
            else
            {
                forward = transform.forward;
                right = transform.right;
                moveDirection = (forward * _moveDirection.y).normalized;
            };

            forward.Normalize();
            right.Normalize();

            if (_moveDirection.x != 0 || _gameManager.Settings.simpleMove)
            {
                Rotate(_gameManager.Settings.simpleMove == true ? moveDirection : _moveDirection);

                OnSetDirectionMove(moveDirection);
            }

            // OnSetNameText(moveDirection.ToString());
            // transform.Translate(moveDirection * speed * Time.deltaTime);
            DataBonus bonusSpeed = null;
            Machine.Data.bonuses.TryGetValue(TypeBonus.Speed, out bonusSpeed);
            var speed = Data.speed + (bonusSpeed != null ? bonusSpeed.value : 0);

            // kinematic.
            // rb.MovePosition((Vector3)transform.position + (moveDirection * speed * Time.deltaTime));

            // dynamic.
            if ((_moveDirection.y != 0 && !_gameManager.Settings.simpleMove) || _gameManager.Settings.simpleMove)
            {
                Machine.Rb.linearVelocity = moveDirection * (Data.speed + (bonusSpeed != null ? bonusSpeed.value : 0)) * _gameManager.Settings.scaleObjects;
            } else
            {
                Machine.Rb.linearVelocity = Vector3.zero;
            }
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

            Machine.Data.position = transform.position;

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
        
        for (int i = 0; i < Machine.Caterpillars.Count; i++)
        {
            Machine.Caterpillars[i].Move();
        }
    }

    public virtual void Stop()
    {
        if (isMove)
        {
            isMove = false;

            if (!Machine.Rb.isKinematic)
            {
                Machine.Rb.linearVelocity = Vector3.zero;
                Machine.Rb.angularVelocity = Vector3.zero;
            }

            // for (int i = 0; i < Machine.Caterpillars.Count; i++)
            // {
            //     Machine.Caterpillars[i].Stop();
            // }
        }
    }

    public void OnSetDirectionMove(Vector3 direction)
    {
        Machine.Data.directionMove = direction;
    }

    public void ReDraw(List<ColorsModify> colors)
    {
        voxelMeshRender.UploadedAllMeshes(colors);
    }
    public void ReDraw(DataDetail dataDetail)
    {
        if (this == null) return;

        List<GameBody> allConfigs = _gameManager.ResourceSystem.GetAllBody();

        Config = allConfigs.FirstOrDefault(x => x.name == dataDetail.nameConfig);;

        SetRelativePoints();

        voxelMeshRender.OnSetConfigMeshGenerator(Config.MeshConfig);
        voxelMeshRender.UploadedAllMeshes(dataDetail);
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

    public void OnCollision(BaseMachine ktoStrelyal, Vector3 _pointCollision, bool isDrawMesh, GameObject explodeGameObject, int damageRadius, Vector3 direction, Vector3 normal)
    {
        for (int i = 0; i < voxelMeshRender.Containers.Length; i++)
        {
            if (voxelMeshRender.Containers[i].IsDestructible())
            {
                Vector3 localPoint = voxelMeshRender.Containers[i].transform.InverseTransformPoint(_pointCollision);
                if (voxelMeshRender.Containers[i].PointInCollider(_pointCollision))
                {
                    // Debug.Log($"<color=blue>Body OnCollision: {_pointCollision} / {localPoint}</color>");
                    voxelMeshRender.Containers[i].ExposionVoxels(ktoStrelyal, localPoint, isDrawMesh, explodeGameObject, damageRadius, direction, normal).Forget();
                }
            }
        }
    }

    public void OnSetSpriteGerb(Sprite sprite)
    {
        // _bodyGerbSprite.sprite = sprite;
    }
    
    /// <summary>
    /// Функция расчета ХР для базы машины.
    /// </summary>
    /// <returns>кол-во всех вокселей и кол-во разрушенных, ХР - от 0 до 1</returns>
    public ContainerData RefreshHP()
    {
        // float totalVoxels = 0f;
        // float totalVoxelsDestructible = 0f;
        var result = new ContainerData();

        result.countVoxels = voxelMeshRender.Config.sOVoxelData.countVoxels;

        if (voxelMeshRender.Containers != null)
        {
            // Debug.Log($"voxelMeshRender.Containers.length={voxelMeshRender.Containers.Length}");
            
            for (int i = 0; i < voxelMeshRender.Containers.Length; i++)
            {
                result.countVoxelsDestructible += voxelMeshRender.Containers[i].ContainerData.countVoxelsDestructible;
            }
        }

        result.levelDestruction = (float)result.countVoxelsDestructible / result.countVoxels;

        _data.containerData = result;

        return result;
    }
    
#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Color color = _gameManager ? _gameManager.Settings.DebugSettings.gizmoBodyColor : Color.yellow;
        bool isDraw = _gameManager ? _gameManager.Settings.DebugSettings.gizmoBodyForwards : true;
        float length = _gameManager ? _gameManager.Settings.DebugSettings.gizmoBodyLength : 30;
        if (isDraw)
        {
            Gizmos.color = color;
            Gizmos.DrawRay(transform.position, transform.forward * length);
            // Gizmos.DrawGUITexture(new Rect(new Vector3(0,0,0), new Vector2(10,2)));
            Vector3 textPosition = transform.position + transform.forward * 1;
            Handles.Label(textPosition, "body forward");
        }
    }
#endif

}
