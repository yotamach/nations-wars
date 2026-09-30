using NationsWars.Sim;
using UnityEngine;

namespace NationsWars.Game
{
    public enum UnitRole { Infantry, Tank, Harvester }

    [CreateAssetMenu(menuName = "Nations Wars/Unit", fileName = "NewUnit")]
    public class UnitDef : ScriptableObject
    {
        public string displayName = "Unit";
        public UnitRole role = UnitRole.Infantry;
        public int cost = 100;
        [Tooltip("Seconds to build at full power")] public float buildTime = 5f;
        public int maxHealth = 100;
        [Tooltip("World units per second")] public float moveSpeed = 4f;
        public Armor armor = Armor.Infantry;
        public WeaponDef weapon;
        [Tooltip("Optional. Leave empty to use the built-in placeholder model.")] public GameObject prefab;
    }
}
