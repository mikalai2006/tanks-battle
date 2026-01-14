using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Mikalai2006.Voxel;
using UnityEngine;

public class BaseWheel : MonoBehaviour, IColored
{
    [SerializeField] public List<TrailRenderer> trails;
    public GameWheelOption Option {get ; private set;}
    BaseMachine Machine;
    [SerializeField] GameObject Wrapper;
    [SerializeField] protected DataWheel _data;
    public DataWheel Data => _data;
    [SerializeField] protected List<VoxelMeshRender> voxelMeshRenders;

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
#endregion

    public void Init(BaseMachine baseMachine, GameWheelOption config, int i, DataDetail dataWheel)
    {
        Machine = baseMachine;

        Option = config;

        Parallel.For(0, voxelMeshRenders.Count, (i) =>
        {
            voxelMeshRenders[i].OnSetConfigMeshGenerator(Option.Config.MeshConfig);
            
            if (Machine.MachineLevelData != null && Machine.MachineLevelData.data != null)
            {
                voxelMeshRenders[i].SetData(Machine.MachineLevelData.data, dataWheel);
            }
        });

        // sprite.sprite = Option.Config.sprite;
        // sprite.color = Option.Config.color;

        if (Machine.Caterpillars.Count > 0)
        {
            transform.localPosition = Option.offsetWheel + new Vector3(0, 1f, 0);
        } else
        {
            transform.localPosition = Option.offsetWheel;
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
                        SubmeshesData submeshesData = Option.Config.MeshConfig.sOVoxelData.GetVoxelGroup(voxel.position);

                        Color32 groupColor32 = submeshesData.color;

                        output.voxelGroupData = submeshesData;
                    
                        Debug.Log($"<color=purple>Wheel[{Machine.MachineLevelData.id}] OnPointer: {_pointPointer}:::{localPoint}:::{pos}|||{groupColor32}-{voxel.color}</color>");
                    }
                }
            }
        }
        return output;
    }
}
