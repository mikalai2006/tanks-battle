using UnityEngine;
using Cysharp.Threading.Tasks;
using Mikalai2006.Voxel;
using System.Collections.Generic;

public abstract class BaseMuzzle : MonoBehaviour
{
    protected GameManager _gameManager = GameManager.Instance;
    [SerializeField] private Animator _animator;
    protected BaseMachine Machine;
    [SerializeField] protected GameObject pivot;
    [SerializeField] protected GameObject pointEffects;
    public GameObject PointEffects => pointEffects;
    [SerializeField] protected GameMuzzleOption Option;
    [SerializeField] protected GameMuzzle Config => Option.Config;
    [SerializeField] protected SpriteRenderer sprite;
    protected ParticleSystem[] particlesBoom;
    [SerializeField] protected DataMuzzle data;
    public DataMuzzle Data => data;
    [SerializeField] protected BaseTower Tower;
    [SerializeField] protected GameObject MaxDistanceObject;
    public TrajectoryRenderer trajectoryRenderer;
    public GameObject decal;
    [SerializeField] protected VoxelMeshRender voxelMeshRender;
    [SerializeField] private LayerMask ignoreMask;
    [SerializeField] protected Light SpotLight;

    #region Unity methods
    void Awake()
    {
        data = new();
    }

    public virtual void Update()
    {
        // обновляем время до выстрела
        if (data.timeBeforeShot > 0 && Tower.Data.isShot)
        {
            OnSetTimeBetweenShot(data.timeBeforeShot - Time.deltaTime);
        }

    }
    #endregion

    public void Init(BaseMachine _machine, BaseTower tower, GameMuzzleOption option, int index)
    {
        Option = option;

        Tower = tower;

        Machine = _machine;

        SpotLight.enabled = !Machine.MachineLevelData.isBot;

        data.index = index;

        // MeshConfig meshConfig = Config.MeshConfig;
        // meshConfig.sOVoxelData.Pivot = new Vector3(meshConfig.sOVoxelData.Pivot.x,meshConfig.sOVoxelData.Pivot.y,1);

        voxelMeshRender.OnSetConfigMeshGenerator(Config.MeshConfig);

        // sprite.color = Config.color;
        // particlesBoom = particlesBoomGameObject.GetComponentsInChildren<ParticleSystem>();

        OnSetTimeBetweenShot(Config.timeBetweenShot + (data.index * (Config.timeBetweenShot / 2)));

        transform.localPosition = Option.offsetMuzzle;

        pointEffects.transform.localPosition = new Vector3(Config.MeshConfig.sOVoxelData.Bounds.x + 5f, 0, 0);

        pivot.transform.localPosition = new Vector3(-Config.MeshConfig.sOVoxelData.Bounds.x, 0, 0);

        MaxDistanceObject.transform.localPosition = new Vector3(Config.distanceAttack * (1 / _gameManager.Settings.scaleObjects), 0, 0);

        transform.localRotation = Quaternion.Euler(0, 90, 0);
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
        if (_gameManager.Settings.playerOptions.showTrajectory)
        {
            trajectoryRenderer.ShowStretchTrajectory(pointEffects.transform.position, point);
        }
    }

