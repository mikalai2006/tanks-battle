using UnityEngine;

public class IndicatorMachineSetup : MonoBehaviour
{
    [SerializeField] EdgeCollider2D col;

    // the camera the collider is adjusted to

    [SerializeField] Camera cam;
    float offsetW;
    float offsetH;

    void Start()
    {
        offsetW = GameManager.Instance.Settings.offsetMarkerEdge.x;
        offsetH = GameManager.Instance.Settings.offsetMarkerEdge.y;
    }

    private void Update()
    {
        // cam = GameObject.FindGameObjectWithTag("CameraGame").GetComponent<Camera>();
        //Initializes Array with 5 Corners(5Because: reconnecting to first position)

        Vector2[] points = new Vector2[5];

        //sets all the postions where the points need to be

        points[0] = cam.ViewportToWorldPoint(new Vector2(1 - offsetW, 1 - offsetH));
        points[1] = cam.ViewportToWorldPoint(new Vector2(0 + offsetW, 1 - offsetH));
        points[2] = cam.ViewportToWorldPoint(new Vector2(0 + offsetW, 0 + offsetH));
        points[3] = cam.ViewportToWorldPoint(new Vector2(1 - offsetW, 0 + offsetH));
        points[4] = cam.ViewportToWorldPoint(new Vector2(1 - offsetW, 1 - offsetH));

        //sets all the points of the collider to the new positions

        col.points = points;
    }
}
