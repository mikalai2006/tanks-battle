using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Mikalai2006.Voxel;
using UnityEngine;

public class BaseCaterpillar : MonoBehaviour
{
    // [SerializeField] public List<Animator> animators;
    // [SerializeField] public SpriteRenderer sprite;
    [SerializeField] public List<TrailRenderer> trails;
    GameCaterpillarOption Option;
    BaseMachine Machine;
    [SerializeField] GameObject Wrapper;
    [SerializeField] List<GameObject> wheels = new();
    bool isMove = false;
    [SerializeField] protected List<VoxelMeshRender> voxelMeshRenders;

    void Awake()
    {
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
        if (Option.Config.isRotate)
        {
            if (isMove)
            {
                for (int i = 0; i < wheels.Count; i++)
                {
                    wheels[i].transform.Rotate(Vector3.right, 5f * Machine.Data.speed * Time.deltaTime);
                }
            }
        }
    }

    public void Init(BaseMachine baseMachine, GameCaterpillarOption config, int i)
    {
        Machine = baseMachine;

        Option = config;

        Parallel.For(0, voxelMeshRenders.Count, (i) =>
        {
            voxelMeshRenders[i].OnSetConfigMeshGenerator(Option.Config.MeshConfig);
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

    public void OnCollision(Vector3 _pointCollision, bool isDrawMesh, GameObject explodeGameObject, int damageRadius)
    {
        List<UniTask> tasks = new List<UniTask>();
        for (int x = 0; x < voxelMeshRenders.Count; x++)
        {
            for (int i = 0; i < voxelMeshRenders[x].Containers.Length; i++)
            {
                Vector3 localPoint = voxelMeshRenders[x].Containers[i].transform.InverseTransformPoint(_pointCollision);
                // Debug.Log($"<color=green>Body OnCollision: {_pointCollision} / {localPoint}</color>");
                tasks.Add(voxelMeshRenders[x].Containers[i].ExposionVoxels(localPoint, isDrawMesh, explodeGameObject, damageRadius));
            }
        }
        UniTask.WhenAll(tasks).Forget();
    }

    public void Move()
    {
        isMove = true;
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
        isMove = false;
        // // foreach (Animator animator in animators)
        // // {
        // //     animator.SetBool("move", false);
        // // }
        // foreach (TrailRenderer trail in trails)
        // {
        //     trail.emitting = false;
        // }
    }
    
}
