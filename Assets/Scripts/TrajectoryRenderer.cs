using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class TrajectoryRenderer : MonoBehaviour
{
    private LineRenderer lineRenderer;
    [SerializeField] private int countPoints;
    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    public void ShowTrajectory(Vector3 origin, Vector3 speed)
    {
        Vector3[] points = new Vector3[countPoints];

        lineRenderer.positionCount = countPoints;

        for (int i = 0; i < countPoints; i++)
        {
            float time = i * 0.1f;

            Vector3 progressBeforeGravity = speed * time;
            Vector3 gravityOffset = Vector3.up * -0.5f * Physics.gravity.y * time * time;
            Vector3 newPosition = origin + progressBeforeGravity - gravityOffset;
            points[i] = newPosition;
            //points[i] = origin + speed * time + Physics.gravity * time * time / 2f;

            if (points[i].y < 0)
            {
                lineRenderer.positionCount = i + 1;
                break;
            }
        }

        lineRenderer.SetPositions(points);
    }

        public void ShowStretchTrajectory(Vector3 start, Vector3 end)
    {
        Vector3[] points = new Vector3[countPoints];

        lineRenderer.positionCount = 2;

        points[0] = start;
        points[1] = end;

        lineRenderer.SetPositions(points);
    }
}
