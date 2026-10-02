using UnityEngine;

namespace NationsWars.Game
{
    /// <summary>Blocks the nav grid cells under this object's renderer bounds.</summary>
    [RequireComponent(typeof(Renderer))]
    public class MapObstacle : MonoBehaviour
    {
        void Start()
        {
            if (MapGrid.Instance != null) MapGrid.Instance.BlockBounds(GetComponent<Renderer>().bounds);
        }
    }
}
