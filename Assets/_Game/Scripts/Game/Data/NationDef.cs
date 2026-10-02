using System.Collections.Generic;
using UnityEngine;

namespace NationsWars.Game
{
    [CreateAssetMenu(menuName = "Nations Wars/Nation", fileName = "NewNation")]
    public class NationDef : ScriptableObject
    {
        public string displayName = "Nation";
        public string tagline;
        public Color color = Color.white;
        [TextArea] public string passive;
        public string superweapon;
        public List<UnitDef> units = new List<UnitDef>();
        public List<BuildingDef> buildings = new List<BuildingDef>();
        [Tooltip("Units placed at the start position in skirmish until base building is in")]
        public List<UnitDef> startingUnits = new List<UnitDef>();
    }
}
