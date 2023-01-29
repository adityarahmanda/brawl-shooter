using Fusion;
using System.Collections.Generic;
using UnityEngine;

namespace BrawlShooter
{
    public abstract class NetworkContextBehaviour : NetworkBehaviour
    {
        public NetworkSceneManager SceneManager => NetworkLauncher.Instance.SceneManager;
        public Player Local => NetworkLauncher.Instance.Local;
        public List<Player> players => NetworkLauncher.Instance.Players;

        public bool IsLocal(Player player) => Local.id == player.id;
    }
}