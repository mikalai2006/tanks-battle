using System.Collections.Generic;
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
        if (isMove)
        {
            for (int i = 0; i < wheels.Count; i++)
            {   
                wheels[i].transform.Rotate(Vector3.right, 20f * Machine.Data.speed * Time.deltaTime);
            }
        }
    }

    public void Init(BaseMachine baseMachine, GameCaterpillarOption config, int i)
    {
        Machine = baseMachine;

        Option = config;

        // sprite.sprite = Option.Config.sprite;
        // sprite.color = Option.Config.color;

        transform.localPosition = Option.offsetCat;

        // Debug.Log($"CaterpillarBox.transform.childCount={CaterpillarBox.transform.childCount}");
        for (int j = 0; j < Wrapper.transform.childCount; j++)
        {
            wheels.Add(Wrapper.transform.GetChild(j).gameObject);
        }

    }

    public void Move()
    {
        isMove = true;
        // foreach (Animator animator in animators)
        // {
        //     animator.SetBool("move", true);
        // }

        foreach (TrailRenderer trail in trails)
        {
            trail.emitting = true;
        }
    }
    
    public void Stop()
    {
        isMove = false;
        // foreach (Animator animator in animators)
        // {
        //     animator.SetBool("move", false);
        // }
        foreach (TrailRenderer trail in trails)
        {
            trail.emitting = false;
        }
    }
    
}
