using UnityEngine;

namespace InternationalKarate.Managers
{
    /// <summary>
    /// Camera controller for the 2D fighting game
    /// Keeps both fighters in view
    /// </summary>
    public class CameraController : MonoBehaviour
    {
        [Header("Fighter Tracking")]
        public Transform fighter1;
        public Transform fighter2;

        [Header("Camera Settings")]
        public float smoothSpeed = 5f;
        public float minSize = 5f;
        public float maxSize = 10f;
        public float zoomPadding = 2f;

        [Header("Boundaries")]
        public float leftBoundary = -10f;
        public float rightBoundary = 10f;
        public float groundLevel = 0f;

        private Camera cam;

        private void Awake()
        {
            cam = GetComponent<Camera>();
        }

        private void LateUpdate()
        {
            if (fighter1 == null || fighter2 == null)
                return;

            UpdateCameraPosition();
            UpdateCameraZoom();
        }

        private void UpdateCameraPosition()
        {
            // Calculate midpoint between fighters
            float centerX = (fighter1.position.x + fighter2.position.x) / 2f;

            // Clamp to boundaries
            centerX = Mathf.Clamp(centerX, leftBoundary, rightBoundary);

            // Smooth camera movement
            Vector3 targetPosition = new Vector3(centerX, groundLevel, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
        }

        private void UpdateCameraZoom()
        {
            // Calculate distance between fighters
            float distance = Mathf.Abs(fighter1.position.x - fighter2.position.x);

            // Calculate required camera size to fit both fighters
            float targetSize = (distance / 2f) + zoomPadding;
            targetSize = Mathf.Clamp(targetSize, minSize, maxSize);

            // Smooth zoom
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetSize, smoothSpeed * Time.deltaTime);
        }

        private void OnDrawGizmos()
        {
            // Draw camera boundaries
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(new Vector3(leftBoundary, -10, 0), new Vector3(leftBoundary, 10, 0));
            Gizmos.DrawLine(new Vector3(rightBoundary, -10, 0), new Vector3(rightBoundary, 10, 0));

            // Draw ground level
            Gizmos.color = Color.green;
            Gizmos.DrawLine(new Vector3(leftBoundary, groundLevel, 0), new Vector3(rightBoundary, groundLevel, 0));
        }
    }
}
