using Fusion;
using TMPro;
using UnityEngine;
using BrawlShooter.FusionHelpers;
using DG.Tweening;
using UnityEngine.SceneManagement;

namespace BrawlShooter
{
    /// <summary>
    /// App entry point and main UI flow management.
    /// </summary>
    public class GameLauncher : MonoBehaviour
    {
        [SerializeField] private GameManager _gameManagerPrefab;
        [SerializeField] private Player _playerPrefab;

        [SerializeField] private TextMeshProUGUI _progressText;
        [SerializeField] private TextMeshProUGUI _errorText;
        [SerializeField] private TMP_InputField _roomInputField;

        private FusionLauncher.ConnectionStatus _status = FusionLauncher.ConnectionStatus.Disconnected;
        private GameMode _gameMode;

        private void Awake()
        {
            DontDestroyOnLoad(this);
        }

        private void Start()
        {
            AudioManager.instance.PlayMusic("bgm", 0);
        }

        private void Update()
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
            UIManager.instance.SwitchPanel(Panel.Type.EnterRoom);
        }

        public void OnEnterRoom()
        {
            FusionLauncher launcher = FindObjectOfType<FusionLauncher>();
            if (launcher == null)
                launcher = new GameObject("Launcher").AddComponent<FusionLauncher>();

            LevelManager lm = FindObjectOfType<LevelManager>();
            lm.launcher = launcher;

            launcher.Launch(_gameMode, _roomInputField.text, lm, OnConnectionStatusUpdate, OnSpawnWorld, OnSpawnPlayer, OnDespawnPlayer);
        }

        private void OnConnectionStatusUpdate(NetworkRunner runner, FusionLauncher.ConnectionStatus status, string reason)
        {
            if (!this)
                return;

            Debug.Log(status);

            _status = status;
            UpdateProgressUI(reason);
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
            if (GameManager.playState != GameManager.PlayState.LOBBY)
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

        private void UpdateProgressUI(string reason)
        {
            bool progress = false;
            bool error = false;

            switch (_status)
            {
                case FusionLauncher.ConnectionStatus.Failed:
                    error = true;
                    break;
                case FusionLauncher.ConnectionStatus.Connecting:
                    _progressText.text = "Connecting";
                    progress = true;
                    break;
                case FusionLauncher.ConnectionStatus.Disconnected:
                    error = true;
                    break;
                case FusionLauncher.ConnectionStatus.Loading:
                    _progressText.text = "Loading";
                    progress = true;
                    break;
            }

            if (progress)
            {
                UIManager.instance.SwitchPanel(Panel.Type.Progress);
                return;
            }

            if (error)
            {
                _errorText.text = "Error - " + reason;
                UIManager.instance.SwitchPanel(Panel.Type.Error);
                return;
            }
        }
    }
}