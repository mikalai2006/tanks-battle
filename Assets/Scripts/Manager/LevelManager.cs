using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

public class LevelManager : MonoBehaviour
{
    private GameManager _gameManager => GameManager.Instance;
    private GameSetting _gameSetting => GameManager.Instance.Settings;
    [SerializeField] public MapManager mapManager;
    [SerializeField] public Light2D globalLight;
    [SerializeField] public List<BaseMachine> machines;
    [SerializeField] public List<IndicatorMachine> indicators;
    [SerializeField] public List<BaseBonus> bonuses;
    [SerializeField] public GameObject objectSpawnMachines;
    [SerializeField] public GameObject objectSpawnEffect;
    [SerializeField] public GameObject objectSpawnBonuses;
    [SerializeField] public GameObject objectSpawnText;
    [SerializeField] public GameObject objectSpawnIndicators;
    [SerializeField] public UITopSide UiTopSide;
    
    [SerializeField] Camera _camera;
    public Camera Camera => _camera;

    void Start()
    {
        _camera = GameObject.FindGameObjectWithTag("CameraGame").GetComponent<Camera>();

        globalLight.intensity = _gameManager.LevelConfig.light;

        // создаем карту
        mapManager.CreateMap();

        // создаем игровые комманды
        if (_gameManager.LevelConfig.typeLevel == TypeLevel.Command)
        {
            _gameManager.StateManager.InitDataCommandLevel();

            // // проходим по коммандам и создаем игроков
            // foreach (TeamData team in _gameManager.StateManager.stateLevel.teams)
            // {
            //     for (int i = 0; i < _gameManager.LevelConfig.countPlayers; i++)
            //     {
            //         GridTileNode node = mapManager.gridTileHelper.GetAllGridNodes().Where(n =>
            //             !n.OccupiedUnit
            //             && n.StateNode.HasFlag(StateNode.Empty)
            //             && !n.StateNode.HasFlag(StateNode.Disable)
            //         ).OrderBy(t => UnityEngine.Random.value).First();

            //         if (node != null)
            //         {
            //             bool bot = true;

            //             GameMachine configMachine = _gameSetting.machines[UnityEngine.Random.Range(0, _gameSetting.machines.Count - 1)];

            //             if (team.index == 0 && i == 0)
            //             {
            //                 bot = false;
            //             }

            //             Addressables.InstantiateAsync(
            //                 configMachine.machinePrefab,
            //                 node.position,
            //                 Quaternion.identity,
            //                 objectSpawnMachines.transform
            //             ).Completed += (AsyncOperationHandle<GameObject> handle) => LoadedAsset(handle, configMachine, data);
            //         }
            //     }
            //     //team.machines.Add(obj);
            // }
        }
        else if (_gameManager.LevelConfig.typeLevel == TypeLevel.Alone)
        {
            _gameManager.StateManager.InitDataAloneLevel();

            // создаем игроков
            foreach (MachineLevelData data in _gameManager.StateManager.stateLevel.machines)
            {

                GridTileNode node = mapManager.gridTileHelper.GetAllGridNodes().Where(n =>
                    // !n.OccupiedUnit
                    // && n.StateNode.HasFlag(StateNode.Empty)
                    n.X > 1
                    && n.X < _gameManager.LevelConfig.gridSize.x
                    && n.Y > 1
                    && n.Y < _gameManager.LevelConfig.gridSize.y
                    && !n.StateNode.HasFlag(StateNode.Disable)
                ).OrderBy(t => UnityEngine.Random.value).First();

                if (node != null)
                {
                    GameMachine configMachine = _gameSetting.machines.Find(m => m.name == data.id);

                    // Addressables.InstantiateAsync
                    var gObject = Instantiate(
                        configMachine.machinePrefab,
                        node.position,
                        Quaternion.identity,
                        objectSpawnMachines.transform
                    );

                    BaseMachine obj = gObject.GetComponent<BaseMachine>();
                    if (obj != null)
                    {
                        // Debug.Log($"load {obj.name}/{team.index}");
                        if (data.isBot)
                        {
                            obj.GetComponent<PlayerController>().enabled = false;
                            obj.GetComponent<PlayerInput>().enabled = false;
                            obj.GetComponentInChildren<Light2D>().enabled = false;
                            obj.Areol.SetActive(false);
                            obj.GetComponent<CameraFollow>().enabled = false;
                            obj.GetComponent<StateController>().enabled = true;
                            obj.OnSetConfig(configMachine, data);
                            obj.SetOccupiedNode(node);
                            // obj.transform.position = _transform;
                        }
                        else
                        {
                            obj.GetComponent<PlayerController>().enabled = true;
                            obj.GetComponent<PlayerInput>().enabled = true;
                            obj.GetComponentInChildren<Light2D>().enabled = true;
                            obj.Areol.SetActive(true);
                            obj.GetComponent<CameraFollow>().enabled = true;
                            obj.GetComponent<StateController>().enabled = false;
                            obj.OnSetConfig(configMachine, data);
                            obj.SetOccupiedNode(node);
                        }

                        machines.Add(obj);
                        // team.machines.Add(obj);
                    }

                    IndicatorMachine indicatorObject = Instantiate(
                        configMachine.indicatorPrefab,
                        Vector3.zero,
                        Quaternion.identity,
                        objectSpawnIndicators.transform
                    );
                    if (indicatorObject != null)
                    {
                        obj.OnSetIndicator(indicatorObject);
                        indicatorObject.OnSetMachine(obj);
                        OnAddIndicator(indicatorObject);
                    }


                    //.Completed += (AsyncOperationHandle<GameObject> handle) => LoadedAsset(handle, configMachine, data, node);
                }
            }
        }

        // установка настроек для индикаторов машин на карте.
        BaseMachine targetIndicator = machines.Find(m => !m.MachineLevelData.isBot);
        if (targetIndicator != null)
        {
            for (int i = 0; i < indicators.Count; i++)
            {
                IndicatorMachine ind = indicators[i].GetComponentInChildren<IndicatorMachine>();
                if (ind != null)
                {
                    ind.OnSetTarget(targetIndicator);
                }
            }
        }

        // spawn bonuses.
        List<GridTileNode> vacantNodes = mapManager.gridTileHelper.GetEmptyNodes().OrderBy(t => UnityEngine.Random.value).ToList();
        for (int i = 0; i < 15; i++)
        {
            GameBonus configB = Helpers.GetProbabilityItem<GameBonus>(_gameManager.LevelConfig.bonuses).Item;
            OnSpawnBonus(vacantNodes[i], configB);
        }

    }

