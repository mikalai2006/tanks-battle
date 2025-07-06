using System.Collections.Generic;
using UnityEngine;

public class BaseCaterpillar : MonoBehaviour
{
    [SerializeField] public List<Animator> animators;
    [SerializeField] public SpriteRenderer sprite;
    [SerializeField] public List<TrailRenderer> trails;
    public GameObject trailPrefab;
    GameCaterpillarOption Option;

    void Awake()
    {
        // sprite = GetComponent<SpriteRenderer>();
        Stop();
    }

    void Start()
    {
        Stop();
    }



    public void Init(BaseMachine baseMachine, GameCaterpillarOption config, int i)
    {
        Option = config;
        sprite.sprite = Option.Config.sprite;
        sprite.color = Option.Config.color;
        transform.localPosition = new Vector3(Option.offsetCat.x, Option.offsetCat.y);
    }

    public void Move()
    {
        foreach (Animator animator in animators)
        {
            animator.SetBool("move", true);
        }

        foreach (TrailRenderer trail in trails)
        {
            trail.emitting = true;
        }
    }
    
    public void Stop()
    {
        foreach (Animator animator in animators)
        {
            animator.SetBool("move", false);
        }
        foreach (TrailRenderer trail in trails)
        {
            trail.emitting = false;
        }
    }
}
