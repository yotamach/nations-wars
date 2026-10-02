using UnityEngine;

namespace NationsWars.Game
{
    /// <summary>
    /// Right-click an enemy to attack it, or the ground to move there (units spread out in a block).
    /// </summary>
    public class CommandIssuer : MonoBehaviour
    {
        public float spacing = 2.4f;
        public float enemyClickRadius = 34f;

        void Update()
        {
            if (!RtsInput.RightDown || SelectionManager.Instance == null) return;

            var selected = SelectionManager.Instance.Selected;
            if (selected.Count == 0) return;

            Camera cam = Camera.main;
            if (cam == null) return;

            Unit enemy = EnemyUnderCursor(cam, RtsInput.MousePosition);
            if (enemy != null)
            {
                foreach (Unit u in selected)
                {
                    if (u.Def.weapon != null) u.OrderAttack(enemy);
                    else u.MoveTo(enemy.transform.position); // unarmed units just go there
                }
                return;
            }

            Ray ray = cam.ScreenPointToRay(RtsInput.MousePosition);
            var ground = new Plane(Vector3.up, Vector3.zero);
            float distance;
            if (!ground.Raycast(ray, out distance)) return;
            Vector3 target = ray.GetPoint(distance);

            int cols = Mathf.CeilToInt(Mathf.Sqrt(selected.Count));
            int rows = Mathf.CeilToInt(selected.Count / (float)cols);

            for (int i = 0; i < selected.Count; i++)
            {
                float x = (i % cols - (cols - 1) * 0.5f) * spacing;
                float z = (i / cols - (rows - 1) * 0.5f) * spacing;
                selected[i].MoveTo(target + new Vector3(x, 0f, z));
            }
        }

        Unit EnemyUnderCursor(Camera cam, Vector2 mouse)
        {
            Unit best = null;
            float bestDist = enemyClickRadius * enemyClickRadius;
            foreach (Unit u in Unit.All)
            {
                if (u.IsDead || !SkirmishSession.AreEnemies(SkirmishSession.LocalSlot, u.OwnerSlot)) continue;
                Vector3 s = cam.WorldToScreenPoint(u.transform.position + Vector3.up);
                if (s.z <= 0f) continue;
                float d = ((Vector2)s - mouse).sqrMagnitude;
                if (d < bestDist) { bestDist = d; best = u; }
            }
            return best;
        }
    }
}
