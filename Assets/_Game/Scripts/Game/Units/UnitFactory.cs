using UnityEngine;

namespace NationsWars.Game
{
    /// <summary>Spawns units. Uses the def's prefab when set, otherwise a primitive placeholder in the team color.</summary>
    public static class UnitFactory
    {
        public static Unit Spawn(UnitDef def, int ownerSlot, Color teamColor, Vector3 position)
        {
            GameObject root = def.prefab != null ? Object.Instantiate(def.prefab) : BuildPlaceholder(def, teamColor);
            root.name = def.displayName + " (P" + (ownerSlot + 1) + ")";
            root.transform.position = position;

            GameObject ring = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            ring.name = "SelectionRing";
            Object.Destroy(ring.GetComponent<Collider>());
            float d = def.role == UnitRole.Infantry ? 1.6f : 4f;
            ring.transform.SetParent(root.transform, false);
            ring.transform.localPosition = new Vector3(0f, 0.05f, 0f);
            ring.transform.localScale = new Vector3(d, 0.02f, d);
            ring.GetComponent<Renderer>().sharedMaterial = MaterialUtil.Get(new Color(0.4f, 1f, 0.5f));

            var unit = root.AddComponent<Unit>();
            unit.Init(def, ownerSlot, ring);
            return unit;
        }

        static GameObject BuildPlaceholder(UnitDef def, Color team)
        {
            var root = new GameObject(def.displayName);
            Color dark = new Color(0.16f, 0.17f, 0.19f);

            switch (def.role)
            {
                case UnitRole.Infantry:
                    Part(root, PrimitiveType.Capsule, new Vector3(0f, 0.9f, 0f), new Vector3(0.7f, 0.9f, 0.7f), team);
                    Part(root, PrimitiveType.Cube, new Vector3(0.2f, 1f, 0.35f), new Vector3(0.1f, 0.1f, 0.8f), dark);
                    break;

                case UnitRole.Harvester:
                    Part(root, PrimitiveType.Cube, new Vector3(0f, 0.95f, 0f), new Vector3(2.6f, 1.3f, 4f), new Color(0.85f, 0.65f, 0.17f));
                    Part(root, PrimitiveType.Cube, new Vector3(0f, 1.9f, 1.2f), new Vector3(2.2f, 0.9f, 1.4f), team);
                    Part(root, PrimitiveType.Cube, new Vector3(0f, 0.4f, 0f), new Vector3(3f, 0.6f, 4.4f), dark);
                    break;

                default: // Tank
                    Part(root, PrimitiveType.Cube, new Vector3(0f, 0.35f, 0f), new Vector3(2.6f, 0.7f, 3.6f), dark);
                    Part(root, PrimitiveType.Cube, new Vector3(0f, 0.95f, 0f), new Vector3(2.1f, 0.6f, 3.2f), team);
                    Part(root, PrimitiveType.Cylinder, new Vector3(0f, 1.45f, -0.15f), new Vector3(1.5f, 0.25f, 1.5f), team);
                    Part(root, PrimitiveType.Cube, new Vector3(0f, 1.5f, 1.4f), new Vector3(0.2f, 0.2f, 2.4f), dark);
                    break;
            }
            return root;
        }

        static void Part(GameObject parent, PrimitiveType type, Vector3 localPos, Vector3 scale, Color color)
        {
            GameObject p = GameObject.CreatePrimitive(type);
            Object.Destroy(p.GetComponent<Collider>());
            p.transform.SetParent(parent.transform, false);
            p.transform.localPosition = localPos;
            p.transform.localScale = scale;
            p.GetComponent<Renderer>().sharedMaterial = MaterialUtil.Get(color);
        }
    }
}