    public void OnRemoveMachine(BaseMachine _mach)
    {
        OnRemoveIndicator(_mach.Indicator);

        if (machines.Contains(_mach))
        {
            machines.Remove(_mach);
        }
    }

    public void OnAddIndicator(IndicatorMachine im)
    {
        if (!indicators.Contains(im))
        {
            indicators.Add(im);
        }
    }

    public void OnRemoveIndicator(IndicatorMachine im)
    {
        if (indicators.Contains(im))
        {
            im.DestroyGameObject();
            indicators.Remove(im);
        }
    }

    public void OnSpawnBonus(GridTileNode node, GameBonus configBonus)
    {
        var gObject = Instantiate(
            configBonus.prefabMap,
            node.position,
            Quaternion.identity,
            objectSpawnBonuses.transform
        );

        BaseBonus obj = gObject.GetComponent<BaseBonus>();
        obj.Init(configBonus);
    }

//     public void LoadedAsset(AsyncOperationHandle<GameObject> handle, GameMachine configMachine, MachineLevelData data, GridTileNode node)
    //     {
    //         if (handle.Status == AsyncOperationStatus.Succeeded)
    //         {
    //             BaseMachine obj = handle.Result.GetComponent<BaseMachine>();
    //             if (obj != null)
    //             {
    //                 // Debug.Log($"load {obj.name}/{team.index}");
    //                 if (data.isBot)
    //                 {
    //                     obj.GetComponent<PlayerController>().enabled = false;
    //                     obj.GetComponent<PlayerInput>().enabled = false;
    //                     obj.GetComponentInChildren<Light2D>().enabled = false;
    //                     obj.Areol.SetActive(false);
    //                     obj.GetComponent<CameraFollow>().enabled = false;
    //                     obj.GetComponent<StateController>().enabled = true;
    //                     obj.OnSetConfig(configMachine, data);
    //                     obj.SetOccupiedNode(node);
    //                     // obj.transform.position = _transform;
    //                 }
    //                 else
    //                 {
    //                     obj.GetComponent<PlayerController>().enabled = true;
    //                     obj.GetComponent<PlayerInput>().enabled = true;
    //                     obj.GetComponentInChildren<Light2D>().enabled = true;
    //                     obj.Areol.SetActive(true);
    //                     obj.GetComponent<CameraFollow>().enabled = true;
    //                     obj.GetComponent<StateController>().enabled = false;
    //                     obj.OnSetConfig(configMachine, data);
    //                     obj.SetOccupiedNode(node);
    //                 }

    //                 // machines.Add(obj);
    //                 // team.machines.Add(obj);
    //             }
    //         }
    //         else
    //         {
    //             Debug.LogError($"Error Load prefab::: {handle.Status}");
    //         }
    //     }

}
