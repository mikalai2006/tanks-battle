using UnityEngine;

public class FrameRate : MonoBehaviour
{
    void Awake()
    {
        Application.targetFrameRate = GameManager.Instance.Settings.DebugSettings.mode == AppMode.Mobile ? GameManager.Instance.Settings.playerOptions.MobileFPS : 1000;
    }
}