    public void OnSetRotation(Vector3 pointCenterScreen, float speedRotate)
    {
        // var offset = Machine.LevelManager.Camera.WorldToScreenPoint(new Vector3(0,0,Option.offsetMuzzle.z));
        // Debug.Log($"forward={transform.forward}");
        // Vector2 screenCenterPoint = new Vector2(Screen.width / 2f - offset.x, Screen.height / 2f);
        // Vector3 centerScreenWithOffsetMuzzle = Machine.LevelManager.Camera.ScreenToWorldPoint(screenCenterPoint);
        // MaxDistanceObject.transform.position = new Vector3(MaxDistanceObject.transform.position.x,pointCenterScreen.y,MaxDistanceObject.transform.position.z);

        // бросаем линию вперед на расстояние атаки.
        RaycastHit hit;
        Vector3 distanceAndDirection = transform.forward * 500f;
        Vector3 endPoint = MaxDistanceObject.transform.position; // pointEffects.transform.position + distanceAndDirection;
        Vector3 pointTarget;
        Vector3 castPoint;
        if (Physics.Linecast(pointEffects.transform.position, endPoint, out hit, ~(ignoreMask)))
        {
            pointTarget = hit.point;
            castPoint = pointCenterScreen;
        }
        else
        {
            pointTarget = MaxDistanceObject.transform.position; //transform.position + distanceAndDirection;
            castPoint = pointCenterScreen;
        };

        decal.transform.position = pointTarget - distanceAndDirection.normalized * 0.1f;
        decal.transform.rotation = Quaternion.LookRotation(-distanceAndDirection.normalized);

        var directionLook = castPoint - pivot.transform.position;

        Quaternion lookRotation = Quaternion.LookRotation(directionLook, Vector3.up);
        OnSetAngle(lookRotation, pointTarget, speedRotate);


        Debug.DrawLine(pointEffects.transform.position, pointTarget, Color.white);


        // Debug.DrawLine(pivot.transform.position, pointCenterScreen, Color.yellow);
        // var offset = Machine.LevelManager.Camera.WorldToScreenPoint(new Vector3(0,0,Option.offsetMuzzle.z));

        // decal.transform.position = pointCenterScreen - pointCenterScreen.normalized * 0.1f;
        // decal.transform.rotation = Quaternion.LookRotation(-pointCenterScreen.normalized);

        // var direction = pointCenterScreen - Option.offsetMuzzle - pivot.transform.position;
        // Quaternion lookRotation = Quaternion.LookRotation(direction);

        // OnSetAngle(lookRotation, pointCenterScreen, speedRotate);
    }


    public void OnSetTimeBetweenShot(float time)
    {
        data.timeBeforeShot = time;
        // Badge.OnChangeData(this);
    }

    /// <summary>
    /// Функция остановки стрельбы из дула.
    /// </summary>
    public void OnStopShot()
    {
        if (data.countShotSeria != 0)
        {
            data.countShotSeria = 0;
            OnSetTimeBetweenShot(data.index * (Config.timeBetweenShot / 2));
        }
    }

    public virtual void OnShot(GameObject target)
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

        // // for (int i = 0; i < particlesBoom.Length; i++)
        // // {
        // //     particlesBoom[i].gameObject.SetActive(true);
        // // }
        // if (_animator)
        // {
        //     _animator.SetTrigger("shot");
        // }

        // TODO Effect stretch fire muzzle
        GameObject objEffect = Lean.Pool.LeanPool.Spawn(Config.fireEffect, Machine.LevelManager.objectSpawnEffect.transform, false);
        objEffect.transform.position = pointEffects.transform.position;

        ParticleSystem[] particles = objEffect.transform.GetChild(0).GetComponentsInChildren<ParticleSystem>();
        if (particles.Length > 0)
        {
            for (int i = 0; i < particles.Length; i++)
            {
                var main = particles[i].main;
                var rend = particles[i].GetComponent<ParticleSystemRenderer>();
                rend.material = Config.material; //gameObject.GetComponent<MeshRenderer>().material;
            }
        }
        objEffect.transform.eulerAngles = new Vector3(0, Tower.transform.eulerAngles.z, 0);
        Lean.Pool.LeanPool.Despawn(objEffect, 2);


        OnSetTimeBetweenShot(Config.timeBetweenShot);
    }

    public void OnCollision(Vector3 _pointCollision, bool isDrawMesh, GameObject explodeGameObject, int damageRadius, Vector3 direction)
    {
        List<UniTask> tasks = new List<UniTask>(0);
        for (int i = 0; i < voxelMeshRender.Containers.Length; i++)
        {
            if (voxelMeshRender.Containers[i].IsDestructible())
            {
                Vector3 localPoint = voxelMeshRender.Containers[i].transform.InverseTransformPoint(_pointCollision);
                // Debug.Log($"<color=green>Body OnCollision: {_pointCollision} / {localPoint}</color>");
                tasks.Add(voxelMeshRender.Containers[i].ExposionVoxels(localPoint, isDrawMesh, explodeGameObject, damageRadius, direction));
            }
        }
        UniTask.WhenAll(tasks).Forget();
    }

    public float GetValueDestructible()
    {
        float totalVoxels = 0f;
        float totalVoxelsDestructible = 0f;

        for (int i = 0; i < voxelMeshRender.Containers.Length; i++)
        {
            totalVoxelsDestructible += voxelMeshRender.Containers[i].ContainerData.countVoxelsDestructible;
            totalVoxels += voxelMeshRender.Containers[i].ContainerData.countVoxels;
        }

        float result = totalVoxelsDestructible / totalVoxels;

        return result;
    }
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
