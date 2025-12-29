using System;
using System.Collections.Generic;
using UIToolkitLibrary;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class GarageUIManager : MonoBehaviour
{
    GameManager gameManager => GameManager.Instance;
    [SerializeField] private Transform Wrapper;
    [SerializeField] private Camera Camera;
    [SerializeField] private CinemachineCamera CinemachineCamera;
    List<BaseMachine> machines;
    int activeIndexMachine = 0;

    void Start()
    {
        machines = new List<BaseMachine>();

        Init();

        ShopUIEvents.ClickButtonNextInShop += OnNextMachine;
        ShopUIEvents.ClickButtonPrevInShop += OnPrevMachine;
    }

    void OnDestroy()
    {
        ShopUIEvents.ClickButtonNextInShop -= OnNextMachine;
        ShopUIEvents.ClickButtonPrevInShop -= OnPrevMachine;
    }

    private void OnNextMachine()
    {
        activeIndexMachine = Mathf.Min(machines.Count - 1, activeIndexMachine + 1);

        OnFocusMachineByIndex(activeIndexMachine);

        ShopUIEvents.FocusMachineInShop.Invoke(gameManager.Settings.machines[activeIndexMachine]);
    }
    private void OnPrevMachine()
    {
        activeIndexMachine = Mathf.Max(0, activeIndexMachine - 1);
        
        OnFocusMachineByIndex(activeIndexMachine);

        ShopUIEvents.FocusMachineInShop.Invoke(gameManager.Settings.machines[activeIndexMachine]);
    }

    private void Init()
    {

        // gameManager.StateManager.AddMachine("T3");
        int index = 0;

        foreach (var item in gameManager.Settings.machines)
        {
            GameMachine configMachine = gameManager.Settings.machines.Find(m => m.name == item.name);

            CreateMachine(configMachine, index, new MachineLevelData
            {
                id = item.name,
                gerbId = gameManager.StateManager.statePlayer.gerbId,
                name = gameManager.AppInfo.UserInfo.name,
                rank = gameManager.StateManager.statePlayer.rank,
            });

            index ++;
        }

        
        CinemachineBrain brain = Camera.GetComponent<CinemachineBrain>();
        if (brain != null && machines.Count > 0)
        {
            OnFocusMachineByIndex(activeIndexMachine);
        }
    }

    void OnFocusMachineByIndex(int index = 0)
    {
        index = Mathf.Clamp(index, 0, gameManager.Settings.machines.Count - 1);

        CinemachineCamera.Follow = machines[index].objectTargetCamera.transform;
        CinemachineCamera.LookAt = machines[index].objectTargetCamera.transform;
        
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
            machines.Add(obj);

            obj.GetComponent<PlayerController>().enabled = false;
            obj.GetComponent<PlayerInput>().enabled = false;
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
