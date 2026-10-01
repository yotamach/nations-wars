using System;

namespace NationsWars.Sim
{
    /// <summary>
    /// Red Alert 2 style ranks. A unit earns XP equal to the cost of every enemy it kills and gains a star
    /// (Veteran, Elite, Heroic) at fixed multiples of its own cost. Each star raises damage and armor by 20%
    /// (see DamageTable). Heroic units also heal themselves.
    /// </summary>
    public static class VeterancyRules
    {
        public const int MaxStars = 3;
        public const float HeroicHealFractionPerSecond = 0.01f;

        /// <summary>XP needed for star 1, 2 and 3, as a multiple of the unit's own cost.</summary>
        static readonly float[] ThresholdMultipliers = { 1.5f, 3f, 5f };
        static readonly string[] Names = { "Rookie", "Veteran", "Elite", "Heroic" };

        public static int ThresholdXp(int unitCost, int star)
        {
            if (star < 1 || star > MaxStars) throw new ArgumentOutOfRangeException("star");
            int cost = Math.Max(1, unitCost);
            return Math.Max(1, (int)Math.Round(cost * ThresholdMultipliers[star - 1], MidpointRounding.AwayFromZero));
        }

        public static float DamageMultiplier(int stars)
        {
            return 1f + DamageTable.VeterancyBonusPerRank * Clamp(stars);
        }

        /// <summary>Multiplier on damage taken. 3 stars take 1 / 1.6 = 62.5% damage.</summary>
        public static float DamageTakenMultiplier(int stars)
        {
            return 1f / (1f + DamageTable.VeterancyBonusPerRank * Clamp(stars));
        }

        public static float HealFractionPerSecond(int stars)
        {
            return stars >= MaxStars ? HeroicHealFractionPerSecond : 0f;
        }

        public static string RankName(int stars)
        {
            return Names[Clamp(stars)];
        }

        static int Clamp(int stars)
        {
            return Math.Max(0, Math.Min(MaxStars, stars));
        }
    }

    /// <summary>One unit's experience. No engine types, so it can live in the deterministic sim.</summary>
    public sealed class Veterancy
    {
        readonly int unitCost;

        public int Xp { get; private set; }
        public int Stars { get; private set; }

        public Veterancy(int unitCost)
        {
            this.unitCost = unitCost;
        }

        public bool IsMaxRank { get { return Stars >= VeterancyRules.MaxStars; } }

        /// <summary>XP still needed for the next star, or 0 at three stars.</summary>
        public int XpToNextStar
        {
            get { return IsMaxRank ? 0 : VeterancyRules.ThresholdXp(unitCost, Stars + 1) - Xp; }
        }

        /// <summary>Credits a kill. Returns true if the unit gained at least one star (a big kill can skip ranks).</summary>
        public bool AddKill(int victimCost)
        {
            if (victimCost > 0) Xp += victimCost;

            int before = Stars;
            while (Stars < VeterancyRules.MaxStars && Xp >= VeterancyRules.ThresholdXp(unitCost, Stars + 1))
                Stars++;
            return Stars > before;
        }
    }
}
