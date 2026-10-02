using NationsWars.Sim;
using UnityEngine;

namespace NationsWars.Game
{
    /// <summary>Connects world space (x, z on the ground plane) to the engine-free NavGrid.</summary>
    public class MapGrid : MonoBehaviour
    {
        public static MapGrid Instance { get; private set; }

        public int width = 128;
        public int height = 128;
        [Tooltip("World units per cell")] public float cellSize = 2f;

        public NavGrid Grid { get; private set; }
        public Vector3 Origin { get; private set; }
        public float HalfExtent { get { return Mathf.Max(width, height) * cellSize * 0.5f; } }

        void Awake()
        {
            Instance = this;
            Grid = new NavGrid(width, height);
            Origin = new Vector3(-width * cellSize * 0.5f, 0f, -height * cellSize * 0.5f);
        }

        void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        public GridPos WorldToCell(Vector3 p)
        {
            int x = Mathf.Clamp(Mathf.FloorToInt((p.x - Origin.x) / cellSize), 0, width - 1);
            int y = Mathf.Clamp(Mathf.FloorToInt((p.z - Origin.z) / cellSize), 0, height - 1);
            return new GridPos(x, y);
        }

        public Vector3 CellToWorld(GridPos c)
        {
            return new Vector3(Origin.x + (c.X + 0.5f) * cellSize, 0f, Origin.z + (c.Y + 0.5f) * cellSize);
        }

        public void BlockBounds(Bounds b)
        {
            GridPos min = WorldToCell(b.min);
            GridPos max = WorldToCell(b.max);
            Grid.BlockRect(min.X, min.Y, max.X - min.X + 1, max.Y - min.Y + 1);
        }
    }
}
