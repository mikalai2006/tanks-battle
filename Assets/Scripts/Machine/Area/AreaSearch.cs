using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class AreaSearch : MonoBehaviour
{
    GameManager _gameManager => GameManager.Instance;
    private MeshRenderer meshRenderer;
    [SerializeField] GPUInstanceEnabler gPUInstanceEnabler;
    [SerializeField] BaseMachine Machine;
    [SerializeField] Dictionary<BaseMachine, AreaSearchData> targets;
    [Tooltip("Какие слои искать, при проверке видим ли объект")]
    public LayerMask findLayers;
    public Dictionary<BaseMachine, AreaSearchData> Targets => targets;
    public List<AreaSearchData> testTargets;
    private float distanceSearch = 0;
    public float DistanceSearch => distanceSearch;
    private CancellationTokenSource cancelTokenSource;

#region Unity methods
    void Awake()
    {
        cancelTokenSource = new CancellationTokenSource();

        targets = new();
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.enabled = false;
        Machine = GetComponentInParent<BaseMachine>();
    }

    private void OnDestroy()
    {
        targets.Clear();

        if (!cancelTokenSource.Token.IsCancellationRequested)
        {
        cancelTokenSource.Cancel();
        cancelTokenSource.Dispose();
        }
    }

    async UniTask RefreshTargets(CancellationToken token)
    {
        while(targets.Count > 0 && !token.IsCancellationRequested) {
            for (int k = 0; k < targets.Count; k++)
            {
                KeyValuePair<BaseMachine, AreaSearchData> data = targets.ElementAt(k);

                data.Value.distance = Vector3.Distance(transform.position, data.Key.transform.position);

                if (!data.Value.isInArea)
                {
                    continue;
                }

                if (data.Value.isVisible)
                {
                    data.Value.timeView += Time.fixedDeltaTime;
                } else
                {
                    // data.Value.distance = 0;
                    data.Value.timeView = 0;
                }
                
                // float offsetRay = _baseMachine.AreaMove.transform.localScale.x;
                var direction = data.Key.transform.position - transform.position;
                Vector3 startRay = transform.position + (direction.normalized * 0.5f); // + offsetRay * direction.normalized;
                // RaycastHit[] hits = Physics.RaycastAll(startRay, direction, Mathf.RoundToInt(distanceSearch), findLayers);

                // bool isObstacle = false;
                // for (int i = 0; i < hits.Length; i++)
                // {
                //     if (!Machine.MachineLevelData.isBot)
                // {
                //     Debug.DrawRay(startRay, direction, Color.red, 2);
                // }
                    if (Physics.Raycast(startRay, direction,  out RaycastHit hit, Mathf.RoundToInt(distanceSearch), findLayers)) {
                    // Debug.Log($"hit.collider={hit.collider.name}, {hit.collider.gameObject == data.Key.AreaMoveGameObject.gameObject}, {hit.collider.gameObject == Machine.AreaMoveGameObject.gameObject}");

                    // AreaSearch isColliderAreaSearch = hit.collider.GetComponent<AreaSearch>();
                    // AreaMove isColliderAreaMove = hit.collider.GetComponent<AreaMove>();
                    // BaseBullet isBullet = hit.collider.GetComponent<BaseBullet>();
                    // // игнорируем снаряды
                    // if (isBullet)
                    // {
                    //     continue;
                    // }

                    // // игнорируем проверку зоны поиска
                    // if (isColliderAreaSearch)
                    // {
                    //     continue;
                    // }

                    // игнорируем свою зону поиска
                    if (hit.collider.gameObject == Machine.AreaMoveGameObject.gameObject)
                    {
                        continue;
                    }
                    
                    // // если есть препятствие тайл, выходим и устанавливаем препятствие
                    // if (hit.collider.CompareTag("TilemapWithCollider"))
                    // {
                    //     isObstacle = true;
                    //     break;
                    // }
                    // если нашли коллайдер который искали выходим
                    if (hit.collider.gameObject == data.Key.AreaMoveGameObject.gameObject)
                    {
                        SetVisibleMachine(data.Key, true);
                        // break;
                    }
                    // иначе если коллайдер другой машины, устанавливаем его как препятствие
                    else
                    {
                        SetVisibleMachine(data.Key, false);
                        // isObstacle = true;
                        // break;
                    }
                }
            // if (!isObstacle)
            // {
            //     // Debug.DrawRay(startRay, direction, Color.green);
            //     OnChangeStatusMachine(data.Key, true);
            // }
            // else
            // {
            //     // Debug.DrawRay(startRay, direction, Color.red);
            //     // OnRemoveMachine(_baseMachine);
            //     OnChangeStatusMachine(data.Key, false);
            // }

            }

            
            await UniTask.Delay(System.TimeSpan.FromSeconds(0.60f), cancellationToken: token);
        }
    }

    // void FixedUpdate()
    // {
    //     // for (int i = 0; i < targets.Count; i++)
    //     for (int k = 0; k < targets.Count; k++)
    //     {
    //         KeyValuePair<BaseMachine, AreaSearchData> data = targets.ElementAt(k);

    //         data.Value.distance = Vector3.Distance(transform.position, data.Key.transform.position);

    //         if (!data.Value.isInArea)
    //         {
    //             continue;
    //         }

    //         if (data.Value.isVisible)
    //         {
    //             data.Value.timeView += Time.fixedDeltaTime;
    //         } else
    //         {
    //             // data.Value.distance = 0;
    //             data.Value.timeView = 0;
    //         }
            
    //         // float offsetRay = _baseMachine.AreaMove.transform.localScale.x;
    //         var direction = data.Key.transform.position - transform.position;
    //         Vector3 startRay = transform.position + (direction.normalized * 0.5f); // + offsetRay * direction.normalized;
    //         // RaycastHit[] hits = Physics.RaycastAll(startRay, direction, Mathf.RoundToInt(distanceSearch), findLayers);

    //         // bool isObstacle = false;
    //         // for (int i = 0; i < hits.Length; i++)
    //         // {
    //         //     if (!Machine.MachineLevelData.isBot)
    //         // {
    //         //     Debug.DrawRay(startRay, direction, Color.red, 2);
    //         // }
    //             if (Physics.Raycast(startRay, direction,  out RaycastHit hit, Mathf.RoundToInt(distanceSearch), findLayers)) {
    //             // Debug.Log($"hit.collider={hit.collider.name}, {hit.collider.gameObject == data.Key.AreaMoveGameObject.gameObject}, {hit.collider.gameObject == Machine.AreaMoveGameObject.gameObject}");

    //             // AreaSearch isColliderAreaSearch = hit.collider.GetComponent<AreaSearch>();
    //             // AreaMove isColliderAreaMove = hit.collider.GetComponent<AreaMove>();
    //             // BaseBullet isBullet = hit.collider.GetComponent<BaseBullet>();
    //             // // игнорируем снаряды
    //             // if (isBullet)
    //             // {
    //             //     continue;
    //             // }

    //             // // игнорируем проверку зоны поиска
    //             // if (isColliderAreaSearch)
    //             // {
    //             //     continue;
    //             // }

    //             // игнорируем свою зону поиска
    //             if (hit.collider.gameObject == Machine.AreaMoveGameObject.gameObject)
    //             {
    //                 continue;
    //             }
                
    //             // // если есть препятствие тайл, выходим и устанавливаем препятствие
    //             // if (hit.collider.CompareTag("TilemapWithCollider"))
    //             // {
    //             //     isObstacle = true;
    //             //     break;
    //             // }
    //             // если нашли коллайдер который искали выходим
    //             if (hit.collider.gameObject == data.Key.AreaMoveGameObject.gameObject)
    //             {
    //                 SetVisibleMachine(data.Key, true);
    //                 // break;
    //             }
    //             // иначе если коллайдер другой машины, устанавливаем его как препятствие
    //             else
    //             {
    //                 SetVisibleMachine(data.Key, false);
    //                 // isObstacle = true;
    //                 // break;
    //             }
    //         }
    //     // if (!isObstacle)
    //     // {
    //     //     // Debug.DrawRay(startRay, direction, Color.green);
    //     //     OnChangeStatusMachine(data.Key, true);
    //     // }
    //     // else
    //     // {
    //     //     // Debug.DrawRay(startRay, direction, Color.red);
    //     //     // OnRemoveMachine(_baseMachine);
    //     //     OnChangeStatusMachine(data.Key, false);
    //     // }


    //     }

    //     // // проверяем наличие бонуса дистанции поиска.
    //     // machine.Data.bonuses.TryGetValue(TypeBonus.distanceSearch, out DataBonus bonusDistanceSearch);
    //     // if (bonusDistanceSearch != null)
    //     // {
    //     //     distanceSearch = machine.Config.distanceSearch * 2 + bonusDistanceSearch.value * 2;
    //     //     OnSetSize(distanceSearch);
    //     // }
    //     // else
    //     // {
    //     //     distanceSearch = machine.Config.distanceSearch * 2;
    //     //     OnSetSize(distanceSearch);
    //     // }
    // }

    void OnTriggerEnter(Collider collider)
    {
        // Debug.Log($"OnTriggerEnter AreaSearch: {collider.name}");
        var _baseMachine = collider.GetComponentInParent<BaseMachine>();

        if (_baseMachine == null || _baseMachine == Machine)
        {
            return;
        }

        SetStatusInAreaMachine(_baseMachine, true);

        // var isColliderTarget = collider.GetComponent<AreaSearch>();

        // if (isColliderTarget != null)
        // {
        //     return;
        // }

        // // float offsetRay = _baseMachine.AreaMove.transform.localScale.x;
        // var direction = _baseMachine.transform.position - transform.position;
        // Vector3 startRay = transform.position;// + offsetRay * direction.normalized;
        // RaycastHit2D[] hits = Physics2D.RaycastAll(startRay, direction, ignoreLayers, Mathf.RoundToInt(distanceSearch));

        // bool isObstacle = false;
        // for (int i = 0; i < hits.Length; i++)
        // {
        //     RaycastHit2D hit = hits[i];

        //     AreaSearch isColliderAreaSearch = hit.collider.GetComponent<AreaSearch>();
        //     AreaMove isColliderAreaMove = hit.collider.GetComponent<AreaMove>();
        //     BaseBullet isBullet = hit.collider.GetComponent<BaseBullet>();
        //     // игнорируем снаряды
        //     if (isBullet)
        //     {
        //         continue;
        //     }

        //     // игнорируем проверку зоны поиска
        //     if (isColliderAreaSearch)
        //     {
        //         continue;
        //     }

        //     // игнорируем свою зону поиска
        //     if (isColliderAreaMove == Machine.AreaMove)
        //     {
        //         continue;
        //     }
            
        //     // // если есть препятствие тайл, выходим и устанавливаем препятствие
        //     // if (hit.collider.CompareTag("TilemapWithCollider"))
        //     // {
        //     //     isObstacle = true;
        //     //     break;
        //     // }
        //     // если нашли коллайдер который искали выходим
        //     if (_baseMachine.AreaMove == isColliderAreaMove)
        //     {
        //         break;
        //     }
        //     // иначе если коллайдер другой машины, устанавливаем его как препятствие
        //     else
        //     {
        //         isObstacle = true;
        //         break;
        //     }
        // }
        // if (!isObstacle)
        // {
        //     // Debug.DrawRay(startRay, direction, Color.green);
        //     OnChangeStatusMachine(_baseMachine, true);
        // }
        // else
        // {
        //     // Debug.DrawRay(startRay, direction, Color.red);
        //     // OnRemoveMachine(_baseMachine);
        //     OnChangeStatusMachine(_baseMachine, false);
        // }
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

    void OnTriggerExit(Collider collider)
    {
        var _baseMachine = collider.GetComponentInParent<BaseMachine>();
        // OnRemoveMachine(_baseMachine);
        if (_baseMachine != null)
        {
            SetStatusInAreaMachine(_baseMachine, false);
            SetVisibleMachine(_baseMachine, false);
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

        for (int i = 0; i < Machine.LevelManager.machines.Count; i++)
        {
            if (Machine.LevelManager.machines.ElementAt(i) != Machine)
            {
                targets.Add(Machine.LevelManager.machines[i], new AreaSearchData{
                    areaSearch = Machine.LevelManager.machines[i].AreaSearch
                });
            }
        }

        // Debug.LogWarning($"Создаем список всех машин в AreaSearch ({targets.Count})");
        // }

        // Test.
        testTargets = Targets.Values.ToList();

        RefreshTargets(cancelTokenSource.Token).Forget();
    }

    public void OnSetSize(float _size)
    {
        float size = _size; // * (1 /_gameManager.Settings.scaleObjects);
        transform.localScale = new Vector3(size, 0.1f, size); // Vector3.Lerp(transform.localScale, new Vector3(size, 0.1f, size), _gameManager.Settings.speedChangeAreaSize * Time.deltaTime);
    }

    private void SetVisibleMachine(BaseMachine _machine, bool status)
    {
        if (_machine == null)
        {
            return;
        }

        if (targets.ContainsKey(_machine))
        {
            targets[_machine].isVisible = status;
        }
    }

    private void SetStatusInAreaMachine(BaseMachine _machine, bool status)
    {
        if (_machine == null)
        {
            return;
        }

        // float distance = Vector2.Distance(machine.transform.position, _machine.transform.position);
        if (targets.ContainsKey(_machine)) //  && distance <= machine.Config.distanceAttack - 1
        {
            targets[_machine].timeView = 0;
            targets[_machine].isInArea = status;
            // targets[_machine].distance = 0;
            targets[_machine].areaSearch = _machine.AreaSearch;
        } else
        {
            Debug.LogWarning($"Не найдена машина {_machine.name} в списке!");
        }
    }
}
