using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion;
using BrawlShooter.FusionHelpers;

namespace BrawlShooter
{
    public class LevelManager : NetworkSceneManagerBase
    {
        public FusionLauncher launcher { get; set; }

        private Scene _loadedScene;
        private LevelBehaviour _currentLevel;

        [SerializeField] private GameObject _mainSceneEssentials;
        [SerializeField] private WinnerUI _winnerUI;

        [SerializeField] private ProgressBar _healthbarPrefab;
        [SerializeField] private BulletBar _bulletbarPrefab;
        [SerializeField] private WeaponIndicator _weaponIndicatorPrefab;

        private int _returnToLobbyCountdown = 5;

        protected override void Shutdown(NetworkRunner runner)
        {
            base.Shutdown(runner);

            StartCoroutine(OnShutdownCoroutine());
        }

        private IEnumerator OnShutdownCoroutine()
        {
            if (_loadedScene != default)
                yield return SceneManager.UnloadSceneAsync(_loadedScene);

            _loadedScene = default;
            PlayerManager.ResetPlayerManager();
            _mainSceneEssentials.SetActive(true);
        }

        public void LoadLevel(int nextLevelIndex)
        {
            Runner.SetActiveScene(nextLevelIndex);
        }

        protected override IEnumerator SwitchScene(SceneRef prevScene, SceneRef newScene, FinishedLoadingDelegate finished)
        {
            Debug.Log($"Switching Scene from {prevScene} to {newScene}");
            if (newScene < 0)
            {
                finished(new List<NetworkObject>());
                yield break;
            }

            if (Runner.IsServer)
                GameManager.playState = GameManager.PlayState.TRANSITION;

            InputController.fetchInput = false;

            if (prevScene > 0)
            {
                if (GameManager.instance.winnerID != -1)
                {
                    _winnerUI.SetWinner(PlayerManager.GetPlayerFromID(GameManager.instance.winnerID));
                    _winnerUI.SetReturnToLobbyCountdown(_returnToLobbyCountdown);
                    UIManager.instance.SwitchPanel(Panel.Type.WinnerUI);

                    yield return new WaitForSeconds(1f);

                    for (int i = _returnToLobbyCountdown - 1; i > 0; i--)
                    {
                        _winnerUI.SetReturnToLobbyCountdown(i);
                        yield return new WaitForSeconds(1f);
                    }

                    _winnerUI.SetVisible(false);
                }

                // Despawn players with a small delay between each one
                Debug.Log("De-spawning all players");
                for (int i = 0; i < PlayerManager.allPlayers.Count; i++)
                {
                    Debug.Log($"de-spawning player character {i}:{PlayerManager.allPlayers[i]}");
                    PlayerManager.allPlayers[i].DespawnCharacter();
                    PlayerManager.allPlayers[i].state = Player.State.Despawned;
                    yield return new WaitForSeconds(0.1f);
                }
            }

            launcher.SetConnectionStatus(FusionLauncher.ConnectionStatus.Loading, "");

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
            if (newScene > 0)
            {
                yield return SceneManager.LoadSceneAsync(newScene, LoadSceneMode.Additive);
                _loadedScene = SceneManager.GetSceneByBuildIndex(newScene);
                Debug.Log($"Loaded scene {newScene}: {_loadedScene}");
                sceneObjects = FindNetworkObjects(_loadedScene, disable: false);
                UIManager.instance.CloseAllPanels();
            }

            _mainSceneEssentials.SetActive(_loadedScene == default);

            // Delay one frame
            yield return null;

            launcher.SetConnectionStatus(FusionLauncher.ConnectionStatus.Loaded, "");

            if (_loadedScene == default)
            {
                StartCoroutine(AudioManager.instance.PlayMusicFade("bgm", 1f));

                for (int i = 0; i < PlayerManager.allPlayers.Count; i++)
                {
                    LobbyManager.instance.UpdatePlayerLobbyStatus(PlayerManager.allPlayers[i]);
                }

                if(launcher.status == FusionLauncher.ConnectionStatus.Disconnected || launcher.status == FusionLauncher.ConnectionStatus.Failed)
                    UIManager.instance.SwitchPanel(Panel.Type.Intro);
                else
                    UIManager.instance.SwitchPanel(Panel.Type.Lobby);
            }
            else
            {
                StartCoroutine(AudioManager.instance.StopMusicFade(1f));

                _currentLevel = FindObjectOfType<LevelBehaviour>();

                // Respawn with slight delay between each player
                Debug.Log($"Respawning All Players");
                for (int i = 0; i < PlayerManager.allPlayers.Count; i++)
                {
                    Player player = PlayerManager.allPlayers[i];
                    Debug.Log($"Spawning Player {i}:{player} character");

                    player.transform.position = _currentLevel.GetPlayerSpawnPoint();
                    player.SpawnCharacter(_healthbarPrefab, _bulletbarPrefab, _weaponIndicatorPrefab);

                    if (player.Object.HasInputAuthority)
                    {
                        _currentLevel.SetCameraToFollow(player.transform);
                    }

                    player.state = Player.State.Active;

                    yield return new WaitForSeconds(0.3f);
                }
            }

            Debug.Log($"Switched Scene from {prevScene} to {newScene} - loaded {sceneObjects.Count} scene objects");
            finished(sceneObjects);

            if (Runner.IsServer)
                GameManager.playState = _loadedScene == default ? GameManager.PlayState.LOBBY : GameManager.PlayState.LEVEL;

            InputController.fetchInput = true;
        }
    }
}
