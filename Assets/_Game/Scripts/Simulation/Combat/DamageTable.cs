using System;

namespace NationsWars.Sim
{
    public enum Armor { Infantry, LightVehicle, HeavyArmor, Aircraft, Building }

    public enum Warhead { Bullet, Cannon, Missile, Fire, Energy, Explosive }

    /// <summary>
    /// Warhead vs armor multipliers, in percent. Matches the damage table in docs/game-plan.html.
    /// </summary>
    public sealed class DamageTable
    {
        public const int MaxVeterancyRank = 3;
        public const float VeterancyBonusPerRank = 0.2f;

        public static readonly DamageTable Default = CreateDefault();

        readonly int[,] percent;

        public DamageTable(int[,] percent)
        {
            if (percent.GetLength(0) != Enum.GetValues(typeof(Warhead)).Length ||
                percent.GetLength(1) != Enum.GetValues(typeof(Armor)).Length)
                throw new ArgumentException("Table must be [warheads, armors].");
            this.percent = percent;
        }

        static DamageTable CreateDefault()
        {
            //                        Inf  Light Heavy Air  Bldg
            return new DamageTable(new int[,]
            {
                /* Bullet    */ { 100,  60,  20,  50,  20 },
                /* Cannon    */ {  30, 100, 100,   0,  70 },
                /* Missile   */ {  25, 100,  90, 100,  60 },
                /* Fire      */ { 150,  60,  25,   0,  80 },
                /* Energy    */ {  80,  90,  90,  60,  60 },
                /* Explosive */ {  90,  80,  60,   0, 150 },
            });
        }

        public int Percent(Warhead warhead, Armor armor)
        {
            return percent[(int)warhead, (int)armor];
        }

        /// <summary>
        /// Damage dealt by one hit. Attacker rank raises damage and defender rank lowers it,
        /// each by 20% per rank. A hit that can damage the target always does at least 1.
        /// </summary>
        public int Calculate(int baseDamage, Warhead warhead, Armor armor, int attackerRank = 0, int defenderRank = 0)
        {
            if (baseDamage <= 0) return 0;
            int pct = Percent(warhead, armor);
            if (pct <= 0) return 0;

            float attack = 1f + VeterancyBonusPerRank * ClampRank(attackerRank);
            float defense = 1f + VeterancyBonusPerRank * ClampRank(defenderRank);
            float damage = baseDamage * (pct / 100f) * attack / defense;
            return Math.Max(1, (int)Math.Round(damage, MidpointRounding.AwayFromZero));
        }

        static int ClampRank(int rank)
        {
            return Math.Max(0, Math.Min(MaxVeterancyRank, rank));
        }
    }
}
