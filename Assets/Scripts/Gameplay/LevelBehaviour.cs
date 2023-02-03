using Fusion;

namespace BrawlShooter
{
    public class LevelBehaviour : NetworkBehaviour
    {
        private SpawnPoint[] _spawnPoints;

        private void Awake()
        {
            _spawnPoints = FindObjectsOfType<SpawnPoint>();
        }

        public void SpawnAllPlayerAgents()
        {
            if (!Runner.IsServer) return;

            int i = 0;
            foreach (var player in NetworkManager.Instance.players.Values)
            {
                player.SpawnAgent(_spawnPoints[i].transform);
                i++;
            }
        }
    }
}