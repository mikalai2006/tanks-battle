using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Mikalai2006.Voxel;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor; 
#endif

public class BaseWheel : MonoBehaviour, IColored
{
    protected GameManager _gameManager => GameManager.Instance;
    [SerializeField] public List<TrailRenderer> trails;
    public GameWheel Config {get ; private set;}
    BaseMachine Machine;
    protected DataDetail DataDetail;
    [SerializeField] GameObject Wrapper;
    [SerializeField] protected DataWheel _data;
    public DataWheel Data => _data;
    [SerializeField] protected List<VoxelMeshRender> voxelMeshRenders;
    [SerializeField] Vector3 lastPosition;
    [SerializeField] Vector3 lastRotation;
    [SerializeField] float forwardThreshold = 0.01f; // Минимальная скорость

#region Unity methods
    void Awake()
    {
        _data= new();
        // sprite = GetComponent<SpriteRenderer>();
        // Stop();
    }

    // void Start()
    // {
    //     Stop();
    // }

    // void Update()
    // {
    //     if (Option.Config.isRotate && Machine.IsMove)
    //     {
    //         for (int i = 0; i < wheels.Count; i++)
    //         {
    //             wheels[i].transform.Rotate(Vector3.right, 5f * Machine.Body.Data.speed * Time.deltaTime);
    //         }
    //     }
    // }

     void Start()
    {
        lastPosition = Machine.transform.position;
    }

    void Update()
    {
        if (Machine == null  || Machine.IsSleep)
        {
            return;
        }
        
        if (Machine.IsMove)
        {
            // 1. Вычисляем вектор движения
            Vector3 movementDirection = (Machine.transform.position - lastPosition) / Time.deltaTime;
            // Debug.Log($"movementDirection={movementDirection}, movementDirection.magnitude={movementDirection.magnitude}");
            if (movementDirection.magnitude > forwardThreshold)
            {
                // 2. Сравниваем направление движения с локальным "вперед"
                float dotProduct = Vector3.Dot(movementDirection.normalized, Machine.Body.transform.forward);

                if (dotProduct > 0)
                {
                    transform.Rotate(Vector3.right, 5f * Machine.Body.Data.speed * Time.deltaTime);
                }
                else if (dotProduct < 0)
                {
                    transform.Rotate(-Vector3.right, 5f * Machine.Body.Data.speed * Time.deltaTime);
                }
            } else
            {
                // 3. Определяем угол поворота
                float diffAngle = Mathf.DeltaAngle(lastRotation.y, Machine.Body.transform.rotation.eulerAngles.y);
                // Debug.Log($"Need check rotation! lastRotation={lastRotation}, rotation={Machine.Body.transform.rotation.eulerAngles}, diffAngle={diffAngle}");
                
                if (diffAngle >= 0)
                {
                    transform.Rotate(Vector3.right, 5f * Machine.Body.Data.speed * Time.deltaTime);
                }
                else if (diffAngle < 0)
                {
                    transform.Rotate(-Vector3.right, 5f * Machine.Body.Data.speed * Time.deltaTime);
                }
            }

            // 4. Обновляем позицию и вращение для следующего кадра
            lastPosition = Machine.transform.position;
            lastRotation = Machine.Body.transform.rotation.eulerAngles;
        }
    }
#endregion

    public void Init(BaseMachine baseMachine, GameWheel config, int i, DataDetail dataWheel)
    {
        Machine = baseMachine;

        DataDetail = dataWheel;

        Config = config;

        Parallel.For(0, voxelMeshRenders.Count, (i) =>
        {
            voxelMeshRenders[i].OnSetConfigMeshGenerator(Config.MeshConfig);
            
            if (Machine.MachineLevelData != null && Machine.MachineLevelData.data != null)
            {
                voxelMeshRenders[i].SetData(Machine.MachineLevelData.data, dataWheel);
            }
        });

        SetRelativePoints();
    }

    
    /// <summary>
    /// Устанавливает точки привязки и позиции базовых элементов.
    /// </summary>
    public void SetRelativePoints()
    {
        if (Machine.Caterpillars.Count > 0)
        {
            transform.localPosition = DataDetail.offset + new Vector3(0, (Config.MeshConfig.sOVoxelData.Bounds.y / 2f) + 1f, 0);
        } else
        {
            transform.localPosition = DataDetail.offset + new Vector3(0, Config.MeshConfig.sOVoxelData.Bounds.y / 2f, 0);
        }
    }

