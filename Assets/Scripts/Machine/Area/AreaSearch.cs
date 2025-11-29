using System.Collections.Generic;
using UnityEngine;

public class AreaSearch : MonoBehaviour
{
    GameManager _gameManager => GameManager.Instance;
    private MeshRenderer meshRenderer;
    [SerializeField] GPUInstanceEnabler gPUInstanceEnabler;
    [SerializeField] BaseMachine machine;
    [SerializeField] Dictionary<BaseMachine, AreaSearchData> targets;
    public Dictionary<BaseMachine, AreaSearchData> Targets => targets;
    // public List<AreaSearchData> testTargets;
    private float distanceSearch = 0;
    public float DistanceSearch => distanceSearch;

#region Unity methods
    void Awake()
    {
        targets = new();
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.enabled = false;
        machine = GetComponentInParent<BaseMachine>();
    }

    void FixedUpdate()
    {
        
        // for (int i = 0; i < targets.Count; i++)
        foreach (KeyValuePair<BaseMachine, AreaSearchData> data in targets)
        {
            // AreaSearchData areaSearchData = targets[targets.ElementAt(i).Key];
            // areaSearchData.timeView += Time.deltaTime;
            // targets[targets.ElementAt(i).Key] = areaSearchData;
            if (data.Value.isVisible)
            {
                data.Value.timeView += Time.fixedDeltaTime;
                data.Value.distance = Vector3.Distance(transform.position, machine.transform.position);
            } else
            {
                data.Value.distance = 0;
                data.Value.timeView = 0;
            }
        }

        // // проверяем наличие бонуса дистанции поиска.
        // machine.Data.bonuses.TryGetValue(TypeBonus.distanceSearch, out DataBonus bonusDistanceSearch);
        // if (bonusDistanceSearch != null)
        // {
        //     distanceSearch = machine.Config.distanceSearch * 2 + bonusDistanceSearch.value * 2;
        //     OnSetSize(distanceSearch);
        // }
        // else
        // {
        //     distanceSearch = machine.Config.distanceSearch * 2;
        //     OnSetSize(distanceSearch);
        // }
    }

    void OnTriggerEnter(Collider collider)
    {
        // Debug.Log($"OnTriggerEnter AreaSearch: {collider.name}");
        var _baseMachine = collider.GetComponentInParent<BaseMachine>();

        if (_baseMachine != null)
        {
            var isColliderTarget = collider.GetComponent<AreaSearch>();

            // if (_baseMachine.AreaMove != collider && _baseMachine.AreaAttack != collider)
            if (_baseMachine != machine && !isColliderTarget)
            {
                // float offsetRay = _baseMachine.AreaMove.transform.localScale.x;
                var direction = _baseMachine.transform.position - transform.position;
                Vector3 startRay = transform.position;// + offsetRay * direction.normalized;
                RaycastHit2D[] hits = Physics2D.RaycastAll(startRay, direction, distanceSearch);

                bool isObstacle = false;
                for (int i = 0; i < hits.Length; i++)
                {
                    RaycastHit2D hit = hits[i];

                    AreaSearch isColliderAreaSearch = hit.collider.GetComponent<AreaSearch>();
                    AreaMove isColliderAreaMove = hit.collider.GetComponent<AreaMove>();
                    BaseBullet isBullet = hit.collider.GetComponent<BaseBullet>();
                    // игнорируем снаряды
                    if (isBullet)
                    {
                        continue;
                    }

                    // игнорируем проверку зоны поиска
                    if (isColliderAreaSearch)
                    {
                        continue;
                    }
                    // игнорируем свою зону поиска
                    if (isColliderAreaMove == machine.AreaMove)
                    {
                        continue;
                    }
                    // если есть препятствие тайл, выходим и устанавливаем препятствие
                    if (hit.collider.CompareTag("TilemapWithCollider"))
                    {
                        isObstacle = true;
                        break;
                    }
                    // если нашли коллайдер который искали выходим
                    if (_baseMachine.AreaMove == isColliderAreaMove)
                    {
                        break;
                    }
                    // иначе если коллайдер другой машины, устанавливаем его как препятствие
                    else
                    {
                        isObstacle = true;
                        break;
                    }
                }
                if (!isObstacle)
                {
                    // Debug.DrawRay(startRay, direction, Color.green);
                    OnChangeStatusMachine(_baseMachine, true);
                }
                else
                {
                    // Debug.DrawRay(startRay, direction, Color.red);
                    // OnRemoveMachine(_baseMachine);
                    OnChangeStatusMachine(_baseMachine, false);
                }
                // Tilemap tm = hit.collider.GetComponent<Tilemap>();
                // if (tm != null) {
                //     Debug.Log($"Point2: {hit.point}");
                // }

                // if (hit.collider)
                // {
                //     BaseMachine bm = hit.collider.GetComponentInParent<BaseMachine>();
                //     AreaAttack isColliderAreaAttack = collider.GetComponent<AreaAttack>();
                //     AreaMove isColliderMyAreaMove = collider.GetComponent<AreaMove>();

                //     if (bm && !isColliderAreaAttack)
                //     {
                //         Debug.DrawRay(startRay, direction, Color.green);
                //         Debug.Log($"Trigger2:  baseMachine {_baseMachine.name}/ {hit}");
                //     }
                // }
                // // machine.stateController.patrolState.OnSetEnemy(_baseMachine);
                // machine.stateController.patrolState.OnSetObstacle(collision.collider.gameObject.transform.position);
                // // machine.stateController.ChangeState(machine.stateController.chaseState);
            }

        }
    }

