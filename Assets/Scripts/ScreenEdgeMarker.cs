using UnityEngine;
using UnityEngine.UI;

public class ScreenEdgeMarker : MonoBehaviour
{
    public GameObject indicatorPrefab; // UI object to use as indicator
    public float distanceBuffer = 10f; // Distance from screen edge to show indicator

    private RectTransform indicator;
    private Camera _camera;

    void Start()
    {
        _camera = GameObject.FindGameObjectWithTag("CameraGame").GetComponent<Camera>();// Camera.main;
        indicator = Instantiate(indicatorPrefab, transform.parent).GetComponent<RectTransform>();
        indicator.gameObject.SetActive(false); // Initially hide indicator
    }

    void Update()
    {
        if (_camera == null) return; // Ensure camera is available

        Vector3 worldPosition = transform.position; // Object's position in world space
        Vector3 screenPosition = _camera.WorldToScreenPoint(worldPosition); // Convert to screen space

        if (IsOffscreen(screenPosition))
        {
            indicator.gameObject.SetActive(true); // Show indicator

            // Calculate position on screen edge
            Vector2 edgePosition = CalculateEdgePosition(screenPosition);
            indicator.anchoredPosition = edgePosition;
        }
        else
        {
            indicator.gameObject.SetActive(false); // Hide indicator
        }
    }

    // Check if the object is offscreen
    private bool IsOffscreen(Vector3 screenPosition)
    {
        return screenPosition.x < 0 || screenPosition.x > Screen.width || screenPosition.y < 0 || screenPosition.y > Screen.height;
    }

    // Calculate position on the closest screen edge
    private Vector2 CalculateEdgePosition(Vector3 screenPosition)
    {
        Vector2 edgePosition = Vector2.zero;

        if (screenPosition.x < distanceBuffer)
        {
            edgePosition.x = distanceBuffer;
        }
        else if (screenPosition.x > Screen.width - distanceBuffer)
        {
            edgePosition.x = Screen.width - distanceBuffer;
        }
        else if (screenPosition.y < distanceBuffer)
        {
            edgePosition.y = distanceBuffer;
        }
        else if (screenPosition.y > Screen.height - distanceBuffer)
        {
            edgePosition.y = Screen.height - distanceBuffer;
        }

        return edgePosition;
    }
}