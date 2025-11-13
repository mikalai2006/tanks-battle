using System.Collections.Generic;
using UnityEngine;

public class StateController : MonoBehaviour
{
    public BaseMachine Machine { get; private set; }
    [SerializeField] private State currentState;
    [SerializeField] private string currentStateName;

    // public SleepState sleepState = new SleepState();
    public ChaseState chaseState = new ChaseState();
    public PatrolState patrolState = new PatrolState();
    public HurtState hurtState = new HurtState();
    public AttackState attackState = new AttackState();
    public IdleState idleState = new IdleState();

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
        ChangeState(idleState);
    }

    void Update()
    {

        if (currentState != null)
        {
            currentState.OnUpdate();
        }
    }

    void FixedUpdate()
    {
        if (currentState != null)
        {
            currentState.OnFixedUpdate();
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
        currentStateName = newState.GetType().ToString();
        
        // if (Machine)
        // {
        //     Machine.Badge.OnSetNameText(newState.ToString());
        // }
        // Debug.Log($"Change state machine: - {newState.ToString()}");
    }
}