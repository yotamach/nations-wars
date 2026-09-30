using UnityEngine;

namespace NationsWars.Game
{
    /// <summary>Placeholder IMGUI HUD: credits, power, selection and controls. Replaced by UI Toolkit later.</summary>
    public class SkirmishHud : MonoBehaviour
    {
        const float PanelWidth = 260f;
        const float PanelHeight = 132f;

        public static bool IsOverHud(Vector2 screenPos)
        {
            // Input y goes up from the bottom; the panel is drawn from the top-left.
            return screenPos.x <= 12f + PanelWidth && screenPos.y >= Screen.height - 12f - PanelHeight;
        }

        void OnGUI()
        {
            var eco = SkirmishSession.LocalEconomy;
            if (eco == null) return;

            var panel = new Rect(12f, 12f, PanelWidth, PanelHeight);
            GUI.Box(panel, GUIContent.none);

            GUILayout.BeginArea(new Rect(panel.x + 10f, panel.y + 8f, panel.width - 20f, panel.height - 16f));
            GUILayout.Label("<b>Credits</b>  $" + eco.Credits.ToString("N0"), Style());
            string power = "<b>Power</b>  " + eco.PowerProduced + " / " + eco.PowerConsumed + (eco.IsLowPower ? "  <color=#ff6a5a>LOW</color>" : "");
            GUILayout.Label(power, Style());
            int selected = SelectionManager.Instance != null ? SelectionManager.Instance.Selected.Count : 0;
            GUILayout.Label("<b>Selected</b>  " + selected, Style());
            GUILayout.Label("LMB select · drag box · RMB move\nWASD pan · wheel zoom · Ctrl+1-9 groups", Style(11));
            GUILayout.EndArea();
        }

        static GUIStyle Style(int size = 14)
        {
            return new GUIStyle(GUI.skin.label) { richText = true, fontSize = size, normal = { textColor = Color.white } };
        }
    }
}
