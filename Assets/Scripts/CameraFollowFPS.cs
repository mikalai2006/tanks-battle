using UnityEngine;

public class CameraFollowFPS : MonoBehaviour
{
    [SerializeField] float smoothSpeed;
    [SerializeField] Vector3 offset;
    [SerializeField] GameObject _cameraWrapper;

    // void Awake()
    // {
    // }

    void Start()
    {
        offset = new Vector3(0,5,-40); //_camera.ViewportToWorldPoint(new Vector3(0.5f, 0.4f, -20f));
    }

    void LateUpdate()
    {
        _cameraWrapper.transform.position = Vector3.Lerp(_cameraWrapper.transform.position, transform.position + offset, smoothSpeed);
    }
}
