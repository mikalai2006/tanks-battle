using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PatrolState : State
{
    private float timeWait = 0;

    public override void OnEnter(StateController sc)
    {
        base.OnEnter(sc);

        stateController.Target = Vector3.zero;

        timeWait = 1f;
    }

    public override void OnUpdate()
    {
        if (timeWait <= 0)
        {
            timeWait = 0;

            if (stateController.Machine.ObjectTarget != null)
            {
                stateController.ChangeState(stateController.chaseState);
            }
            else
            {
                if (stateController.Path.Count == 0)
                {
                    List<GridTileNode> allVacantNodes = stateController.Machine.LevelManager.mapManager.gridTileHelper.GetEmptyNodes();
                    GridTileNode nodeTo = allVacantNodes[UnityEngine.Random.Range(0, allVacantNodes.Count - 1)];

                    stateController.Path = stateController.Machine.LevelManager.mapManager.gridTileHelper.FindPath(
                        stateController.Machine.OccupiedNode.position,
                        nodeTo.position,
                        true
                    );

                    // for (int i = 0; i < stateController.Path.Count; i++)
                    // {
                    //     stateController.Machine.LevelManager.mapManager.OnSetColor(stateController.Path[i], Color.magenta);
                    // }
                }

                // if (stateController.Target == Vector3.zero)
                // {
                //     List<GridTileNode> allVacantNodes = stateController.Machine.LevelManager.mapManager.gridTileHelper.GetEmptyNodes();
                //     GridTileNode nodeTo = allVacantNodes[UnityEngine.Random.Range(0, allVacantNodes.Count - 1)];
                //     stateController.Path = stateController.Machine.LevelManager.mapManager.gridTileHelper.FindPath(
                //         stateController.Machine.OccupiedNode.position,
                //         nodeTo.position,
                //         false
                //     );
                //     stateController.Target = stateController.Path[0].position;
                //     // new Vector3(UnityEngine.Random.Range(1, _gameManager.LevelConfig.gridSize.x - 1), UnityEngine.Random.Range(1, _gameManager.LevelConfig.gridSize.y - 1));
                // }
                // else
                // {
                if (stateController.Obstacle != Vector3.zero)
                {
                    Vector3 dirVector = stateController.Machine.transform.position - stateController.Obstacle;
                    float distance = Vector3.Distance(stateController.Obstacle, stateController.Machine.transform.position);
                    if (distance < 3f && distance > 1f)
                    {
                        // Debug.Log($"Normalize: {dirVector.normalized}, distance={distance}");
                        stateController.Machine.Move(dirVector.normalized);
                    }
                    else
                    {
                        // stateController.Target = Vector3.zero;  //new Vector3(UnityEngine.Random.Range(1, _gameManager.LevelConfig.gridSize.x - 1), UnityEngine.Random.Range(2, _gameManager.LevelConfig.gridSize.y - 1));
                        stateController.Obstacle = Vector3.zero;
                    }
                }
                else
                {
                    stateController.Target = stateController.Path[0].position + new Vector3(0.5f, 0.5f);
                    Vector3 dirVector = stateController.Target - stateController.Machine.transform.position;
                    float distance = Vector3.Distance(stateController.Target, stateController.Machine.transform.position);
                    if (distance > .5f)
                    {
                        stateController.Machine.Move(dirVector.normalized);
                    }
                    else
                    {
                        // stateController.Target = Vector3.zero;
                        // stateController.Machine.OnSetTarget(null);
                        stateController.Path.RemoveAt(0);
                    }
                }
                // }
            }

        }
        else
        {
            timeWait -= Time.deltaTime;
            stateController.Machine.Stop();
        }
    }

    public override void OnExit()
    {
        base.OnExit();

        stateController.Machine.Stop();

        stateController.Target = Vector3.zero;
    }
}
