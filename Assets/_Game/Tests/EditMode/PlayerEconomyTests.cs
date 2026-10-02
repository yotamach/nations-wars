using System;
using NationsWars.Sim;
using NUnit.Framework;

namespace NationsWars.Tests
{
    public class PlayerEconomyTests
    {
        [Test]
        public void SpendingNeedsEnoughCredits()
        {
            var eco = new PlayerEconomy(1000);
            Assert.IsTrue(eco.TrySpend(900));
            Assert.AreEqual(100, eco.Credits);
            Assert.IsFalse(eco.TrySpend(101));
            Assert.AreEqual(100, eco.Credits);
        }

        [Test]
        public void EarnAddsCredits()
        {
            var eco = new PlayerEconomy(0);
            eco.Earn(EconomyRules.OreLoad);
            Assert.AreEqual(700, eco.Credits);
        }

        [Test]
        public void NegativeAmountsAreRejected()
        {
            var eco = new PlayerEconomy(10);
            Assert.Throws<ArgumentOutOfRangeException>(() => eco.Earn(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => eco.TrySpend(-1));
        }

        [Test]
        public void LowPowerHalvesProductionSpeed()
        {
            var eco = new PlayerEconomy(0);
            eco.AddPower(produced: 100, consumed: 0);
            eco.AddPower(produced: 0, consumed: 60);
            Assert.IsFalse(eco.IsLowPower);
            Assert.AreEqual(1f, eco.ProductionSpeedMultiplier, 0.0001f);

            eco.AddPower(produced: 0, consumed: 60);
            Assert.IsTrue(eco.IsLowPower);
            Assert.AreEqual(0.5f, eco.ProductionSpeedMultiplier, 0.0001f);

            eco.RemovePower(produced: 0, consumed: 60);
            Assert.IsFalse(eco.IsLowPower);
        }

        [Test]
        public void OneDerrickPaysThreeHundredPerMinute()
        {
            var eco = new PlayerEconomy(0);
            for (int i = 0; i < 60; i++) eco.TickOil(1, 1f);
            Assert.AreEqual(300, eco.Credits);
        }

        [Test]
        public void OilIncomeMultiplierAppliesAndFractionsCarryOver()
        {
            var boosted = new PlayerEconomy(0);
            boosted.TickOil(1, 60f, 1.5f);
            Assert.AreEqual(450, boosted.Credits);

            var eco = new PlayerEconomy(0);
            for (int i = 0; i < 10; i++) eco.TickOil(1, 0.25f); // 12.5 credits, nothing lost
            Assert.AreEqual(12, eco.Credits);
        }

        [Test]
        public void NoDerricksMeansNoIncome()
        {
            var eco = new PlayerEconomy(50);
            Assert.AreEqual(0, eco.TickOil(0, 60f));
            Assert.AreEqual(50, eco.Credits);
        }
    }
}