    public void OnCollision(BaseMachine ktoStrelyal, Vector3 _pointCollision, bool isDrawMesh, GameObject explodeGameObject, int damageRadius, Vector3 direction, Vector3 normal)
    {
        List<UniTask> tasks = new List<UniTask>();
        for (int x = 0; x < voxelMeshRenders.Count; x++)
        {
            for (int i = 0; i < voxelMeshRenders[x].Containers.Length; i++)
            {
                if (voxelMeshRenders[x].Containers[i].IsDestructible())
                {
                    Vector3 localPoint = voxelMeshRenders[x].Containers[i].transform.InverseTransformPoint(_pointCollision);
                    if (voxelMeshRenders[x].Containers[i].PointInCollider(_pointCollision))
                    {
                        Debug.Log($"<color=blue>Wheel OnCollision: {_pointCollision} / {localPoint}</color>");
                        tasks.Add(voxelMeshRenders[x].Containers[i].ExposionVoxels(ktoStrelyal, localPoint, isDrawMesh, explodeGameObject, damageRadius, direction, normal));
                    }
                }
            }
        }
        UniTask.WhenAll(tasks).Forget();
    }

    // public void Move()
    // {
    //     // // foreach (Animator animator in animators)
    //     // // {
    //     // //     animator.SetBool("move", true);
    //     // // }

    //     // foreach (TrailRenderer trail in trails)
    //     // {
    //     //     trail.emitting = true;
    //     // }
    // }
    
    // public void Stop()
    // {
    //     // // foreach (Animator animator in animators)
    //     // // {
    //     // //     animator.SetBool("move", false);
    //     // // }
    //     // foreach (TrailRenderer trail in trails)
    //     // {
    //     //     trail.emitting = false;
    //     // }
    // }
    
    /// <summary>
    /// Функция расчета ХР для шасси машины.
    /// </summary>
    /// <returns>кол-во всех вокселей и кол-во разрушенных, ХР - от 0 до 1</returns>
    public ContainerData RefreshHP()
    {
        var result = new ContainerData();

        for (int x = 0; x < voxelMeshRenders.Count; x++)
        {
            result.countVoxels += voxelMeshRenders[x].Config.sOVoxelData.countVoxels;

            if (voxelMeshRenders[x].Containers != null)
            {
                for (int i = 0; i < voxelMeshRenders[x].Containers.Length; i++)
                {
                    result.countVoxelsDestructible += voxelMeshRenders[x].Containers[i].ContainerData.countVoxelsDestructible;
                    // result.countVoxels += voxelMeshRenders[x].Containers[i].ContainerData.countVoxels;
                    // Debug.Log($"_containerData: {voxelMeshRender.Containers[i].ContainerData.countVoxels}/{voxelMeshRender.Containers[i].ContainerData.countVoxelsDestructible}");
                }
            }
        }
        
        result.levelDestruction = (float)result.countVoxelsDestructible / result.countVoxels;

        _data.containerData = result;

        return result;
    }

    
    public void ReDraw(List<ColorsModify> colors)
    {
        for (int i = 0; i < voxelMeshRenders.Count; i++)
        {
            voxelMeshRenders[i].UploadedAllMeshes(colors);
        };
    }

    public FillData OnFill(Vector3 _pointPointer)
    {
        FillData output = new FillData();

        for (int x = 0; x < voxelMeshRenders.Count; x++)
        {
            for (int i = 0; i < voxelMeshRenders[x].Containers.Length; i++)
            {
                if (voxelMeshRenders[x].Containers[i].PointInCollider(_pointPointer))
                {
                    Vector3 localPoint = voxelMeshRenders[x].Containers[i].transform.InverseTransformPoint(_pointPointer);
                    // voxelMeshRender.Containers[i].ExposionVoxels(ktoStrelyal, localPoint, isDrawMesh, explodeGameObject, damageRadius, direction, normal).Forget();
                    Vector3Int pos = Helpers.RoundVector3(localPoint);

                    Voxel voxel = voxelMeshRenders[x].Containers[i].GetVoxelMinDistance(pos);

                    if (!voxel.color.Equals(Color.clear))
                    {
                        SubmeshesData submeshesData = Config.MeshConfig.sOVoxelData.GetVoxelGroup(voxel.position);

                        Color32 groupColor32 = submeshesData.color;

                        output.voxelGroupData = submeshesData;
                    
                        Debug.Log($"<color=purple>Wheel[{Machine.MachineLevelData.id}] OnPointer: {_pointPointer}:::{localPoint}:::{pos}|||{groupColor32}-{voxel.color}</color>");
                    }
                }
            }
        }
        return output;
    }

    
#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Color color = _gameManager ? _gameManager.Settings.DebugSettings.gizmoWheelsColor : Color.yellow;
        bool isDraw = _gameManager ? _gameManager.Settings.DebugSettings.gizmoWheels : true;
        float length = _gameManager ? _gameManager.Settings.DebugSettings.gizmoWheelsLength : 30;
        if (isDraw)
        {
            Gizmos.color = color;
            Gizmos.DrawRay(transform.position, transform.forward * length);
            // Gizmos.DrawGUITexture(new Rect(new Vector3(0,0,0), new Vector2(10,2)));
            Vector3 textPosition = transform.position + transform.forward * 1;
            Handles.Label(textPosition, "wheel forward");
        }
    }
#endif
}
