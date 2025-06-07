using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] float smoothSpeed;
    [SerializeField] Vector3 offset;
    [SerializeField] Camera _camera;

    void Awake()
    {
        _camera = GameObject.FindGameObjectWithTag("CameraGame").GetComponent<Camera>();
    }

    void Start()
    {
        offset = _camera.ViewportToWorldPoint(new Vector3(0.5f, 0.4f, 0));
    }

    void LateUpdate()
    {
        _camera.transform.position = Vector3.Lerp(_camera.transform.position, transform.position + offset, smoothSpeed);
    }
}
