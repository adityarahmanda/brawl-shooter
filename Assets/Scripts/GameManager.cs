using Fusion;
using TowerBomber.FusionHelpers;
using UnityEngine;

namespace TowerBomber
{
    public class GameManager : NetworkBehaviour, IStateAuthorityChanged
    {
        public static GameManager instance;

        public enum PlayState
        {
            LOBBY,
            LEVEL,
            TRANSITION
        }

        [Networked, SerializeField]
        private PlayState networkedPlayState { get; set; }

        public static PlayState playState
        {
            get => (instance != null && instance.Object != null && instance.Object.IsValid) ? instance.networkedPlayState : PlayState.LOBBY;
            set
            {
                if (instance != null && instance.Object != null && instance.Object.IsValid)
                    instance.networkedPlayState = value;
            }
        }

        public const ShutdownReason ShutdownReason_GameAlreadyRunning = (ShutdownReason)100;

        private int m_lastSurvivedTime = 0;
        public int lastSurvivedTime => m_lastSurvivedTime;

        private bool _restart;

        private LevelManager _levelManager;

        public override void Spawned()
        {
            // We only want one GameManager
            if (instance)
                Runner.Despawn(Object); // TODO: I've never seen this happen - do we really need this check?
            else
            {
                instance = this;

                // Find managers and UI
                _levelManager = FindObjectOfType<LevelManager>(true);

                if (playState != PlayState.LOBBY)
                {
                    Debug.Log("Rejecting Player, game is already running!");
                    _restart = true;
                }
            }
        }

        private void Update()
        {
            if (_restart || Input.GetKeyDown(KeyCode.Escape))
            {
                Restart(_restart ? ShutdownReason_GameAlreadyRunning : ShutdownReason.Ok);
                return;
            }
            PlayerManager.HandleNewPlayers();
        }

        public void Restart(ShutdownReason shutdownReason)
        {
            if (!Runner.IsShutdown)
            {
                // Calling with destroyGameObject false because we do this in the OnShutdown callback on FusionLauncher
                Runner.Shutdown(false, shutdownReason);
                instance = null;
                _restart = false;
            }
        }

        // Transition from lobby to level
        public void OnAllPlayersReady()
        {
            if (playState != PlayState.LOBBY)
                return;

            Debug.Log("All players are ready");

            // Reset stats and transition to level.
            Debug.Log($"Resetting all player's stats");
            for (int i = 0; i < PlayerManager.allPlayers.Count; i++)
            {
                PlayerManager.allPlayers[i].ResetStats();
            }

            // close and hide the session from matchmaking / lists. this demo does not allow late join.
            Runner.SessionInfo.IsOpen = false;
            Runner.SessionInfo.IsVisible = false;

            // load level scene
            LoadLevel(1);
        }


        private void LoadLevel(int nextLevelIndex)
        {
            if (!Object.HasStateAuthority)
                return;

            // Reset lives and transition to level
            //ResetLives();

            //// Reset players ready state so we don't launch immediately
            for (int i = 0; i < PlayerManager.allPlayers.Count; i++)
                PlayerManager.allPlayers[i].ResetReady();

            _levelManager.LoadLevel(nextLevelIndex);
        }

        public void StateAuthorityChanged()
        {
            Debug.Log($"State Authority of GameManager changed: {Object.StateAuthority}");
        }
    }
}