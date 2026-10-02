using NationsWars.Sim;
using UnityEngine;

namespace NationsWars.Game
{
    [CreateAssetMenu(menuName = "Nations Wars/Weapon", fileName = "NewWeapon")]
    public class WeaponDef : ScriptableObject
    {
        public string displayName = "Weapon";
        public Warhead warhead = Warhead.Bullet;
        public int damage = 10;
        [Tooltip("World units")] public float range = 10f;
        [Tooltip("Seconds between shots")] public float cooldown = 1f;
    }
}
