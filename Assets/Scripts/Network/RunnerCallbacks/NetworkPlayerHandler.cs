using Fusion;
using UnityEngine;

namespace BrawlShooter
{
    public class NetworkPlayerHandler : NetworkRunnerCallbacks
    {
        public override void OnPlayerJoined(NetworkRunner runner, PlayerRef playerref)
        {
            if (!runner.IsServer) return;

            runner.Spawn(Launcher.playerPrefab, Vector3.zero, Quaternion.identity, playerref, OnBeforeSpawned);

            void OnBeforeSpawned(NetworkRunner runner, NetworkObject networkObject)
            {
                networkObject.gameObject.name = playerref.ToString();
                networkObject.transform.parent = Launcher.transform;
            }

            Debug.Log(playerref + " joined");
        }

        public override void OnPlayerLeft(NetworkRunner runner, PlayerRef playerref)
        {
            if (!runner.IsServer) return;

            runner.Despawn(runner.GetPlayerObject(playerref));
            Debug.Log(playerref + " left");
        }
    }
}