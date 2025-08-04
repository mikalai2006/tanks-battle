using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] float smoothSpeed;
    [SerializeField] Vector3 offset;
    [SerializeField] Camera _camera;

    // void Awake()
    // {
    // }

    void Start()
    {
        _camera = GameObject.FindGameObjectWithTag("CameraGame").GetComponent<Camera>();
        offset = _camera.ViewportToWorldPoint(new Vector3(0.5f, 0.4f, -30f));
    }

    void LateUpdate()
    {
        _camera.transform.position = Vector3.Lerp(_camera.transform.position, transform.position + offset, smoothSpeed);
    }
}
