using UnityEngine;

namespace NationsWars.Game
{
    /// <summary>Right-click on the ground to send the selected units there, spread out in a block.</summary>
    public class CommandIssuer : MonoBehaviour
    {
        public float spacing = 2.4f;

        void Update()
        {
            if (!RtsInput.RightDown || SelectionManager.Instance == null) return;

            var selected = SelectionManager.Instance.Selected;
            if (selected.Count == 0) return;

            Camera cam = Camera.main;
            if (cam == null) return;

            Ray ray = cam.ScreenPointToRay(RtsInput.MousePosition);
            var ground = new Plane(Vector3.up, Vector3.zero);
            float distance;
            if (!ground.Raycast(ray, out distance)) return;
            Vector3 target = ray.GetPoint(distance);

            int cols = Mathf.CeilToInt(Mathf.Sqrt(selected.Count));
            int rows = Mathf.CeilToInt(selected.Count / (float)cols);

            for (int i = 0; i < selected.Count; i++)
            {
                float x = (i % cols - (cols - 1) * 0.5f) * spacing;
                float z = (i / cols - (rows - 1) * 0.5f) * spacing;
                selected[i].MoveTo(target + new Vector3(x, 0f, z));
            }
        }
    }
}
