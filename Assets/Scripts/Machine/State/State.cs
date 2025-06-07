
using System;
using UnityEngine;


[Serializable]
public abstract class State
{
    protected GameManager _gameManager => GameManager.Instance;
    [SerializeField] protected StateController stateController;

    public virtual void OnEnter(StateController sc)
    {
        stateController = sc;
    }

    public virtual void OnUpdate()
    {

    }

    public virtual void OnHurt()
    {

    }

    public virtual void OnExit()
    {

    }

    // public virtual void OnSetEnemy(BaseMachine enemy)
    // {
    //     stateController.Enemy = enemy;
    //     stateController.Machine.OnSetTarget(enemy);
    // }
    public virtual void OnSetObstacle(Vector3 _obstacle)
    {
        stateController.Obstacle = _obstacle;
    }
}