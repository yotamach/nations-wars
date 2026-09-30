using System.Collections.Generic;
using NationsWars.Sim;

namespace NationsWars.Game
{
    /// <summary>Live state of the running skirmish: who is playing and each player's economy.</summary>
    public static class SkirmishSession
    {
        public static SkirmishSettings Settings { get; private set; }
        public static int LocalSlot { get; private set; }
        public static readonly List<PlayerEconomy> Economies = new List<PlayerEconomy>();

        public static PlayerEconomy LocalEconomy
        {
            get { return LocalSlot >= 0 && LocalSlot < Economies.Count ? Economies[LocalSlot] : null; }
        }

        static int FirstHumanSlot(SkirmishSettings settings)
        {
            for (int i = 0; i < settings.slots.Count; i++)
                if (settings.slots[i].isHuman) return i;
            return 0;
        }

        public static void Begin(SkirmishSettings settings)
        {
            Settings = settings;
            LocalSlot = FirstHumanSlot(settings);
            Economies.Clear();
            for (int i = 0; i < settings.slots.Count; i++)
                Economies.Add(new PlayerEconomy(settings.startingCredits));
        }
    }
}
