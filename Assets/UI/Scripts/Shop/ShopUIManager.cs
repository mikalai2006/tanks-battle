using System.Collections.Generic;
using UIToolkitLibrary;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShopUIManager : MonoBehaviour
{
    GameManager gameManager => GameManager.Instance;
    [SerializeField] private Transform Wrapper;
    [SerializeField] private Camera Camera;
    [SerializeField] private CinemachineCamera CinemachineCamera;
    [SerializeField] private List<GameMachine> machinesConfigs;
    List<BaseMachine> machinesGameObjects;
    int activeIndexMachine = 0;
    [SerializeField] private InputActionReference mousePositionAction;
    [SerializeField] private InputActionReference clickAction;

    void Start()
    {
        machinesGameObjects = new List<BaseMachine>();

        gameManager.SetActiveCamera(Camera);

        Init();

        ShopUIEvents.ClickButtonNextInShop += OnNextMachine;
        ShopUIEvents.ClickButtonPrevInShop += OnPrevMachine;
        ShopUIEvents.ClickButtonBuyInShop += OnBuyMachine;
    }

    void OnDestroy()
    {
        ShopUIEvents.ClickButtonNextInShop -= OnNextMachine;
        ShopUIEvents.ClickButtonPrevInShop -= OnPrevMachine;
        ShopUIEvents.ClickButtonBuyInShop -= OnBuyMachine;
    }

    void OnEnable()
    {
        clickAction.action.Enable();
        mousePositionAction.action.Enable();
        // Subscribe to the performed event of the click action
    }

    void OnDisable()
    {
        clickAction.action.Disable();
        mousePositionAction.action.Disable();
    }

    private void OnNextMachine()
    {
        activeIndexMachine = Mathf.Min(machinesGameObjects.Count - 1, activeIndexMachine + 1);

        OnFocusMachineByIndex(activeIndexMachine);

        ShopUIEvents.FocusMachineInShop.Invoke(machinesConfigs[activeIndexMachine]);
    }

    private void OnBuyMachine()
    {
        GameMachine configMachine = machinesConfigs[activeIndexMachine];
        
        gameManager.StateManager.BuyMachine(configMachine);
    }

    private void OnPrevMachine()
    {
        activeIndexMachine = Mathf.Max(0, activeIndexMachine - 1);
        
        OnFocusMachineByIndex(activeIndexMachine);

        ShopUIEvents.FocusMachineInShop.Invoke(machinesConfigs[activeIndexMachine]);
    }

    private void Init()
    {
        // gameManager.StateManager.AddMachine("T3");
        int index = 0;

        if (gameManager?.Settings != null)
        {
            machinesConfigs.AddRange(gameManager.Settings.machines);
        }

        foreach (var item in machinesConfigs)
        {
            GameMachine configMachine = machinesConfigs.Find(m => m.name == item.name);

            CreateMachine(configMachine, index, new MachineLevelData
            {
                id = item.name,
            });

            index ++;
        }

        
        CinemachineBrain brain = Camera.GetComponent<CinemachineBrain>();
        if (brain != null && machinesGameObjects.Count > 0)
        {
            OnFocusMachineByIndex(activeIndexMachine);
        }
    }

    void OnFocusMachineByIndex(int index = 0)
    {
        index = Mathf.Clamp(index, 0, machinesConfigs.Count - 1);

        CinemachineCamera.Follow = machinesGameObjects[index].objectTargetCamera.transform;
        CinemachineCamera.LookAt = machinesGameObjects[index].objectTargetCamera.transform;
        
        CinemachineOrbitalFollow cinemachineOrbitalFollow = CinemachineCamera.GetComponent<CinemachineOrbitalFollow>();
        if (cinemachineOrbitalFollow)
        {
            cinemachineOrbitalFollow.HorizontalAxis.Value = 150;
            cinemachineOrbitalFollow.VerticalAxis.Value = 15;
        }
    }

    void CreateMachine(GameMachine configMachine, int index, MachineLevelData data)
    {
        var gObject = Instantiate(
            configMachine.machinePrefab,
            new Vector3(0, 0, -index),
            Quaternion.identity,
            Wrapper
        );

        BaseMachine obj = gObject.GetComponent<BaseMachine>();
        if (obj != null)
        {
            machinesGameObjects.Add(obj);

            obj.GetComponent<PlayerController>().enabled = false;
            obj.GetComponent<PlayerInput>().enabled = false;
            obj.AreaSearch.gameObject.SetActive(false);
            // obj.GetComponentInChildren<NavMeshAgent>().enabled = false;
            // var lightComponent = obj.GetComponentInChildren<Light>();
            // if (lightComponent)
            // {
            //     lightComponent.enabled = true;
            // }
            // obj.Areol.SetActive(true);
            // obj.GetComponent<CameraFollow>().enabled = false;
            // obj.GetComponent<CameraFollowFPS>().enabled = false;
            obj.GetComponent<StateController>().enabled = false;
            obj.GetComponentInChildren<HealthBarController>().gameObject.SetActive(false);

            // CameraHandler.OnSetCharacter(obj);
            obj.Init(configMachine, data);

            obj.Body.OnSetAngleBody(45);
        }

    }
}
