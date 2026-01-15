using System;
using System.Collections.Generic;
using System.Linq;
using Mikalai2006.Voxel;
using UIToolkitLibrary;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class GarageUIManager : MonoBehaviour
{
    GameManager gameManager => GameManager.Instance;
    [SerializeField] private Transform Wrapper;
    [SerializeField] private Transform Wrapper3dModels;
    [SerializeField] private Camera Camera;
    [SerializeField] private CinemachineCamera CinemachineCamera;
    [SerializeField] private CinemachineInputAxisController CinemachineCameraInputController;
    // [SerializeField] private Camera CameraSecond;
    // [SerializeField] private CinemachineCamera CinemachineCameraSecond;
    // [SerializeField] private CinemachineInputAxisController CinemachineCameraSecondInputController;
    [SerializeField] private List<GameMachine> machinesConfigs;
    [SerializeField] List<BaseMachine> machinesGameObjects;
    [SerializeField] private int activeIndexMachine = 0;
    [SerializeField] private InputActionReference mousePositionAction;
    [SerializeField] private InputActionReference clickAction;
    [SerializeField] private InputActionReference actionCamera;
    [SerializeField] private ColorModifyItem activeColorModifyItem;
    List<GameTowerOption> dataTowers;
    List<UIGarageTowerItemView> towersGameObjects;
    [SerializeField] private int activeIndexTower = 0;
    [SerializeField] private float step = 0.1f;
    private DataDetail cacheTower;
    // CinemachineBrain brain;
    

    void OnEnable()
    {
        clickAction.action.Enable();
        mousePositionAction.action.Enable();
        
        clickAction.action.performed += OnClickPerformed;
        clickAction.action.started += OnStartTouch;
        clickAction.action.canceled += OnEndTouch;
    }

    void OnDisable()
    {
        clickAction.action.performed -= OnClickPerformed;
        clickAction.action.started -= OnStartTouch;
        clickAction.action.canceled -= OnEndTouch;

        clickAction.action.Disable();
        mousePositionAction.action.Disable();

        // OnClickButtonTowerClose();
    }

    void Start()
    {
        towersGameObjects = new ();

        dataTowers= new ();

        machinesGameObjects = new List<BaseMachine>();

        gameManager.SetActiveCamera(Camera);

        Init();
        
        // brain = Camera.GetComponent<CinemachineBrain>();

        GarageUIEvents.ClickButtonNextMachine += OnNextMachine;
        GarageUIEvents.ClickButtonPrevMachine += OnPrevMachine;
        GarageUIEvents.ClickButtonSellActiveMachine += OnSellActiveMachine;
        GarageUIEvents.ClickByColor += ChooseColorModify;
        GarageUIEvents.OpenColors += OnDisable;
        GarageUIEvents.CloseColors += OnEnable;
        GarageUIEvents.FillCancel += OnCancelColorModify;

        UIEvents.ClickButtonTower += OnClickButtonTower;
        UIEvents.ClickButtonPrevTower += OnClickButtonPrevTower;
        UIEvents.ClickButtonNextTower += OnClickButtonNextTower;
        UIEvents.ClickButtonTowerClose += OnClickButtonTowerClose;
    }

    void OnDestroy()
    {
        GarageUIEvents.ClickButtonNextMachine -= OnNextMachine;
        GarageUIEvents.ClickButtonPrevMachine -= OnPrevMachine;
        GarageUIEvents.ClickButtonSellActiveMachine -= OnSellActiveMachine;
        GarageUIEvents.ClickByColor -= ChooseColorModify;
        GarageUIEvents.OpenColors -= OnDisable;
        GarageUIEvents.CloseColors -= OnEnable;
        GarageUIEvents.FillCancel -= OnCancelColorModify;
        
        UIEvents.ClickButtonTower -= OnClickButtonTower;
        UIEvents.ClickButtonPrevTower -= OnClickButtonPrevTower;
        UIEvents.ClickButtonNextTower -= OnClickButtonNextTower;
        UIEvents.ClickButtonTowerClose -= OnClickButtonTowerClose;
    }

    private void OnClickButtonTowerClose()
    {
        int count = towersGameObjects.Count;
        for (int i = 0; i < count; i++)
        {
            if (towersGameObjects[i] != null)
            {
                Destroy(towersGameObjects[i].gameObject);
            }
        }

        towersGameObjects.Clear();

        ReDrawTower(-1);

        dataTowers.Clear();

        Wrapper3dModels.gameObject.SetActive(false);
    }

    private void OnClickButtonNextTower()
    {
        activeIndexTower += 1;
        activeIndexTower = Mathf.Clamp(activeIndexTower, 0, dataTowers.Count - 1);


        Wrapper3dModels.transform.localPosition = new Vector3(
            Wrapper3dModels.transform.localPosition.x,
            0.15f + activeIndexTower * step,
            Wrapper3dModels.transform.localPosition.z);

        UIEvents.FocusTower?.Invoke(dataTowers[activeIndexTower]);

        ReDrawTower(activeIndexTower);
    }

    private void OnClickButtonPrevTower()
    {
        activeIndexTower -= 1;

        activeIndexTower = Mathf.Clamp(activeIndexTower, 0, dataTowers.Count - 1);

        Wrapper3dModels.transform.localPosition = new Vector3(
            Wrapper3dModels.transform.localPosition.x,
            0.15f + activeIndexTower * step,
            Wrapper3dModels.transform.localPosition.z);

        UIEvents.FocusTower?.Invoke(dataTowers[activeIndexTower]);

        ReDrawTower(activeIndexTower);
    }

    void ReDrawTower(int indexTower)
    {
        var tower = machinesGameObjects[activeIndexMachine].Towers.Find(x => x.Parent == null);

        if (indexTower > -1)
        {
            DataDetail dataDetail = new DataDetail
            {
                nameConfig = dataTowers[indexTower].Config.name,
                offset = tower.DataDetailTower.offset,
                number = tower.DataDetailTower.number,
            };

            tower.ReDraw(dataTowers[indexTower], dataDetail);
        } else
        {
            if (cacheTower != null)
            {
                var conf = dataTowers.FirstOrDefault(x => x.Config.name == cacheTower.nameConfig);

                if (conf != null)
                {
                    // Debug.Log($"conf={conf.Config.name},cacheTower={cacheTower.nameConfig}");

                    tower.ReDraw(conf, cacheTower);
                } else
                {
                    Debug.LogWarning($"not found conf={conf},cacheTower={cacheTower.nameConfig}");
                }
            }
        }
        // machinesGameObjects[activeIndexMachine].CreateTower(dataTowers[activeIndexTower], dataDetail);
    }

    private void OnClickButtonTower()
    {
        var tower = machinesGameObjects[activeIndexMachine].Towers.Find(x => x.Parent == null);
        cacheTower = tower.DataDetailTower;

        CreateTowerItems();

        OnClickButtonPrevTower();
        
        Wrapper3dModels.gameObject.SetActive(true);
    }
    

    private void ChooseColorModify(ColorModifyItem colorMI)
    {
        activeColorModifyItem = colorMI;
    }

    private void OnNextMachine()
    {
        OnFocusMachineByIndex(Mathf.Min(machinesGameObjects.Count - 1, activeIndexMachine + 1));
    }

    private void OnPrevMachine()
    {
        OnFocusMachineByIndex(Mathf.Max(0, activeIndexMachine - 1));
    }

    private void OnSellActiveMachine()
    {
        var configMachine = gameManager.StateManager.statePlayer.machines[activeIndexMachine];
        
        Destroy(machinesGameObjects[activeIndexMachine].gameObject);

        machinesGameObjects.RemoveAt(activeIndexMachine);

        gameManager.StateManager.SellMachine(configMachine);

        OnFocusMachineByIndex(0);
    }

    private void Init()
    {
        // gameManager.StateManager.AddMachine("T3");

        if (gameManager?.Settings != null)
        {
            machinesConfigs.AddRange(gameManager.Settings.machines);
        }

        DrawMachines();

        var index = gameManager.StateManager.statePlayer != null ? gameManager.StateManager.statePlayer.indexActiveMachine : 0;

        if (index > machinesGameObjects.Count)
        {
            index = 0;
        }
        
        if (machinesGameObjects.Count > 0)
        {
            OnFocusMachineByIndex(index);
        }

        Wrapper3dModels.gameObject.SetActive(false);
    }

    private void DrawMachines()
    {
        for (int i = 0; i < machinesGameObjects.Count; i++)
        {
            Destroy(machinesGameObjects[i].gameObject);

            machinesGameObjects.RemoveAt(i);
        }
        
        int index = 0;

        foreach (var item in gameManager.StateManager.statePlayer.machines)
        {
            GameMachine configMachine = machinesConfigs.Find(m => m.name == item.name);

            if (configMachine != null)
            {
                CreateMachine(configMachine, index, new MachineLevelData
                {
                    id = item.name,
                    data = item.data
                });
            }
            
            index ++;
        }

    }

    void OnFocusMachineByIndex(int index = 0)
    {
        activeIndexMachine = Mathf.Clamp(index, 0, gameManager.StateManager.statePlayer.machines.Count - 1);

        gameManager.StateManager.SetActiveMachine(activeIndexMachine);
        if (gameManager.StateManager.statePlayer.machines.Count <= 0)
        {
            return;
        }

        for (int i = 0; i < machinesGameObjects.Count; i++)
        {
            machinesGameObjects[i].transform.localPosition = new Vector3(machinesGameObjects[i].transform.localPosition.x, 100, machinesGameObjects[i].transform.localPosition.z);
        }

        machinesGameObjects[activeIndexMachine].transform.localPosition = new Vector3(machinesGameObjects[activeIndexMachine].transform.localPosition.x, 0, machinesGameObjects[activeIndexMachine].transform.localPosition.z);;


        CinemachineCamera.Follow = machinesGameObjects[activeIndexMachine].objectTargetCamera.transform;
        CinemachineCamera.LookAt = machinesGameObjects[activeIndexMachine].objectTargetCamera.transform;
        
        CinemachineOrbitalFollow cinemachineOrbitalFollow = CinemachineCamera.GetComponent<CinemachineOrbitalFollow>();
        if (cinemachineOrbitalFollow)
        {
            cinemachineOrbitalFollow.HorizontalAxis.Value = 90;
            cinemachineOrbitalFollow.VerticalAxis.Value = 15;
        }

        GarageUIEvents.OnFocusMachine?.Invoke(machinesGameObjects[activeIndexMachine]);
    }

    void CreateMachine(GameMachine configMachine, int index, MachineLevelData data)
    {
        var gObject = Instantiate(
            configMachine.machinePrefab,
            new Vector3(0, 0, -index * 0.1f),
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

            foreach (var tower in obj.Towers)
            {
                tower.OnSetAngleTower(new Vector3(-1, 0.3f, 1), false, Time.deltaTime);
            }
        }

        gObject.layer = Wrapper.gameObject.layer;
        Transform[] children = gObject.GetComponentsInChildren<Transform>(true);
        foreach (Transform child in children)
        {
            child.gameObject.layer = gObject.layer;
        }

    }

    private void OnStartTouch(InputAction.CallbackContext context)
    {
        Vector2 mousePosition = mousePositionAction.action.ReadValue<Vector2>();
        
        Ray ray = gameManager.ActiveCamera.ScreenPointToRay(mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // Debug.Log($"hit.collider={hit.collider}");
            BaseMachine bm = hit.collider.GetComponentInParent<BaseMachine>();

            if (bm != null)
            {
                OnEnableInputCamera();
            }
        }
    }

    private void OnEndTouch(InputAction.CallbackContext context)
    {
        OnDisableInputCamera();
    }

    private void OnEnableInputCamera()
    {
        CinemachineCameraInputController.enabled = true;
    }

    private void OnDisableInputCamera()
    {
        CinemachineCameraInputController.enabled = false;
    }

    private void OnCancelColorModify()
    {
        activeColorModifyItem.color = Color.clear;
    }

    private void OnClickPerformed(InputAction.CallbackContext context)
    {
        // Read the current mouse position
        Vector2 mousePosition = mousePositionAction.action.ReadValue<Vector2>();
        
        // Create a ray from the camera through the mouse position
        Ray ray = gameManager.ActiveCamera.ScreenPointToRay(mousePosition);
        RaycastHit hit;

        // Perform the raycast
        if (Physics.Raycast(ray, out hit))
        {
            // Log the name of the object hit
            // Debug.Log($"Clicked on {hit.point}: " + hit.collider.gameObject.name);

            // You can add further logic here, e.g., call a method on the hit object
            // hit.collider.GetComponent<YourCustomScript>()?.HandleClick();
            IColored coloredObject = hit.collider.GetComponentInParent<IColored>();
            BaseMachine bm = hit.collider.GetComponentInParent<BaseMachine>();

            // Debug.DrawLine(hit.point, gameManager.ActiveCamera.transform.position, Color.yellow, 5);

            if (coloredObject != null && !HelperVoxel.AreColorsApproximatelyEqual(activeColorModifyItem.color, Color.clear))
            {
                FillData output = coloredObject.OnFill(hit.point);

                output.outputColor = activeColorModifyItem.color;

                var newColorsModify = gameManager.StateManager.OnFillMachine(output);

                var activeMachine = gameManager.StateManager.statePlayer.machines[gameManager.StateManager.statePlayer.indexActiveMachine];

                // bm.MachineLevelData = new MachineLevelData
                // {
                //     id = activeMachine.name,
                //     colorsModify = activeMachine.colorsModifies,
                // };

                bm.ReDraw(activeMachine.data.colorsModifies);

                OnCancelColorModify();

                GarageUIEvents.FillOk?.Invoke();
            }
        }
    }

    void CreateTowerItems()
    {
        var data = gameManager.Settings.machines.Select(x => x.towers).ToList();
        dataTowers = new List<GameTowerOption>();
        foreach (var item in data)
        {
            dataTowers.AddRange(item);
        }

        float i = 0f;
        foreach (var item in dataTowers)
        {
            CreateTowerItem(item, new Vector2(0, i));
            i += step;
        }
    }

    void CreateTowerItem(GameTowerOption configTower, Vector2 point)
    {
        var gObject = Instantiate(
            gameManager.Settings.prefabGarageItemTower,
            Vector3.zero,
            Quaternion.identity,
            Wrapper3dModels
        );
        UIGarageTowerItemView obj = gObject.GetComponent<UIGarageTowerItemView>();
        if (obj != null)
        {
            towersGameObjects.Add(obj);

            obj.Init(configTower, 0, new DataDetail());
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

        gObject.layer = Wrapper3dModels.gameObject.layer;
        Transform[] children = gObject.GetComponentsInChildren<Transform>(true);
        foreach (Transform child in children)
        {
            child.gameObject.layer = gObject.layer;
        }

        gObject.transform.SetLocalPositionAndRotation(new Vector3(point.x, -point.y, 0), Quaternion.Euler(0, 0, 0));
    }
}
