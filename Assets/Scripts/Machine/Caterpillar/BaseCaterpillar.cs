using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Mikalai2006.Voxel;
using UnityEngine;

public class BaseCaterpillar : MonoBehaviour, IColored
{
    // [SerializeField] public List<Animator> animators;
    // [SerializeField] public SpriteRenderer sprite;
    [SerializeField] public List<TrailRenderer> trails;
    GameCaterpillarOption Option;
    BaseMachine Machine;
    [SerializeField] GameObject Wrapper;
    [SerializeField] protected DataCaterpillar _data;
    public DataCaterpillar Data => _data;
    [SerializeField] List<GameObject> wheels = new();
    [SerializeField] protected List<VoxelMeshRender> voxelMeshRenders;

#region Unity methods
    void Awake()
    {
        _data= new();
        wheels = new();
        // sprite = GetComponent<SpriteRenderer>();
        Stop();
    }

    void Start()
    {
        Stop();
    }

    void Update()
    {
        if (Option.Config.isRotate && Machine.IsMove)
        {
            for (int i = 0; i < wheels.Count; i++)
            {
                wheels[i].transform.Rotate(Vector3.right, 5f * Machine.Body.Data.speed * Time.deltaTime);
            }
        }
    }
#endregion

    public void Init(BaseMachine baseMachine, GameCaterpillarOption config, int i, DataDetail dataCat)
    {
        Machine = baseMachine;

        Option = config;

        Parallel.For(0, voxelMeshRenders.Count, (i) =>
        {
            voxelMeshRenders[i].OnSetConfigMeshGenerator(Option.Config.MeshConfig);

            if (Machine.MachineLevelData != null && Machine.MachineLevelData.data != null)
            {
                voxelMeshRenders[i].SetData(Machine.MachineLevelData.data, dataCat);
            }
        });

        

        // sprite.sprite = Option.Config.sprite;
        // sprite.color = Option.Config.color;

        transform.localPosition = Option.offsetCat;

        // Debug.Log($"CaterpillarBox.transform.childCount={CaterpillarBox.transform.childCount}");
        for (int j = 0; j < Wrapper.transform.childCount; j++)
        {
            wheels.Add(Wrapper.transform.GetChild(j).gameObject);
            // Wrapper.transform.GetChild(j).GetChild(0).transform.localPosition = new Vector3(0.5f,0.5f,0.5f);
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
                        Debug.Log($"<color=blue>Caterpillar OnCollision: {_pointCollision} / {localPoint}</color>");
                        tasks.Add(voxelMeshRenders[x].Containers[i].ExposionVoxels(ktoStrelyal, localPoint, isDrawMesh, explodeGameObject, damageRadius, direction, normal));
                    }
                }
            }
        }
        UniTask.WhenAll(tasks).Forget();
    }

    public void Move()
    {
        // // foreach (Animator animator in animators)
        // // {
        // //     animator.SetBool("move", true);
        // // }

        // foreach (TrailRenderer trail in trails)
        // {
        //     trail.emitting = true;
        // }
    }
    
    public void Stop()
    {
        // // foreach (Animator animator in animators)
        // // {
        // //     animator.SetBool("move", false);
        // // }
        // foreach (TrailRenderer trail in trails)
        // {
        //     trail.emitting = false;
        // }
    }
    
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
                    
                        Debug.Log($"<color=purple>Caterpillar[{Machine.MachineLevelData.id}] OnPointer: {_pointPointer}:::{localPoint}:::{pos}|||{groupColor32}-{voxel.color}</color>");
                    }
                }
            }
        }
        return output;
    }
}
