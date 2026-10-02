using System;
using NationsWars.Sim;
using NUnit.Framework;

namespace NationsWars.Tests
{
    public class DamageTableTests
    {
        readonly DamageTable table = DamageTable.Default;

        [Test]
        public void CannonsAreFullDamageAgainstHeavyArmorAndUselessAgainstAircraft()
        {
            Assert.AreEqual(100, table.Percent(Warhead.Cannon, Armor.HeavyArmor));
            Assert.AreEqual(0, table.Percent(Warhead.Cannon, Armor.Aircraft));
            Assert.AreEqual(0, table.Calculate(40, Warhead.Cannon, Armor.Aircraft));
        }

        [Test]
        public void RiflesAreWeakAgainstHeavyArmor()
        {
            Assert.AreEqual(8, table.Calculate(8, Warhead.Bullet, Armor.Infantry));
            Assert.AreEqual(2, table.Calculate(8, Warhead.Bullet, Armor.HeavyArmor));
        }

        [Test]
        public void FireBeatsInfantryAndExplosivesBeatBuildings()
        {
            Assert.AreEqual(150, table.Percent(Warhead.Fire, Armor.Infantry));
            Assert.AreEqual(150, table.Percent(Warhead.Explosive, Armor.Building));
            Assert.AreEqual(36, table.Calculate(40, Warhead.Explosive, Armor.Infantry));
        }

        [Test]
        public void VeterancyRaisesDamageAndLowersDamageTaken()
        {
            Assert.AreEqual(48, table.Calculate(40, Warhead.Cannon, Armor.HeavyArmor, attackerRank: 1));
            Assert.AreEqual(64, table.Calculate(40, Warhead.Cannon, Armor.HeavyArmor, attackerRank: 3));
            Assert.AreEqual(25, table.Calculate(40, Warhead.Cannon, Armor.HeavyArmor, defenderRank: 3));
        }

        [Test]
        public void RanksAboveHeroicDoNotStack()
        {
            Assert.AreEqual(
                table.Calculate(40, Warhead.Cannon, Armor.HeavyArmor, attackerRank: 3),
                table.Calculate(40, Warhead.Cannon, Armor.HeavyArmor, attackerRank: 9));
        }

        [Test]
        public void AHitThatCanDamageAlwaysDoesAtLeastOne()
        {
            Assert.AreEqual(1, table.Calculate(1, Warhead.Bullet, Armor.HeavyArmor));
        }

        [Test]
        public void EveryWarheadArmorPairIsDefinedAndNotNegative()
        {
            foreach (Warhead w in Enum.GetValues(typeof(Warhead)))
                foreach (Armor a in Enum.GetValues(typeof(Armor)))
                    Assert.IsTrue(table.Percent(w, a) >= 0);
        }
    }
}
