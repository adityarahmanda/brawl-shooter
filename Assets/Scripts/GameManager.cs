using Cinemachine;
using System.Collections.Generic;
using UnityEngine;

namespace BrawlShooter
{
    public class GameManager : NetworkSingleton<GameManager>
    {
        [SerializeField]
        private CinemachineVirtualCamera _virtualCamera;

        private SpawnPoint[] _spawnPoints;

        public List<Player> Players => NetworkLauncher.Instance.Players;

        protected override void Awake()
        {
            base.Awake();

            _spawnPoints = FindObjectsOfType<SpawnPoint>();
        }

        public override void Spawned()
        {
            if (Object.HasStateAuthority)
            {
                int i = 0;
                foreach (var player in Players)
                {
                    var agent = player.SpawnAgent(_spawnPoints[i].transform.position, _spawnPoints[i].transform.rotation);
                    
                    if (agent.HasInputAuthority)
                    {
                        _virtualCamera.Follow = agent.transform;
                    }

                    i++;
                }
            }
        }
    }
}