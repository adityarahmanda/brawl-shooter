using Fusion;
using Fusion.Sockets;
using UnityEngine;

namespace BrawlShooter
{
    public class NetworkConnectionHandler : NetworkRunnerCallbacks
    {
        public override void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
        {
            request.Accept();
        }

        public override void OnConnectedToServer(NetworkRunner runner)
        {
            Debug.Log($"{runner.name} Connected to server");
        }

        public override void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
        {
            Debug.Log($"{runner.name} Connect failed {reason}");
        }

        public override void OnDisconnectedFromServer(NetworkRunner runner)
        {
            Debug.Log($"{runner.name} Disconnected from server");
        }

        public override void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
            Debug.Log($"Shutting Down {runner.name}...");
            
            string message = "";
            switch (shutdownReason)
            {
                //case GameManager.ShutdownReason_GameAlreadyRunning:
                //    message = "Game in this room already started!";
                //    break;
                case ShutdownReason.IncompatibleConfiguration:
                    message = "This room already exist in a different game mode!";
                    break;
                case ShutdownReason.Ok:
                    message = "User terminated network session!";
                    break;
                case ShutdownReason.Error:
                    message = "Unknown network error!";
                    break;
                case ShutdownReason.ServerInRoom:
                    message = "There is already a server/host in this room";
                    break;
                case ShutdownReason.DisconnectedByPluginLogic:
                    message = "The Photon server plugin terminated the network session!";
                    break;
                default:
                    message = shutdownReason.ToString();
                    break;
            }
            Debug.Log(message);
        }
    }
}