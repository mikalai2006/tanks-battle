using UnityEngine;

public class SyncPositionTowerBox : MonoBehaviour
{
    [SerializeField] private BaseMachine _machine;
    [SerializeField] private Vector3 _startPosition;
    // [SerializeField] private float radius = 1f;
    [SerializeField] private Quaternion _bodyRotation;

    void Awake()
    {
        _machine = GetComponentInParent<BaseMachine>();

        _startPosition = transform.localPosition;
    }

    void Update()
    {
        _bodyRotation = _machine.Body.transform.rotation;

        // var newX = _startPosition.x + radius * Mathf.Cos(_bodyRotation.z * Mathf.Deg2Rad);

        // var newY = _bodyRotation.y + radius * Mathf.Sin(_bodyRotation.z * Mathf.Deg2Rad);

        // transform.localPosition = new Vector2(newX, newY);

        var offset = _startPosition;

        Vector3 rotatedOffset = _bodyRotation * offset; // Преобразуем локальный сдвиг в мировой

        transform.position = _machine.Body.transform.position + rotatedOffset; // Рассчитываем позицию ребенка
    }
}
