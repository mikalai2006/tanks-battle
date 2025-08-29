using UnityEngine;

public class ParaboloidMover : MonoBehaviour
    {
        public Vector3 startPoint;
        public Vector3 endPoint;
        public float arcHeight = 5f; // Controls the height of the parabola
        public float duration = 1f; // Time to complete the arc

        private float startTime;
        private Vector3 speed;

    public void Init(Vector3 offset, Vector3 _speed, float _duration)
    {
        startTime = Time.time;
        speed = _speed;
        duration = _duration;

        startPoint = transform.position;
        endPoint = startPoint + offset;
        endPoint.y = 0;
    }

        void Update()
        {
            float t = (Time.time - startTime) / duration;

        if (t <= 1f)
        {
            //     // Calculate horizontal position (linear interpolation)
            //     float x = Mathf.Lerp(startPoint.x, endPoint.x, t);
            //     float z = Mathf.Lerp(startPoint.z, endPoint.z, t);

            //     // Calculate vertical position (parabolic interpolation)
            //     // This formula creates an arc that peaks at t = 0.5
            //     Vector3 gravityOffset = Vector3.up * -0.5f * Physics.gravity.y * t * t;
            //     float y = Mathf.Lerp(startPoint.y, endPoint.y, t) + arcHeight * (1 - Mathf.Pow(2 * t - 1, 2));

            // transform.position = new Vector3(x, y, z);// + gravityOffset;
            Vector3 progressBeforeGravity = speed * t;
            Vector3 gravityOffset = Vector3.up * -0.5f * Physics.gravity.y * t * t;
            Vector3 newPosition = transform.position + progressBeforeGravity - gravityOffset;
            transform.position = newPosition;
            }
        }
    }