using System.Collections.Generic;
using UnityEngine;

public class StateController : MonoBehaviour
{
    public BaseMachine Machine { get; private set; }
    [SerializeField] private State currentState;

    // public SleepState sleepState = new SleepState();
    public ChaseState chaseState = new ChaseState();
    public PatrolState patrolState = new PatrolState();
    public HurtState hurtState = new HurtState();
    public AttackState attackState = new AttackState();

    // [SerializeField] public BaseMachine Enemy;
    [SerializeField] public Vector3 Obstacle;
    [SerializeField] public Vector3 Target;
    [SerializeField] public List<GridTileNode> Path;

    void Awake()
    {
        Machine = GetComponent<BaseMachine>();
    }

    private void Start()
    {
        ChangeState(patrolState);
    }

    void Update()
    {

        if (currentState != null)
        {
            currentState.OnUpdate();
        }
    }

    public void ChangeState(State newState)
    {
        if (currentState != null)
        {
            currentState.OnExit();
        }
        currentState = newState;
        currentState.OnEnter(this);
        
        // if (Machine)
        // {
        //     Machine.Badge.OnSetNameText(newState.ToString());
        // }
        // Debug.Log($"Change state machine: - {newState.ToString()}");
    }
}