using System.Collections.Generic;
using NationsWars.Sim;
using UnityEngine;

namespace NationsWars.Game
{
    /// <summary>A controllable unit. Moves along an A* path across the nav grid.</summary>
    public class Unit : MonoBehaviour
    {
        public static readonly List<Unit> All = new List<Unit>();

        public UnitDef Def { get; private set; }
        public int OwnerSlot { get; private set; }
        public int Health { get; private set; }
        public bool IsSelected { get; private set; }

        readonly List<GridPos> path = new List<GridPos>();
        int pathIndex;
        GameObject selectionRing;

        void OnEnable() { All.Add(this); }
        void OnDisable() { All.Remove(this); }

        public void Init(UnitDef def, int ownerSlot, GameObject ring)
        {
            Def = def;
            OwnerSlot = ownerSlot;
            Health = def.maxHealth;
            selectionRing = ring;
            SetSelected(false);
        }

        public void SetSelected(bool selected)
        {
            IsSelected = selected;
            if (selectionRing != null) selectionRing.SetActive(selected);
        }

        public void MoveTo(Vector3 world)
        {
            path.Clear();
            pathIndex = 0;

            MapGrid map = MapGrid.Instance;
            if (map == null) return;

            List<GridPos> found = AStar.FindPath(map.Grid, map.WorldToCell(transform.position), map.WorldToCell(world));
            if (found != null) path.AddRange(found);
        }

        public bool IsMoving { get { return pathIndex < path.Count; } }

        void Update()
        {
            if (!IsMoving) return;

            Vector3 target = MapGrid.Instance.CellToWorld(path[pathIndex]);
            target.y = transform.position.y;

            Vector3 to = target - transform.position;
            float dist = to.magnitude;
            float step = Def.moveSpeed * Time.deltaTime;

            if (dist <= step)
            {
                transform.position = target;
                pathIndex++;
                return;
            }

            Vector3 dir = to / dist;
            transform.position += dir * step;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(dir), 360f * Time.deltaTime);
        }
    }
}
