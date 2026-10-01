using NationsWars.Sim;
using UnityEngine;

namespace NationsWars.Game
{
    /// <summary>A short-lived line from shooter to target. Placeholder until real projectiles and VFX.</summary>
    public static class Tracer
    {
        public static void Spawn(Vector3 from, Vector3 to, Warhead warhead)
        {
            var go = new GameObject("Tracer");
            var line = go.AddComponent<LineRenderer>();
            line.positionCount = 2;
            line.SetPosition(0, from);
            line.SetPosition(1, to);
            bool heavy = warhead == Warhead.Cannon || warhead == Warhead.Explosive;
            line.startWidth = line.endWidth = heavy ? 0.25f : 0.1f;
            line.sharedMaterial = MaterialUtil.Get(heavy ? new Color(1f, 0.6f, 0.2f) : new Color(1f, 0.92f, 0.5f));
            Object.Destroy(go, heavy ? 0.12f : 0.07f);
        }
    }
}
