using System;
using UnityEngine;

[Serializable]
public class IdleState : State
{
    private float timeWait = 0;

    public override void OnEnter(StateController sc)
    {
        base.OnEnter(sc);
        // Debug.Log($"<color=green>IdleState Start</color>");

        stateController.Machine.Stop();
        if (stateController.Machine.navMeshAgent.isActiveAndEnabled)
        {
            stateController.Machine.navMeshAgent.isStopped = true;
        }

        timeWait = 1.0f;
    }

    public override void OnUpdate()
    {
        if (timeWait <= 0)
        {
            if (!stateController.Machine.navMeshAgent.pathPending)
            {

                stateController.Target = stateController.Machine.LevelManager.mapManager.GetRandomNavmeshLocation(100);
                stateController.ChangeState(stateController.patrolState);
            }
        }
        else
        {
            timeWait -= Time.deltaTime;
        }
    }

    public override void OnExit()
    {
        base.OnExit();
        if (stateController.Machine.navMeshAgent.isActiveAndEnabled)
        {
            stateController.Machine.navMeshAgent.isStopped = false;
        }
        // Debug.Log($"<color=green>IdleState Stop</color>");
    }
}
