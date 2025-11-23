using Cysharp.Threading.Tasks;
using Mikalai2006.Voxel;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor; 
#endif

public class BaseBody : MonoBehaviour
{
    protected GameManager _gameManager => GameManager.Instance;
    [SerializeField] private SpriteRenderer _bodySprite;
    [SerializeField] private SpriteRenderer _bodyGerbSprite;
    [SerializeField] private SpriteRenderer _damageSprite;
    protected BaseMachine Machine;
    protected GameBody Config;
    [SerializeField] protected DataBody _data;
    public DataBody Data => _data;
    [SerializeField] protected bool isMove;
    public bool IsMove => isMove;
    [SerializeField] protected VoxelMeshRender voxelMeshRender;

#region Unity methods
    void Awake()
    {
        _data = new();
    }
    
    void FixedUpdate()
    {
        // синхронизируем позицию каждой башни
        for (int i = 0; i < Machine.Towers.Count; i++)
        {
            Machine.Towers[i].ChangePosition(Machine);
        }
    }

    void Update()
    {
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

    public void Init(BaseMachine _machine)
    {
        Machine = _machine;

        Config = Machine.Config.body;

        // OnChangeData();

        voxelMeshRender.OnSetConfigMeshGenerator(Config.MeshConfig);

        // _bodySprite.color = Machine.Config.colorBody;
        // _bodySprite.sprite = Machine.Config.body.spriteBody;
        
        // установка основных параметров.
        OnSetSpeed(Config.speed);
        
        OnSetAngleBody(0);
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
        Machine.Areol.transform.rotation = Quaternion.Euler(0, angle + Machine.OffsetRotate, 0);
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
        Machine.Areol.transform.forward = direction;

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
            if (Machine.Rb.linearVelocity != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(Machine.Rb.linearVelocity);
                // Debug.Log($"Rotate::::: {targetRotation}");
                // Quaternion stepRotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 100 * Time.fixedDeltaTime);

                // rb.MoveRotation(stepRotation);
                transform.rotation = Machine.CaterpillarWrapper.transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    Quaternion.Euler(0, targetRotation.eulerAngles.y + Machine.OffsetRotate, 0),
                    10f * Time.fixedDeltaTime
                );

                // Debug.Log($"ROTATION::::: stepRotation={stepRotation}");
            }
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

            if (_gameManager.Settings.simpleMove)
            {
                forward = Machine.LevelManager.cinemachineCamera.transform.forward;  //(transform.position - levelManager.cinemachineCamera.transform.position).normalized;
                right = Machine.LevelManager.cinemachineCamera.transform.right;
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
            Machine.Data.bonuses.TryGetValue(TypeBonus.Speed, out bonusSpeed);
            var speed = Data.speed + (bonusSpeed != null ? bonusSpeed.value : 0);

            // kinematic.
            // rb.MovePosition((Vector3)transform.position + (moveDirection * speed * Time.deltaTime));

            // dynamic.
            Machine.Rb.linearVelocity = moveDirection * (Data.speed + (bonusSpeed != null ? bonusSpeed.value : 0)) * _gameManager.Settings.scaleObjects;
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
        isMove = false;

        if (!Machine.Rb.isKinematic)
        {
            Machine.Rb.linearVelocity = Vector3.zero;
            Machine.Rb.angularVelocity = Vector3.zero;
        }

        for (int i = 0; i < Machine.Caterpillars.Count; i++)
        {
            Machine.Caterpillars[i].Stop();
        }
    }


    public void OnSetDirectionMove(Vector3 direction)
    {
        Machine.Data.directionMove = direction;
    }

    public void OnCollision(BaseMachine ktoStrelyal, Vector3 _pointCollision, bool isDrawMesh, GameObject explodeGameObject, int damageRadius, Vector3 direction, Vector3 normal)
    {
        for (int i = 0; i < voxelMeshRender.Containers.Length; i++)
        {
            if (voxelMeshRender.Containers[i].IsDestructible())
            {
                Vector3 localPoint = voxelMeshRender.Containers[i].transform.InverseTransformPoint(_pointCollision);
                // Debug.Log($"<color=green>Body OnCollision: {_pointCollision} / {localPoint}</color>");
                voxelMeshRender.Containers[i].ExposionVoxels(ktoStrelyal, localPoint, isDrawMesh, explodeGameObject, damageRadius, direction, normal).Forget();
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
