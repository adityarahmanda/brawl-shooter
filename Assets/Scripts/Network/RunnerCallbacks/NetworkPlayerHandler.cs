using Fusion;
using UnityEngine;

namespace BrawlShooter
{
    public class NetworkPlayerHandler : NetworkRunnerCallbacks
    {
        public override void OnPlayerJoined(NetworkRunner runner, PlayerRef playerRef)
        {
            if (!runner.IsServer) return;

            var player = runner.Spawn(NetworkManager.Instance.playerPrefab, Vector3.zero, Quaternion.identity, playerRef, OnBeforeSpawned);
            runner.SetPlayerObject(playerRef, player);

            void OnBeforeSpawned(NetworkRunner runner, NetworkObject networkObject)
            {
                networkObject.gameObject.name = playerRef.ToString();
                DontDestroyOnLoad(networkObject.gameObject);
            }

            Debug.Log(playerRef + " joined");
        }

        public override void OnPlayerLeft(NetworkRunner runner, PlayerRef playerref)
        {
            if (!runner.IsServer) return;

            runner.Despawn(runner.GetPlayerObject(playerref));
            Debug.Log(playerref + " left");
        }
    }
}