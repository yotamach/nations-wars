using NationsWars.Sim;
using NUnit.Framework;

namespace NationsWars.Tests
{
    public class VeterancyTests
    {
        [Test]
        public void NewUnitsAreRookies()
        {
            var v = new Veterancy(900);
            Assert.AreEqual(0, v.Stars);
            Assert.AreEqual(0, v.Xp);
            Assert.AreEqual("Rookie", VeterancyRules.RankName(v.Stars));
        }

        [Test]
        public void StarsComeAtOnePointFiveThreeAndFiveTimesOwnCost()
        {
            Assert.AreEqual(150, VeterancyRules.ThresholdXp(100, 1));
            Assert.AreEqual(300, VeterancyRules.ThresholdXp(100, 2));
            Assert.AreEqual(500, VeterancyRules.ThresholdXp(100, 3));
            Assert.AreEqual(1350, VeterancyRules.ThresholdXp(900, 1));
        }

        [Test]
        public void KillingEnemiesEarnsStarsOneAtATime()
        {
            var v = new Veterancy(100);              // thresholds 150 / 300 / 500
            Assert.IsFalse(v.AddKill(100));          // 100 xp
            Assert.AreEqual(0, v.Stars);
            Assert.IsTrue(v.AddKill(100));           // 200 xp -> Veteran
            Assert.AreEqual(1, v.Stars);
            Assert.IsFalse(v.AddKill(50));           // 250
            Assert.IsTrue(v.AddKill(50));            // 300 -> Elite
            Assert.AreEqual(2, v.Stars);
            Assert.IsTrue(v.AddKill(200));           // 500 -> Heroic
            Assert.AreEqual(3, v.Stars);
            Assert.AreEqual("Heroic", VeterancyRules.RankName(v.Stars));
        }

        [Test]
        public void OneBigKillCanSkipRanks()
        {
            var v = new Veterancy(100);
            Assert.IsTrue(v.AddKill(1000));
            Assert.AreEqual(3, v.Stars);
        }

        [Test]
        public void StarsStopAtThree()
        {
            var v = new Veterancy(100);
            v.AddKill(500);
            Assert.IsFalse(v.AddKill(5000));
            Assert.AreEqual(3, v.Stars);
            Assert.IsTrue(v.IsMaxRank);
            Assert.AreEqual(0, v.XpToNextStar);
        }

        [Test]
        public void XpToNextStarCountsDown()
        {
            var v = new Veterancy(100);
            Assert.AreEqual(150, v.XpToNextStar);
            v.AddKill(100);
            Assert.AreEqual(50, v.XpToNextStar);
            v.AddKill(50);
            Assert.AreEqual(150, v.XpToNextStar); // star 1 reached, 150 xp now needs 300
        }

        [Test]
        public void WorthlessKillsGiveNothing()
        {
            var v = new Veterancy(100);
            Assert.IsFalse(v.AddKill(0));
            Assert.IsFalse(v.AddKill(-50));
            Assert.AreEqual(0, v.Xp);
        }

        [Test]
        public void FreeUnitsStillNeedKillsToRank()
        {
            var v = new Veterancy(0);
            Assert.AreEqual(2, VeterancyRules.ThresholdXp(0, 1)); // treated as cost 1
            Assert.IsFalse(v.AddKill(1));
            Assert.IsTrue(v.AddKill(1));
            Assert.AreEqual(1, v.Stars);
        }

        [Test]
        public void EachStarAddsTwentyPercentOffenceAndDefence()
        {
            Assert.AreEqual(1.0f, VeterancyRules.DamageMultiplier(0), 0.0001f);
            Assert.AreEqual(1.2f, VeterancyRules.DamageMultiplier(1), 0.0001f);
            Assert.AreEqual(1.4f, VeterancyRules.DamageMultiplier(2), 0.0001f);
            Assert.AreEqual(1.6f, VeterancyRules.DamageMultiplier(3), 0.0001f);
            Assert.AreEqual(1f / 1.2f, VeterancyRules.DamageTakenMultiplier(1), 0.0001f);
            Assert.AreEqual(1f / 1.6f, VeterancyRules.DamageTakenMultiplier(3), 0.0001f);
        }

        [Test]
        public void StarsFeedTheDamageTable()
        {
            var t = DamageTable.Default;
            // Grizzly cannon (40) vs a heavy tank
            Assert.AreEqual(40, t.Calculate(40, Warhead.Cannon, Armor.HeavyArmor, 0, 0));
            Assert.AreEqual(48, t.Calculate(40, Warhead.Cannon, Armor.HeavyArmor, 1, 0));
            Assert.AreEqual(64, t.Calculate(40, Warhead.Cannon, Armor.HeavyArmor, 3, 0));
            Assert.AreEqual(25, t.Calculate(40, Warhead.Cannon, Armor.HeavyArmor, 0, 3));
            // Equal stars cancel out
            Assert.AreEqual(40, t.Calculate(40, Warhead.Cannon, Armor.HeavyArmor, 2, 2));
        }

        [Test]
        public void OnlyHeroicUnitsSelfHeal()
        {
            Assert.AreEqual(0f, VeterancyRules.HealFractionPerSecond(0), 0.0001f);
            Assert.AreEqual(0f, VeterancyRules.HealFractionPerSecond(2), 0.0001f);
            Assert.AreEqual(0.01f, VeterancyRules.HealFractionPerSecond(3), 0.0001f);
        }

        [Test]
        public void StarOutsideOneToThreeIsRejected()
        {
            Assert.Throws<System.ArgumentOutOfRangeException>(() => VeterancyRules.ThresholdXp(100, 0));
            Assert.Throws<System.ArgumentOutOfRangeException>(() => VeterancyRules.ThresholdXp(100, 4));
        }
    }
}
