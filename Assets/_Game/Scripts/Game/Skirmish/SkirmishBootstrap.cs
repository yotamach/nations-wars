using UnityEngine;

namespace NationsWars.Game
{
    /// <summary>Starts the battle: builds the session, places each player's starting units and points the camera.</summary>
    public class SkirmishBootstrap : MonoBehaviour
    {
        public NationDef[] nations;
        public int startingCredits = 10000;
        public RtsCameraController cameraController;
        [Tooltip("Distance from map center to each start position")] public float startRadius = 80f;

        void Start()
        {
            if (nations == null || nations.Length == 0)
            {
                Debug.LogError("SkirmishBootstrap has no nations. Run Nations Wars > Setup Skirmish Prototype.");
                return;
            }

            SkirmishSettings settings = SkirmishSettings.Current ?? SkirmishSettings.CreateDefault(nations, startingCredits);
            SkirmishSession.Begin(settings);

            for (int i = 0; i < settings.slots.Count; i++)
                SpawnStartingUnits(settings.slots[i], i, StartPosition(i, settings.slots.Count));

            if (cameraController != null) cameraController.FocusOn(StartPosition(SkirmishSession.LocalSlot, settings.slots.Count));
        }

        /// <summary>Evenly spaced around the map. Two players start in opposite corners.</summary>
        Vector3 StartPosition(int index, int count)
        {
            float angle = (225f + index * 360f / count) * Mathf.Deg2Rad;
            return new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * startRadius;
        }

        void SpawnStartingUnits(SlotConfig slot, int slotIndex, Vector3 origin)
        {
            var list = slot.nation.startingUnits;
            const int perRow = 5;
            const float gap = 3.6f;

            for (int i = 0; i < list.Count; i++)
            {
                float x = (i % perRow - (perRow - 1) * 0.5f) * gap;
                float z = (i / perRow) * gap;
                UnitFactory.Spawn(list[i], slotIndex, slot.color, origin + new Vector3(x, 0f, z));
            }
        }
    }
}
