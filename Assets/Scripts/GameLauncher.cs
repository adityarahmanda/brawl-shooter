using Fusion;
using TMPro;
using UnityEngine;
using TowerBomber.FusionHelpers;
using PlayState = TowerBomber.GameManager.PlayState;

namespace TowerBomber
{
    /// <summary>
    /// App entry point and main UI flow management.
    /// </summary>
    public class GameLauncher : MonoBehaviour
    {
        [SerializeField] private GameManager _gameManagerPrefab;
        [SerializeField] private Player _playerPrefab;

        [SerializeField] private TextMeshProUGUI _progress;
        [SerializeField] private TMP_InputField _room;
        [SerializeField] private Panel _uiStart;
        [SerializeField] private Panel _uiProgress;
        [SerializeField] private Panel _uiRoom;
        [SerializeField] private Panel _uiLobby;
        [SerializeField] private Panel _uiGame;

        private FusionLauncher.ConnectionStatus _status = FusionLauncher.ConnectionStatus.Disconnected;
        private GameMode _gameMode;

        private void Awake()
        {
            DontDestroyOnLoad(this);
        }

        private void Start()
        {
            OnConnectionStatusUpdate(null, FusionLauncher.ConnectionStatus.Disconnected, "");
        }

        private void Update()
        {
            if (_uiProgress.isShowing)
            {
                if (Input.GetKeyUp(KeyCode.Escape))
                {
                    NetworkRunner runner = FindObjectOfType<NetworkRunner>();
                    if (runner != null && !runner.IsShutdown)
                    {
                        // Calling with destroyGameObject false because we do this in the OnShutdown callback on FusionLauncher
                        runner.Shutdown(false);
                    }
                }
                UpdateUI();
            }
        }

        public void OnHostOptions()
        {
            SetGameMode(GameMode.Host);
        }

        public void OnJoinOptions()
        {
            SetGameMode(GameMode.Client);
        }

        private void SetGameMode(GameMode gamemode)
        {
            _gameMode = gamemode;
            if (GateUI(_uiStart))
                _uiRoom.SetVisible(true);
        }

        public void OnEnterRoom()
        {
            if (GateUI(_uiRoom))
            {
                FusionLauncher launcher = FindObjectOfType<FusionLauncher>();
                if (launcher == null)
                    launcher = new GameObject("Launcher").AddComponent<FusionLauncher>();

                LevelManager lm = FindObjectOfType<LevelManager>();
                lm.launcher = launcher;

                launcher.Launch(_gameMode, _room.text, lm, OnConnectionStatusUpdate, OnSpawnWorld, OnSpawnPlayer, OnDespawnPlayer);
            }
        }

        private void OnConnectionStatusUpdate(NetworkRunner runner, FusionLauncher.ConnectionStatus status, string reason)
        {
            if (!this)
                return;

            Debug.Log(status);

            _status = status;
            UpdateUI();
        }

        private void OnSpawnWorld(NetworkRunner runner)
        {
            Debug.Log("Spawning GameManager");
            runner.Spawn(_gameManagerPrefab, Vector3.zero, Quaternion.identity, null, InitNetworkState);
            
            void InitNetworkState(NetworkRunner runner, NetworkObject world)
            {
                world.transform.parent = transform;
            }
        }

        private void OnSpawnPlayer(NetworkRunner runner, PlayerRef playerref)
        {
            if (GameManager.playState != PlayState.LOBBY)
            {
                Debug.Log("Not Spawning Player - game has already started");
                return;
            }

            Debug.Log($"Spawning player {playerref} in Lobby");
            runner.Spawn(_playerPrefab, Vector3.zero, Quaternion.identity, playerref, InitNetworkState);

            void InitNetworkState(NetworkRunner runner, NetworkObject networkObject)
            {
                Player player = networkObject.gameObject.GetComponent<Player>();
                player.state = Player.State.InLobby;
                LobbyManager.instance.UpdatePlayerWeapon(player);
            }

            GameManager.instance.OnPlayerJoinedAndExitLobby();
        }

        private void OnDespawnPlayer(NetworkRunner runner, PlayerRef playerref)
        {
            Debug.Log($"Despawning Player {playerref}");
            Player player = PlayerManager.Get(playerref);
            player.TriggerDespawn();

            GameManager.instance.OnPlayerJoinedAndExitLobby();
        }

        private bool GateUI(Panel ui)
        {
            if (!ui.isShowing)
                return false;
            ui.SetVisible(false);
            return true;
        }

        private void UpdateUI()
        {
            bool intro = false;
            bool progress = false;
            bool running = false;

            switch (_status)
            {
                case FusionLauncher.ConnectionStatus.Disconnected:
                    intro = true;
                    break;
                case FusionLauncher.ConnectionStatus.Failed:
                    intro = true;
                    break;
                case FusionLauncher.ConnectionStatus.Connecting:
                    _progress.text = "Connecting";
                    progress = true;
                    break;
                case FusionLauncher.ConnectionStatus.Connected:
                    running = true;
                    break;
                case FusionLauncher.ConnectionStatus.Loading:
                    _progress.text = "Loading";
                    progress = true;
                    break;
                case FusionLauncher.ConnectionStatus.Loaded:
                    running = true;
                    break;
            }

            _uiStart.SetVisible(intro);
            _uiProgress.SetVisible(progress);
            _uiLobby.SetVisible(GameManager.playState == PlayState.LOBBY && running);
            _uiGame.SetVisible(GameManager.playState == PlayState.LEVEL && running);
        }
    }
}