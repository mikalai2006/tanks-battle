using Unity.Cinemachine;
using UnityEngine;

public class CameraDisableInput : MonoBehaviour
{
    private GameSetting _gameSetting => GameManager.Instance.Settings;
    public CinemachineInputAxisController cinemachineInputAxisController;
    void Start()
    {
        if (_gameSetting.DebugSettings.mode == AppMode.Mobile)
        {
            cinemachineInputAxisController = GetComponent<CinemachineInputAxisController>();
            if (cinemachineInputAxisController != null)
            {
                foreach (var controller in cinemachineInputAxisController.Controllers)
                {
                    controller.Enabled = false;
                }
            }
        }
    }
}
