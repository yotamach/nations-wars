using UnityEngine;

namespace NationsWars.Game
{
    /// <summary>
    /// Floats above a unit and always faces the camera. Shows one to three gold stars for rank, and a health bar
    /// when the unit is selected or hurt.
    /// </summary>
    public class UnitOverlay : MonoBehaviour
    {
        const float BarWidth = 2.2f;
        const float BarHeight = 0.22f;
        const float StarSize = 0.5f;
        const float StarSpacing = 0.6f;

        static readonly Color Gold = new Color(1f, 0.82f, 0.2f);

        Unit unit;
        Transform root;
        Transform fill;
        GameObject back;
        Renderer fillRenderer;
        readonly GameObject[] stars = new GameObject[3];
        int shownStars = -1;

        public void Init(Unit owner, float height)
        {
            unit = owner;
            root = new GameObject("Overlay").transform;
            root.SetParent(transform, false);
            root.localPosition = new Vector3(0f, height, 0f);

            back = Quad("BarBack", new Color(0.05f, 0.05f, 0.06f), new Vector3(BarWidth + 0.1f, BarHeight + 0.08f, 0.02f), Vector3.zero);
            var f = Quad("BarFill", Color.green, new Vector3(BarWidth, BarHeight, 0.02f), new Vector3(0f, 0f, -0.02f));
            fill = f.transform;
            fillRenderer = f.GetComponent<Renderer>();

            for (int i = 0; i < stars.Length; i++)
            {
                stars[i] = Quad("Star" + (i + 1), Gold, new Vector3(StarSize, StarSize, 0.04f), Vector3.zero);
                stars[i].transform.localRotation = Quaternion.Euler(0f, 0f, 45f); // a square on its corner reads as a diamond
            }
        }

        GameObject Quad(string objName, Color color, Vector3 scale, Vector3 pos)
        {
            GameObject q = GameObject.CreatePrimitive(PrimitiveType.Cube);
            q.name = objName;
            Destroy(q.GetComponent<Collider>());
            q.transform.SetParent(root, false);
            q.transform.localPosition = pos;
            q.transform.localScale = scale;
            q.GetComponent<Renderer>().sharedMaterial = MaterialUtil.Get(color);
            return q;
        }

        void LateUpdate()
        {
            Camera cam = Camera.main;
            if (cam == null || unit == null || unit.IsDead) return;

            root.rotation = cam.transform.rotation;

            float frac = Mathf.Clamp01(unit.Health / (float)unit.Def.maxHealth);
            bool showBar = unit.IsSelected || frac < 0.999f;
            back.SetActive(showBar);
            fill.gameObject.SetActive(showBar);
            if (showBar)
            {
                float w = BarWidth * frac;
                fill.localScale = new Vector3(Mathf.Max(0.001f, w), BarHeight, 0.02f);
                fill.localPosition = new Vector3(-(BarWidth - w) * 0.5f, 0f, -0.02f);
                fillRenderer.sharedMaterial = MaterialUtil.Get(frac > 0.6f ? new Color(0.35f, 0.89f, 0.48f) : frac > 0.3f ? new Color(0.95f, 0.82f, 0.29f) : new Color(0.94f, 0.31f, 0.24f));
            }

            if (unit.Stars != shownStars)
            {
                shownStars = unit.Stars;
                for (int i = 0; i < stars.Length; i++)
                {
                    stars[i].SetActive(i < shownStars);
                    stars[i].transform.localPosition = new Vector3((i - (shownStars - 1) * 0.5f) * StarSpacing, 0.5f, -0.02f);
                }
            }
        }
    }
}
