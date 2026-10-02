using NationsWars.Sim;
using UnityEngine;

namespace NationsWars.Game
{
    /// <summary>Placeholder IMGUI HUD: credits, power, selection, promotion notices. Replaced by UI Toolkit later.</summary>
    public class SkirmishHud : MonoBehaviour
    {
        const float PanelWidth = 280f;
        const float PanelHeight = 170f;
        const float NoticeSeconds = 4f;

        static string notice = "";
        static float noticeUntil;

        public static void Notify(string message)
        {
            notice = message;
            noticeUntil = Time.unscaledTime + NoticeSeconds;
        }

        public static bool IsOverHud(Vector2 screenPos)
        {
            // Input y goes up from the bottom; the panel is drawn from the top-left.
            return screenPos.x <= 12f + PanelWidth && screenPos.y >= Screen.height - 12f - PanelHeight;
        }

        void OnEnable() { Unit.AnyPromoted += OnPromoted; }
        void OnDisable() { Unit.AnyPromoted -= OnPromoted; }

        void OnPromoted(Unit unit, int stars)
        {
            if (unit.OwnerSlot != SkirmishSession.LocalSlot) return;
            Notify(unit.Def.displayName + " promoted to " + VeterancyRules.RankName(stars) + " (" + stars + "/3 stars)");
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
            GUILayout.Label(SelectionLine(), Style());
            GUILayout.Label("LMB select · drag box · RMB move / attack\nWASD pan · wheel zoom · Ctrl+1-9 groups", Style(11));
            GUILayout.EndArea();

            if (Time.unscaledTime < noticeUntil)
            {
                var style = new GUIStyle(GUI.skin.box) { richText = true, fontSize = 14, alignment = TextAnchor.MiddleLeft, normal = { textColor = new Color(1f, 0.85f, 0.3f) } };
                GUI.Box(new Rect(12f, 12f + PanelHeight + 6f, PanelWidth + 80f, 28f), notice, style);
            }
        }

        static string SelectionLine()
        {
            var sel = SelectionManager.Instance != null ? SelectionManager.Instance.Selected : null;
            if (sel == null || sel.Count == 0) return "<b>Selected</b>  none";
            if (sel.Count > 1) return "<b>Selected</b>  " + sel.Count + " units";

            Unit u = sel[0];
            string rank = VeterancyRules.RankName(u.Stars) + " " + u.Stars + "/3";
            string next = u.Vet.IsMaxRank ? "max rank" : u.Vet.XpToNextStar + " xp to next star";
            return "<b>" + u.Def.displayName + "</b>  " + u.Health + "/" + u.Def.maxHealth + " HP\n" +
                   rank + "  ·  " + u.Kills + " kills  ·  " + next;
        }

        static GUIStyle Style(int size = 14)
        {
            return new GUIStyle(GUI.skin.label) { richText = true, fontSize = size, normal = { textColor = Color.white } };
        }
    }
}
