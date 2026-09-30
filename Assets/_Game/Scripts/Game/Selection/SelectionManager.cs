using System.Collections.Generic;
using UnityEngine;

namespace NationsWars.Game
{
    /// <summary>
    /// Click to select, drag a box to select many, double-click to select every unit of that type on screen,
    /// Shift to add, Ctrl+1..9 to save a control group and 1..9 to recall it.
    /// </summary>
    public class SelectionManager : MonoBehaviour
    {
        public static SelectionManager Instance { get; private set; }

        const float DragThreshold = 6f;
        const float ClickRadius = 30f;
        const float DoubleClickTime = 0.35f;

        public readonly List<Unit> Selected = new List<Unit>();

        readonly List<Unit>[] groups = new List<Unit>[10];
        Vector2 dragStart;
        bool mouseDown;
        bool dragging;
        float lastClickTime = -10f;
        UnitDef lastClickedDef;

        void Awake() { Instance = this; }
        void OnDestroy() { if (Instance == this) Instance = null; }

        void Update()
        {
            Selected.RemoveAll(u => u == null);

            Camera cam = Camera.main;
            if (cam == null) return;
            Vector2 mouse = RtsInput.MousePosition;

            if (RtsInput.LeftDown && !SkirmishHud.IsOverHud(mouse))
            {
                mouseDown = true;
                dragging = false;
                dragStart = mouse;
            }

            if (mouseDown && RtsInput.LeftHeld && (mouse - dragStart).sqrMagnitude > DragThreshold * DragThreshold)
                dragging = true;

            if (mouseDown && RtsInput.LeftUp)
            {
                if (dragging) BoxSelect(dragStart, mouse, cam, RtsInput.Shift);
                else ClickSelect(mouse, cam, RtsInput.Shift);
                mouseDown = false;
                dragging = false;
            }

            for (int n = 1; n <= 9; n++)
            {
                if (!RtsInput.DigitDown(n)) continue;
                if (RtsInput.Ctrl) groups[n] = new List<Unit>(Selected);
                else if (groups[n] != null) SetSelection(groups[n].FindAll(u => u != null), false);
            }
        }

        bool IsLocal(Unit u) { return u.OwnerSlot == SkirmishSession.LocalSlot; }

        static bool OnScreen(Camera cam, Unit u, out Vector2 screen)
        {
            Vector3 s = cam.WorldToScreenPoint(u.transform.position);
            screen = s;
            return s.z > 0f;
        }

        void ClickSelect(Vector2 mouse, Camera cam, bool additive)
        {
            Unit best = null;
            float bestDist = ClickRadius * ClickRadius;
            foreach (Unit u in Unit.All)
            {
                Vector2 s;
                if (!IsLocal(u) || !OnScreen(cam, u, out s)) continue;
                float d = (s - mouse).sqrMagnitude;
                if (d < bestDist) { bestDist = d; best = u; }
            }

            if (best == null)
            {
                if (!additive) SetSelection(new List<Unit>(), false);
                return;
            }

            bool doubleClick = Time.unscaledTime - lastClickTime < DoubleClickTime && best.Def == lastClickedDef;
            lastClickTime = Time.unscaledTime;
            lastClickedDef = best.Def;

            if (!doubleClick)
            {
                SetSelection(new List<Unit> { best }, additive);
                return;
            }

            var same = new List<Unit>();
            foreach (Unit u in Unit.All)
            {
                Vector2 s;
                if (IsLocal(u) && u.Def == best.Def && OnScreen(cam, u, out s) &&
                    s.x >= 0 && s.y >= 0 && s.x <= Screen.width && s.y <= Screen.height)
                    same.Add(u);
            }
            SetSelection(same, additive);
        }

        void BoxSelect(Vector2 a, Vector2 b, Camera cam, bool additive)
        {
            Vector2 min = Vector2.Min(a, b), max = Vector2.Max(a, b);
            var inside = new List<Unit>();
            foreach (Unit u in Unit.All)
            {
                Vector2 s;
                if (!IsLocal(u) || !OnScreen(cam, u, out s)) continue;
                if (s.x >= min.x && s.x <= max.x && s.y >= min.y && s.y <= max.y) inside.Add(u);
            }
            SetSelection(inside, additive);
        }

        public void SetSelection(List<Unit> units, bool additive)
        {
            if (!additive)
            {
                foreach (Unit u in Selected) if (u != null) u.SetSelected(false);
                Selected.Clear();
            }
            foreach (Unit u in units)
            {
                if (u == null || Selected.Contains(u)) continue;
                Selected.Add(u);
                u.SetSelected(true);
            }
        }

        void OnGUI()
        {
            if (!dragging) return;
            Vector2 now = RtsInput.MousePosition;
            Vector2 min = Vector2.Min(dragStart, now), max = Vector2.Max(dragStart, now);
            var rect = new Rect(min.x, Screen.height - max.y, max.x - min.x, max.y - min.y);

            Color old = GUI.color;
            GUI.color = new Color(0.4f, 1f, 0.5f, 0.15f);
            GUI.DrawTexture(rect, Texture2D.whiteTexture);
            GUI.color = new Color(0.4f, 1f, 0.5f, 0.9f);
            GUI.DrawTexture(new Rect(rect.x, rect.y, rect.width, 1), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(rect.x, rect.yMax - 1, rect.width, 1), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(rect.x, rect.y, 1, rect.height), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(rect.xMax - 1, rect.y, 1, rect.height), Texture2D.whiteTexture);
            GUI.color = old;
        }
    }
}
