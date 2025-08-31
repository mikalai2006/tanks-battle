using System;
using UnityEngine;

[Serializable]
public class PatrolState : State
{
    public override void OnEnter(StateController sc)
    {
        base.OnEnter(sc);
        // Debug.Log($"<color=yellow>PatrolState Start</color>");

        stateController.Machine.navMeshAgent.destination = stateController.Target;
    }

    public override void OnUpdate()
    {
        // if (stateController.Machine.navMeshAgent.velocity.magnitude <= 0)
        // if (stateController.Machine.navMeshAgent.remainingDistance < 0.1f)
        if (stateController.Machine.navMeshAgent.isStopped || stateController.Machine.navMeshAgent.remainingDistance < 0.1f)
        {
            stateController.ChangeState(stateController.idleState);
        }
    }

    public override void OnFixedUpdate()
    {
        stateController.Machine.Move(stateController.Machine.navMeshAgent.velocity);
    }

    public override void OnExit()
    {
        base.OnExit();

        stateController.Target = Vector3.zero;
        // Debug.Log($"<color=yellow>PatrolState Stop: isStopped={stateController.Machine.navMeshAgent.isStopped}, remainingDistance={stateController.Machine.navMeshAgent.remainingDistance}</color>");
    }
}
