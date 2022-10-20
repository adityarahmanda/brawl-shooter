using UnityEditor;
using UnityEngine;

namespace TowerBomber
{
    /// <summary>
    /// SpawnPoint is used by the LevelBehaviour to figure out where it can spawn things.
    /// There is nothing Fusion specific about this.
    /// </summary>
    public class SpawnPoint : MonoBehaviour
    {
        public bool isOccupied;
    }
}