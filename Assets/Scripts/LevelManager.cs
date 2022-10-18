using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Fusion;
using Cinemachine;
using TowerBomber.FusionHelpers;

namespace TowerBomber
{
    public class LevelManager : NetworkSceneManagerBase
    {
        //public static LevelManager i;

        //[Header("Level Global Reference")]
        //public Player player;
        //public Tower tower;

        private Scene _loadedScene;

        public FusionLauncher launcher { get; set; }

        //[SerializeField] private bool m_levelEnded = false;
        //public bool LevelEnded => m_levelEnded;
        //public UnityEvent onLevelEnded;

        //[SerializeField] private float m_timeSurvived;
        //public float timeSurvived => m_timeSurvived;

        //[Header("Level Settings")]
        //[SerializeField] private CinemachineVirtualCamera m_virtualCamera;
        //[SerializeField] private Transform playerSpawnPos;
        //[SerializeField] private Player[] playerPrefabs;

        private void Awake()
        {
            //_scoreManager = FindObjectOfType<ScoreManager>(true);
            //_readyupManager = FindObjectOfType<ReadyupManager>(true);
            //_countdownManager = FindObjectOfType<CountdownManager>(true);

            //_countdownManager.Reset();
            //_scoreManager.HideLobbyScore();
            //_readyupManager.HideUI();

            //for (int i = 0; i < playerPrefabs.Length; i++)
            //{
            //    var playerWeaponController = playerPrefabs[i].gameObject.GetComponent<PlayerWeaponController>();
            //    if (playerWeaponController.weapon.type == GameManager.instance.weaponType)
            //    {
            //        player = Instantiate(playerPrefabs[i], playerSpawnPos.position, playerSpawnPos.rotation) as Player;
            //    }
            //}
        }

        //protected override void Shutdown(NetworkRunner runner)
        //{
        //    base.Shutdown(runner);
        //    _currentLevel = null;
        //    if (_loadedScene != default)
        //        SceneManager.UnloadSceneAsync(_loadedScene);
        //    _loadedScene = default;
        //    PlayerManager.ResetPlayerManager();
        //}

        private void Start()
        {
            //m_timeSurvived = 0;
            //m_virtualCamera.Follow = player.transform;

            // player.onDie.AddListener(EndLevel);
            // tower.onDestroyed.AddListener(EndLevel);
        }

        //private void Update()
        //{
        //    if (m_levelEnded) return;

        //    m_timeSurvived += Time.deltaTime;
        //}

        //private void EndLevel()
        //{
        //    m_levelEnded = true;
        //    onLevelEnded.Invoke();
        //}

        protected override IEnumerator SwitchScene(SceneRef prevScene, SceneRef newScene, FinishedLoadingDelegate finished)
        {
            Debug.Log($"Switching Scene from {prevScene} to {newScene}");
            if (newScene <= 0)
            {
                finished(new List<NetworkObject>());
                yield break;
            }

            if (Runner.IsServer || Runner.IsSharedModeMasterClient)
                GameManager.playState = GameManager.PlayState.TRANSITION;

            // int winner = GameManager.WinningPlayerIndex;

            if (prevScene > 0)
            {
                yield return new WaitForSeconds(1.0f);

                // InputController.fetchInput = false;

                // Despawn players with a small delay between each one
                Debug.Log("De-spawning all tanks");
                //for (int i = 0; i < PlayerManager.allPlayers.Count; i++)
                //{
                //    Debug.Log($"De-spawning tank {i}:{PlayerManager.allPlayers[i]}");
                //    PlayerManager.allPlayers[i].DespawnTank();
                //    yield return new WaitForSeconds(0.1f);
                //}

                // yield return new WaitForSeconds(1.5f - PlayerManager.allPlayers.Count * 0.1f);

                Debug.Log("Despawned all tanks");
                // Players have despawned

                //if (winner != -1)
                //{
                //    _scoreManager.UpdateScore(winner, PlayerManager.GetPlayerFromID(winner).score);
                //    yield return new WaitForSeconds(1.5f);
                //    _scoreManager.HideUiScoreAndReset(false);
                //}
            }

            //_transitionEffect.ToggleGlitch(true);
            //_audioEmitter.Play();
            //launcher.SetConnectionStatus(FusionLauncher.ConnectionStatus.Loading, "");

            //_scoreManager.HideLobbyScore();

            yield return null;
            Debug.Log($"Start loading scene {newScene} in single peer mode");

            if (_loadedScene != default)
            {
                Debug.Log($"Unloading Scene {_loadedScene.buildIndex}");
                yield return SceneManager.UnloadSceneAsync(_loadedScene);
            }

            _loadedScene = default;
            Debug.Log($"Loading scene {newScene}");

            List<NetworkObject> sceneObjects = new List<NetworkObject>();
            if (newScene >= 0)
            {
                yield return SceneManager.LoadSceneAsync(newScene, LoadSceneMode.Additive);
                _loadedScene = SceneManager.GetSceneByBuildIndex(newScene);
                Debug.Log($"Loaded scene {newScene}: {_loadedScene}");
                sceneObjects = FindNetworkObjects(_loadedScene, disable: false);
            }

            // Delay one frame
            yield return null;

            //launcher.SetConnectionStatus(FusionLauncher.ConnectionStatus.Loaded, "");

            // Activate the next level
            //_currentLevel = FindObjectOfType<LevelBehaviour>();
            //if (_currentLevel != null)
            //    _currentLevel.Activate();
            //MusicPlayer.instance.SetLowPassTranstionDirection(newScene > _lobby ? 1f : -1f);

            Debug.Log($"Switched Scene from {prevScene} to {newScene} - loaded {sceneObjects.Count} scene objects");
            finished(sceneObjects);

            Debug.Log("SwitchScene post effect");

            //if (newScene == _lobby)
            //    _readyupManager.ShowUI();
            //else
            //    _readyupManager.HideUI();

            //yield return new WaitForSeconds(0.3f);

            //Debug.Log($"Stop glitching");
            //_transitionEffect.ToggleGlitch(false);
            //_audioEmitter.Stop();

            //if (winner >= 0 && newScene == _lobby)
            //{
            //    // Show lobby scores and reset the score ui.
            //    _scoreManager.ShowLobbyScore(winner);
            //    _scoreManager.HideUiScoreAndReset(true);
            //}

            // Respawn with slight delay between each player
            Debug.Log($"Respawning All Players");
            //for (int i = 0; i < PlayerManager.allPlayers.Count; i++)
            //{
            //    Player player = PlayerManager.allPlayers[i];
            //    Debug.Log($"Respawning Player {i}:{player}");
            //    player.Respawn(0);
            //    yield return new WaitForSeconds(0.3f);
            //}

            //// Set state to playing level
            //if (_loadedScene.buildIndex == _lobby)
            //{
            //    if (Runner.IsServer || Runner.IsSharedModeMasterClient)
            //        GameManager.playState = GameManager.PlayState.LOBBY;
            //    InputController.fetchInput = true;
            //    Debug.Log($"Switched Scene from {prevScene} to {newScene}");
            //}
            //else
            //{
            //    StartCoroutine(_countdownManager.Countdown(() =>
            //    {
            //        // Set state to playing level
            //        if (Runner != null && (Runner.IsServer || Runner.IsSharedModeMasterClient))
            //        {
            //            GameManager.WinningPlayerIndex = -1;
            //            GameManager.playState = GameManager.PlayState.LEVEL;
            //        }
            //        // Enable inputs after countdow finishes
            //        InputController.fetchInput = true;
            //        Debug.Log($"Switched Scene from {prevScene} to {newScene}");
            //    }));
            //}
        }
    }
}