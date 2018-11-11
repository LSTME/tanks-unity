using UnityEngine;

namespace Camera
{
    public class CameraControl : MonoBehaviour
    {
        public float dampTime = 0.2f;
        public float screenEdgeBuffer = 4f;
        public float minSize = 6.5f;
        public Transform[] targets;


        private new UnityEngine.Camera camera;
        private float zoomSpeed;
        private Vector3 moveVelocity;
        private Vector3 desiredPosition;

        private void Awake()
        {
            camera = GetComponentInChildren<UnityEngine.Camera>();
        }

        private void FixedUpdate()
        {
            Move();
            Zoom();
        }

        private void Move()
        {
            FindAveragePosition();

            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref moveVelocity, dampTime);
        }

        private void FindAveragePosition()
        {
            var averagePos = new Vector3();
            var numTargets = 0;

            foreach (var target in targets)
            {
                if (!target.gameObject.activeSelf)
                    continue;

                averagePos += target.position;
                numTargets++;
            }

            if (numTargets > 0)
                averagePos /= numTargets;

            averagePos.y = transform.position.y;

            desiredPosition = averagePos;
        }

        private void Zoom()
        {
            var requiredSize = FindRequiredSize();
            camera.orthographicSize = Mathf.SmoothDamp(camera.orthographicSize, requiredSize, ref zoomSpeed, dampTime);
        }

        private float FindRequiredSize()
        {
            var desiredLocalPos = transform.InverseTransformPoint(desiredPosition);

            var size = 0f;

            foreach (var target in targets)
            {
                if (!target.gameObject.activeSelf)
                    continue;

                var targetLocalPos = transform.InverseTransformPoint(target.position);
                var desiredPosToTarget = targetLocalPos - desiredLocalPos;

                size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.y));
                size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.x) / camera.aspect);
            }

            size += screenEdgeBuffer;
            size = Mathf.Max(size, minSize);

            return size;
        }

        public void SetStartPositionAndSize()
        {
            FindAveragePosition();

            transform.position = desiredPosition;

            camera.orthographicSize = FindRequiredSize();
        }
    }
}