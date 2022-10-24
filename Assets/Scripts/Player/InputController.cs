using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using BrawlShooter;
using UnityEngine;
using PlayState = BrawlShooter.GameManager.PlayState;

namespace BrawlShooter
{
    /// <summary>
    /// Handle player input by responding to Fusion input polling, filling an input struct and then working with
    /// that input struct in the Fusion Simulation loop.
    /// </summary>
    public class InputController : NetworkBehaviour, INetworkRunnerCallbacks
    {
        public static bool fetchInput = false;

        public bool ToggleReady { get; set; }
        public bool ToggleChangeWeapon { get; set; }

        private bool _fireUp;
        private bool _fireDown;
        private Vector2 _moveDelta;
        private Vector2 _aimDelta;

        [SerializeField] 
        private LayerMask _mouseRayMask;
        
        private Player _player;
        private NetworkInputData _frameworkInput = new NetworkInputData();

        /// <summary>
        /// Hook up to the Fusion callbacks so we can handle the input polling
        /// </summary>
        public override void Spawned()
        {
            //_mobileInput = FindObjectOfType<MobileInput>(true);
            _player = GetComponent<Player>();

            // Technically, it does not really matter which InputController fills the input structure, since the actual data will only be sent to the one that does have authority,
            // but in the name of clarity, let's make sure we give input control to the gameobject that also has Input authority.
            if (Object.HasInputAuthority)
            {
                Runner.AddCallbacks(this);
            }

            Debug.Log("Spawned [" + this + "] IsClient=" + Runner.IsClient + " IsServer=" + Runner.IsServer + " HasInputAuth=" + Object.HasInputAuthority + " HasStateAuth=" + Object.HasStateAuthority);
        }

        /// <summary>
        /// Get Unity input and store them in a struct for Fusion
        /// </summary>
        /// <param name="runner">The current NetworkRunner</param>
        /// <param name="input">The target input handler that we'll pass our data to</param>
        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
            if (_player != null && _player.Object != null && fetchInput)
            {
                // Fill networked input struct with input data
                _frameworkInput.moveDirection = _moveDelta.normalized;
                _frameworkInput.aimDirection = _aimDelta.normalized;

                if (_fireDown)
                {
                    _fireDown = false;
                    _frameworkInput.Buttons |= NetworkInputData.FIRE_DOWN;
                }

                if (_fireUp)
                {
                    _fireUp = false;
                    _frameworkInput.Buttons |= NetworkInputData.FIRE_UP;
                }

                if (ToggleReady)
                {
                    ToggleReady = false;
                    _frameworkInput.Buttons |= NetworkInputData.READY;
                }

                if (ToggleChangeWeapon)
                {
                    ToggleChangeWeapon = false;
                    _frameworkInput.Buttons |= NetworkInputData.CHANGE_WEAPON;
                }
            }

            // Hand over the data to Fusion
            input.Set(_frameworkInput);
            _frameworkInput.Buttons = 0;
        }

        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
        {
        }

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
        }

        private void Update()
        {
            if (GameManager.playState == PlayState.LOBBY)
            {
                ToggleReady |= Input.GetKeyDown(KeyCode.R);
            }

            if (GameManager.playState == PlayState.LEVEL && Input.mousePresent)
            {
                if (Input.GetMouseButtonDown(0))
                    _fireDown = true;

                if (Input.GetMouseButtonUp(0))
                    _fireUp = true;

                _moveDelta = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

                if (_player.IsCalculateAimDirection)
                {
                    Vector3 mousePos = Input.mousePosition;

                    RaycastHit hit;
                    Ray ray = Camera.main.ScreenPointToRay(mousePos);

                    Vector3 mouseCollisionPoint = Vector3.zero;
                    //Raycast towards the mouse collider box in the world
                    if (Physics.Raycast(ray, out hit, Mathf.Infinity, _mouseRayMask))
                    {
                        if (hit.collider != null)
                        {
                            mouseCollisionPoint = hit.point;
                        }
                    }

                    Vector3 aimDirection = mouseCollisionPoint - _player.transform.position;
                    _aimDelta = new Vector2(aimDirection.x, aimDirection.z);
                }
            }
        }

        /// <summary>
        /// FixedUpdateNetwork is the main Fusion simulation callback - this is where
        /// we modify network state.
        /// </summary>
        public override void FixedUpdateNetwork()
        {
            if (GameManager.playState == PlayState.TRANSITION)
                return;

            // Get our input struct and act accordingly. This method will only return data if we
            // have Input or State Authority - meaning on the controlling player or the server.
            if (GetInput(out NetworkInputData input))
            {
                if (input.IsDown(NetworkInputData.FIRE_DOWN))
                {
                    _player.CalculateAimDirection();
                }

                if (input.IsDown(NetworkInputData.FIRE_UP))
                {
                    _player.StartShoot();
                }

                if (input.IsDown(NetworkInputData.READY))
                {
                    _player.ToggleReady();
                }

                if(input.IsDown(NetworkInputData.CHANGE_WEAPON))
                {
                    _player.ToggleWeaponType();
                }

                // We let the NetworkCharacterController do the actual work
                _player.SetMoveDirection(input.moveDirection.normalized);
                _player.SetAimDirection(input.aimDirection.normalized);
            }
            _player.Move();
        }

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
    }

    /// <summary>
    /// Our custom definition of an INetworkStruct. Keep in mind that
    /// * bool does not work (C# does not define a consistent size on different platforms)
    /// * Must be a top-level struct (cannot be a nested class)
    /// * Stick to primitive types and structs
    /// * Size is not an issue since only modified data is serialized, but things that change often should be compact (e.g. button states)
    /// </summary>
    public struct NetworkInputData : INetworkInput
    {
        public const uint FIRE_UP = 1 << 0;
        public const uint FIRE_DOWN = 1 << 1;
        public const uint READY = 1 << 2;
        public const uint CHANGE_WEAPON = 1 << 3;

        public uint Buttons;

        public Vector2 aimDirection;
        public Vector2 moveDirection;

        public bool IsDown(uint button)
        {
            return (Buttons & button) == button;
        }
    }
}