    void OnTriggerExit(Collider collider)
    {
        var _baseMachine = collider.GetComponentInParent<BaseMachine>();
        // OnRemoveMachine(_baseMachine);
        if (_baseMachine != null)
        {
            OnChangeStatusMachine(_baseMachine, false);
        }
    }
#endregion

    public void OnSetColor(Color color)
    {
        gPUInstanceEnabler.SetColor(color);
    }

    public void Init(GameMachine configMachine)
    {
        distanceSearch = configMachine.distanceSearch * 2;

        OnSetSize(distanceSearch);
    }

    /// <summary>
    /// Создание списка всех машин для последующего отслеживания данных
    /// обнаружение ближайших врагов, кто в зоне атаки.
    /// </summary>
    public void OnSynMachineList()
    {
        // if (targets.Count < machine.LevelManager.machines.Count)
        // {

        targets.Clear();

        for (int i = 0; i < machine.LevelManager.machines.Count; i++)
        {
            targets.Add(machine.LevelManager.machines[i], new AreaSearchData{});
        }

        Debug.LogWarning($"Создаем список всех машин в AreaSearch ({targets.Count})");
        // }

        // // Test.
        // testTargets = Targets.Values.ToList();
    }

    public void OnSetSize(float _size)
    {
        float size = _size; // * (1 /_gameManager.Settings.scaleObjects);
        transform.localScale = new Vector3(size, 0.1f, size); // Vector3.Lerp(transform.localScale, new Vector3(size, 0.1f, size), _gameManager.Settings.speedChangeAreaSize * Time.deltaTime);
    }

    private void OnChangeStatusMachine(BaseMachine _machine, bool status)
    {
        if (_machine != null)
        {
            // float distance = Vector2.Distance(machine.transform.position, _machine.transform.position);
            if (targets.ContainsKey(_machine)) //  && distance <= machine.Config.distanceAttack - 1
            {
                targets[_machine].timeView = 0;
                targets[_machine].isVisible = status;
                targets[_machine].distance = 0;
            } else
            {
                Debug.LogWarning($"Не найдена машина {_machine.name} в списке!");
            }
        }
    }


    // private void OnRemoveMachine(BaseMachine _machine)
    // {
    //     if (_machine != null)
    //     {
    //         if (targets.ContainsKey(_machine))
    //         {
    //             // targets[_machine] = 0;
    //             targets.Remove(_machine);
    //         }
    //     }
    // }
}
