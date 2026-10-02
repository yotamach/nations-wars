using NationsWars.Sim;
using UnityEngine;

namespace NationsWars.Game
{
    [CreateAssetMenu(menuName = "Nations Wars/Building", fileName = "NewBuilding")]
    public class BuildingDef : ScriptableObject
    {
        public string displayName = "Building";
        public int cost = 600;
        public float buildTime = 10f;
        public int maxHealth = 800;
        [Tooltip("Size in grid cells")] public Vector2Int footprint = new Vector2Int(2, 2);
        public int powerOutput;
        public int powerDraw;
        public Armor armor = Armor.Building;
        [Tooltip("True for buildings only this nation has")] public bool nationUnique;
        public GameObject prefab;
    }
}
