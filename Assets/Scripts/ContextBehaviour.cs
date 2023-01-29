using Fusion;
using System.Collections.Generic;
using UnityEngine;

namespace BrawlShooter
{
    public abstract class ContextBehaviour : MonoBehaviour
    {
        protected NetworkLauncher Launcher => NetworkLauncher.Instance;
        public NetworkRunner Runner => Launcher.Runner;
        public NetworkSceneManager SceneManager => Launcher.SceneManager;
        public Player Local => Launcher.Local;
        public List<Player> Players => Launcher.Players;

        public bool IsLocal(Player player) => Local.id == player.id;
    }
}