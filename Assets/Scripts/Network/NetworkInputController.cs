using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BrawlShooter
{
    public class NetworkInputController : NetworkContextBehaviour, INetworkRunnerCallbacks
    {
        public static bool fetchInput = false;

        private NetworkInputData _data = new NetworkInputData();
        private NetworkButtons _previousButton { get; set; }

        private Vector2 _moveDelta;
        private Vector2 _aimDelta;
        
        [SerializeField] 
        private LayerMask _aimMask;

        public UnityEvent<InputContext> OnFetchInput;

        public override void Spawned()
        {
            if (Object.HasInputAuthority)
            {
                Runner.AddCallbacks(this);
            }

            fetchInput = true;
        }

        private void Update()
        {
            _moveDelta = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            Vector3 aimDirection = Vector3.zero;
            if (Physics.Raycast(ray, out hit, 100f, _aimMask))
            {
                if (hit.collider != null)
                {
                    aimDirection = hit.point - transform.position;
                }
            }

            _aimDelta = new Vector2(aimDirection.x, aimDirection.z);
        }

        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
            if (!Object.HasInputAuthority || !fetchInput) return;

            _data = new NetworkInputData();

            _data.buttons.Set(InputButton.Fire, Input.GetMouseButtonDown(0));
            _data.moveDirection = _moveDelta;
            _data.aimDirection = _aimDelta;

            input.Set(_data);
            _data = default;
        }

        public override void FixedUpdateNetwork()
        {
            if (GetInput(out NetworkInputData data))
            {
                var pressed = data.GetButtonPressed(_previousButton);
                var released = data.GetButtonReleased(_previousButton);
                _previousButton = data.buttons;

                OnFetchInput?.Invoke(new InputContext(pressed, released, data));
            }
        }

        #region Unused Network Callbacks
        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) { }
        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
        public void OnConnectedToServer(NetworkRunner runner) { }
        public void OnDisconnectedFromServer(NetworkRunner runner) { }
        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
        public void OnSceneLoadDone(NetworkRunner runner) { }
        public void OnSceneLoadStart(NetworkRunner runner) { }
        #endregion
    }

    public struct NetworkInputData : INetworkInput
    {
        public NetworkButtons buttons;

        public Vector2 aimDirection;
        public Vector2 moveDirection;

        public bool GetButton(InputButton button)
        {
            return buttons.IsSet(button);
        }

        public NetworkButtons GetButtonPressed(NetworkButtons prev)
        {
            return buttons.GetPressed(prev);
        }

        public NetworkButtons GetButtonReleased(NetworkButtons prev)
        {
            return buttons.GetReleased(prev);
        }
    }

    public struct InputContext : INetworkStruct
    {
        [Networked]
        public NetworkButtons pressed { get; set; }
        [Networked]
        public NetworkButtons released { get; set; }
        public NetworkInputData data;

        public InputContext(NetworkButtons pressed, NetworkButtons released, NetworkInputData data)
        {
            this.pressed = pressed;
            this.released = released;
            this.data = data;
        }
    }

    public enum InputButton
    {
        Fire
    }
}