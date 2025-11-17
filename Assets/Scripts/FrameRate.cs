using UnityEngine;

public class FrameRate : MonoBehaviour
{
    void Awake()
    {
        Application.targetFrameRate = GameManager.Instance.Settings.DebugSettings.mode == AppMode.Mobile ? 60 : 1000;
    }
}
