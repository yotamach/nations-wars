using System.Collections.Generic;
using UnityEngine;

namespace NationsWars.Game
{
    [System.Serializable]
    public class SlotConfig
    {
        public NationDef nation;
        public bool isHuman;
        public int team;
        public Color color = Color.white;
    }

    /// <summary>What the lobby hands to the battle scene. Null means "use the default 1v1".</summary>
    public class SkirmishSettings
    {
        public static SkirmishSettings Current;

        public readonly List<SlotConfig> slots = new List<SlotConfig>();
        public int startingCredits = 10000;

        public static SkirmishSettings CreateDefault(NationDef[] nations, int startingCredits)
        {
            var s = new SkirmishSettings { startingCredits = startingCredits };
            NationDef a = nations[0];
            NationDef b = nations[nations.Length > 1 ? 1 : 0];
            s.slots.Add(new SlotConfig { nation = a, isHuman = true, team = 0, color = a.color });
            s.slots.Add(new SlotConfig { nation = b, isHuman = false, team = 1, color = b.color });
            return s;
        }
    }
}
