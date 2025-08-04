using UnityEngine;

public class FrameRate : MonoBehaviour
{
    void Awake()
    {
        Application.targetFrameRate = 360;
    }
}
