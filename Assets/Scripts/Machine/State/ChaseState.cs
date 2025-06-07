using System;
using UnityEngine;

[Serializable]
public class ChaseState : State
{
    public override void OnEnter(StateController sc)
    {
        base.OnEnter(sc);

        if (stateController.Machine.ObjectTarget != null)
        {
            stateController.Target = stateController.Machine.ObjectTarget.transform.position;
        }
    }

    public override void OnUpdate()
    {
        if (stateController.Machine.ObjectTarget != null)
        {
            if (stateController.Target == Vector3.zero)
            {
                stateController.Target = stateController.Machine.ObjectTarget.transform.position + new Vector3(UnityEngine.Random.Range(-3, 3), UnityEngine.Random.Range(-3, 3), 0);
            }
            else
            {
                if (stateController.Obstacle != Vector3.zero)
                {
                    Vector3 dirVector = stateController.Machine.transform.position - stateController.Obstacle;
                    float distance = Vector3.Distance(stateController.Obstacle, stateController.Machine.transform.position);
                    if (distance > 0.5f && distance < 5)
                    {
                        // Debug.Log($"Normalize: {dirVector.normalized}, distance={distance}");
                        stateController.Machine.Move(dirVector.normalized);
                    }
                    else
                    {
                        // Target = new Vector3(UnityEngine.Random.Range(1, _gameManager.LevelConfig.gridSize.x -1), UnityEngine.Random.Range(2, _gameManager.LevelConfig.gridSize.y -1));
                        stateController.Obstacle = Vector3.zero;
                    }
                }
                else
                {
                    Vector3 dirVector = stateController.Target - stateController.Machine.transform.position;
                    float distance = Vector3.Distance(stateController.Target, stateController.Machine.transform.position);
                    if (distance < stateController.Machine.Config.distanceSearch)
                    {
                        if (distance < 2f)
                        {
                            // stateController.Machine.Move(dirVector.normalized);
                            stateController.Target = Vector3.zero;
                        }
                        else
                        {
                            stateController.Machine.Move(dirVector.normalized);
                        }
                            // stateController.Machine.Move(dirVector.normalized);
                    }
                    else
                    {
                        stateController.ChangeState(stateController.patrolState);
                        // stateController.Target = stateController.Enemy.transform.position + new Vector3(UnityEngine.Random.Range(2,3),UnityEngine.Random.Range(2,3),0);
                    }
                }


            }
        }
        else
        {
            stateController.ChangeState(stateController.patrolState);
        }
    }

    public override void OnExit()
    {
        base.OnExit();

        stateController.Target = Vector3.zero;

        if (stateController.Machine) {
            stateController.Machine.OnSetTarget(null);
        }
    }
}

