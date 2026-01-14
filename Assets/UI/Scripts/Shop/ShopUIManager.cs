using System.Collections.Generic;
using UIToolkitLibrary;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class ShopUIManager : MonoBehaviour
{
    GameManager gameManager => GameManager.Instance;
    [SerializeField] private Transform Wrapper;
    [SerializeField] private Camera Camera;
    [SerializeField] private CinemachineCamera CinemachineCamera;
    [SerializeField] private List<GameMachine> machinesConfigs;
    List<UIShopItemView> shopItemGameObjects;
    [SerializeField] private CinemachineInputAxisController CinemachineCameraInputController;
    int activeIndexMachine = 0;
    public InputActionReference mousePositionAction;
    [SerializeField] private InputActionReference clickAction;

    // // Assign in the Inspector:
    // public UIManager uIManager;
    // public Camera mainCamera; // The main camera in the scene
    // public Camera renderTextureCamera; // The camera rendering to the RenderTexture

    private System.Func<Vector2, Vector2> m_DefaultScreenToPanelSpaceFunction;

    // void Awake()
    // {
    //     uIManager = GameObject.FindGameObjectWithTag("UIManager")?.GetComponent<UIManager>();

    //     mainCamera = GameObject.FindGameObjectWithTag("MainCamera")?.GetComponent<Camera>();
    //     renderTextureCamera = GameObject.FindGameObjectWithTag("SecondCamera")?.GetComponent<Camera>();
    // }


    void Start()
    {
        shopItemGameObjects = new List<UIShopItemView>();

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

        clickAction.action.started += OnStartTouch;
        clickAction.action.canceled += OnEndTouch;
    }

    void OnDisable()
    {
        clickAction.action.Disable();
        mousePositionAction.action.Disable();

        clickAction.action.started -= OnStartTouch;
        clickAction.action.canceled -= OnEndTouch;
    }

    private void OnToggleInputCamera()
    {
        CinemachineCameraInputController.enabled = !CinemachineCameraInputController.enabled;
    }

    private void OnNextMachine()
    {
        activeIndexMachine = Mathf.Min(shopItemGameObjects.Count - 1, activeIndexMachine + 1);

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

    private void OnStartTouch(InputAction.CallbackContext context)
    {
        OnToggleInputCamera();
    }

    private void OnEndTouch(InputAction.CallbackContext context)
    {
        OnToggleInputCamera();
    }

    private void Init()
    {
        // gameManager.StateManager.AddMachine("T3");
        int index = 0;

        if (gameManager?.Settings != null)
        {
            machinesConfigs.AddRange(gameManager.Settings.machines);
        }

        List<Vector2> pointSpiral = Helpers.GenerateArchimedeanSpiral(10, machinesConfigs.Count);

        foreach (var item in machinesConfigs)
        {
            GameMachine configMachine = machinesConfigs.Find(m => m.name == item.name);

            CreateShopItem(configMachine, pointSpiral[index], new MachineLevelData
            {
                id = item.name,
                data = new StateMachinePlayerData(),
            });

            index ++;
        }

        
        CinemachineBrain brain = Camera.GetComponent<CinemachineBrain>();
        if (brain != null && shopItemGameObjects.Count > 0)
        {
            OnFocusMachineByIndex(activeIndexMachine);
        }
    }

    void OnFocusMachineByIndex(int index = 0)
    {
        index = Mathf.Clamp(index, 0, machinesConfigs.Count - 1);

        CinemachineCamera.Follow = shopItemGameObjects[index].ObjectTargetCamera.transform;
        CinemachineCamera.LookAt = shopItemGameObjects[index].ObjectTargetCamera.transform;
        
        CinemachineOrbitalFollow cinemachineOrbitalFollow = CinemachineCamera.GetComponent<CinemachineOrbitalFollow>();
        if (cinemachineOrbitalFollow)
        {
            cinemachineOrbitalFollow.HorizontalAxis.Value = 150;
            cinemachineOrbitalFollow.VerticalAxis.Value = 15;
        }
    }

    void CreateShopItem(GameMachine configMachine, Vector2 point, MachineLevelData data)
    {
        var gObject = Instantiate(
            gameManager.Settings.prefabShopItemMachine,
            new Vector3(-point.x, 0, -point.y),
            Quaternion.identity,
            Wrapper
        );

        UIShopItemView obj = gObject.GetComponent<UIShopItemView>();
        if (obj != null)
        {
            shopItemGameObjects.Add(obj);

            obj.Init(configMachine, data);
        }

        // targetPanelSettings = obj.GetComponentInChildren<UIDocument>().panelSettings;
        // if (targetPanelSettings != null)
        // {
        //     targetPanelSettings.SetScreenToPanelSpaceFunction(ScreenCoordinatesToRenderTexture);
        //     Debug.Log($"Found settings!");
        // } else
        // {
        //     Debug.LogWarning($"Not found settings!");
        // }

    }
}
