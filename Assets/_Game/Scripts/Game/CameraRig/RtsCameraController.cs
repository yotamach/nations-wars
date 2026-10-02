using UnityEngine;

namespace NationsWars.Game
{
    /// <summary>Fixed-angle RTS camera: WASD or arrows pan, edge pan in builds, scroll wheel zooms.</summary>
    [RequireComponent(typeof(Camera))]
    public class RtsCameraController : MonoBehaviour
    {
        public float pitch = 55f;
        public float yaw = 0f;
        public float panSpeed = 45f;
        public float zoomMin = 20f;
        public float zoomMax = 90f;
        public float zoomSpeed = 8f;
        public float zoom = 55f;
        [Tooltip("Pan when the mouse touches the screen edge. Off in the editor so it does not fight the editor UI.")]
        public bool edgePan;
        public float edgeMargin = 12f;
        public Vector3 focus = Vector3.zero;

        void Start()
        {
            if (!Application.isEditor) edgePan = true;
            Apply();
        }

        public void FocusOn(Vector3 worldPoint)
        {
            focus = new Vector3(worldPoint.x, 0f, worldPoint.z);
            ClampFocus();
            Apply();
        }

        void Update()
        {
            Vector2 axis = RtsInput.MoveAxis;

            if (edgePan && Application.isFocused)
            {
                Vector2 m = RtsInput.MousePosition;
                bool inside = m.x >= 0f && m.y >= 0f && m.x <= Screen.width && m.y <= Screen.height;
                if (inside)
                {
                    if (m.x <= edgeMargin) axis.x -= 1f;
                    if (m.x >= Screen.width - edgeMargin) axis.x += 1f;
                    if (m.y <= edgeMargin) axis.y -= 1f;
                    if (m.y >= Screen.height - edgeMargin) axis.y += 1f;
                }
            }

            axis = Vector2.ClampMagnitude(axis, 1f);
            float zoomFactor = Mathf.Lerp(0.6f, 1.5f, Mathf.InverseLerp(zoomMin, zoomMax, zoom));
            Vector3 move = Quaternion.Euler(0f, yaw, 0f) * new Vector3(axis.x, 0f, axis.y);
            focus += move * (panSpeed * zoomFactor * Time.unscaledDeltaTime);

            zoom = Mathf.Clamp(zoom - RtsInput.Scroll * zoomSpeed, zoomMin, zoomMax);

            ClampFocus();
            Apply();
        }

        void ClampFocus()
        {
            float half = MapGrid.Instance != null ? MapGrid.Instance.HalfExtent : 128f;
            focus.x = Mathf.Clamp(focus.x, -half, half);
            focus.z = Mathf.Clamp(focus.z, -half, half);
        }

        void Apply()
        {
            Quaternion rot = Quaternion.Euler(pitch, yaw, 0f);
            transform.rotation = rot;
            transform.position = focus - rot * Vector3.forward * zoom;
        }
    }
}
