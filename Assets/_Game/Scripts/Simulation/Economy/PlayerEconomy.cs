using System;

namespace NationsWars.Sim
{
    public static class EconomyRules
    {
        public const int OreLoad = 700;
        public const int GemLoad = 1400;
        public const int OilCreditsPerMinute = 300;
        public const float LowPowerProductionSpeed = 0.5f;
    }

    /// <summary>Credits and power for one player. No engine types, so it can run on a fixed sim tick.</summary>
    public sealed class PlayerEconomy
    {
        float oilRemainder;

        public int Credits { get; private set; }
        public int PowerProduced { get; private set; }
        public int PowerConsumed { get; private set; }

        public PlayerEconomy(int startingCredits)
        {
            if (startingCredits < 0) throw new ArgumentOutOfRangeException("startingCredits");
            Credits = startingCredits;
        }

        public bool IsLowPower { get { return PowerConsumed > PowerProduced; } }

        /// <summary>Production runs at half speed on low power. Radar and defenses shut off (handled by their owners).</summary>
        public float ProductionSpeedMultiplier
        {
            get { return IsLowPower ? EconomyRules.LowPowerProductionSpeed : 1f; }
        }

        public void Earn(int amount)
        {
            if (amount < 0) throw new ArgumentOutOfRangeException("amount");
            Credits += amount;
        }

        public bool TrySpend(int amount)
        {
            if (amount < 0) throw new ArgumentOutOfRangeException("amount");
            if (amount > Credits) return false;
            Credits -= amount;
            return true;
        }

        public void AddPower(int produced, int consumed)
        {
            PowerProduced += Math.Max(0, produced);
            PowerConsumed += Math.Max(0, consumed);
        }

        public void RemovePower(int produced, int consumed)
        {
            PowerProduced = Math.Max(0, PowerProduced - Math.Max(0, produced));
            PowerConsumed = Math.Max(0, PowerConsumed - Math.Max(0, consumed));
        }

        /// <summary>Adds oil income for elapsed time and returns the whole credits paid out. Fractions carry over.</summary>
        public int TickOil(int derricks, float seconds, float incomeMultiplier = 1f)
        {
            if (derricks <= 0 || seconds <= 0f) return 0;
            oilRemainder += derricks * (EconomyRules.OilCreditsPerMinute / 60f) * seconds * incomeMultiplier;
            int whole = (int)oilRemainder;
            oilRemainder -= whole;
            Credits += whole;
            return whole;
        }
    }
}
