using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.ResourceManagement.AsyncOperations;
using System;

public abstract class BaseMuzzle : MonoBehaviour
{
    protected GameManager _gameManager = GameManager.Instance;
    [SerializeField] private Animator _animator;
    protected BaseMachine Machine;
    [SerializeField] protected GameObject pointEffects;
    [SerializeField] protected GameMuzzleOption Option;
    [SerializeField] protected GameMuzzle Config => Option.Config;
    [SerializeField] protected SpriteRenderer sprite;
    protected ParticleSystem[] particlesBoom;
    [SerializeField] protected DataMuzzle data;
    [SerializeField] protected BaseTower Tower;
    public DataMuzzle Data => data;

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

        // 
    }
    #endregion

    public void Init(BaseMachine _machine, BaseTower tower, GameMuzzleOption option, int index)
    {
        Option = option;

        Tower = tower;

        Machine = _machine;

        data.index = index;

        // sprite.color = Config.color;
        // particlesBoom = particlesBoomGameObject.GetComponentsInChildren<ParticleSystem>();

        OnSetTimeBetweenShot(Config.timeBetweenShot + (data.index * (Config.timeBetweenShot / 2)));
        
        transform.localPosition = Option.offsetMuzzle;
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

        // for (int i = 0; i < particlesBoom.Length; i++)
        // {
        //     particlesBoom[i].gameObject.SetActive(true);
        // }
        if (_animator)
        {
            _animator.SetTrigger("shot");
        }

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

    // bool AnimatorIsPlaying(string stateName) {
    //     return _animator.GetCurrentAnimatorStateInfo(0).length > _animator.GetCurrentAnimatorStateInfo(0).normalizedTime
    //         && _animator.GetCurrentAnimatorStateInfo(0).IsName(stateName);
    // }

     public void LoadedAsset(AsyncOperationHandle<GameObject> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            BaseBullet obj = handle.Result.GetComponent<BaseBullet>();
            if (obj != null)
            {
                obj.OnInit(Machine, Tower, Config);
            }
        }
        else
        {
            Debug.LogError($"Error Load prefab::: {handle.Status}");
        }
    }

}
