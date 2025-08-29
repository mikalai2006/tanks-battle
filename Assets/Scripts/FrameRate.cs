using UnityEngine;

public class FrameRate : MonoBehaviour
{
    void Awake()
    {
        Application.targetFrameRate = GameManager.Instance.Settings.inputJoystick ? 60 : 1000;
    }
}
