using System.Collections.Generic;
using UnityEngine;

public class SectorAttack : MonoBehaviour
{
    [SerializeField] private LineRenderer lr;
    [SerializeField] private float angle1;
    [SerializeField] private float angle2;
    [SerializeField] private float radius;
    
    void Awake()
    {
        List<Vector3> startPoints = new();
        lr.positionCount = 10;

        float resolution = 10.0f;
        for (float t = 0.0f; t < 1.0f; t += 1.0f / resolution)
        {
            float a = Mathf.LerpAngle(angle1, angle2, t);
            float x = Mathf.Cos(a) * radius;
            float y = Mathf.Sin(a) * radius;

            startPoints.Add(new Vector3(transform.parent.parent.transform.position.x + x, transform.parent.parent.transform.position.y + y, 0));
            // lr.SetPosition(lr.positionCount, new Vector3(x, y, 0));
        }

        lr.SetPositions(startPoints.ToArray());
    }

    void Update()
    {
        
    }
}
