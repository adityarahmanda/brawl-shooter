using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion;
using TowerBomber.FusionHelpers;
using PlayState = TowerBomber.GameManager.PlayState;

namespace TowerBomber
{
    public class LevelManager : NetworkSceneManagerBase
    {
        private Scene _loadedScene;
        [SerializeField] private GameObject _mainSceneEssentials;

        public FusionLauncher launcher { get; set; }

        protected override void Shutdown(NetworkRunner runner)
        {
            base.Shutdown(runner);

            if (_loadedScene != default)
                SceneManager.UnloadSceneAsync(_loadedScene);

            _loadedScene = default;
            PlayerManager.ResetPlayerManager();
        }

        public void LoadLevel(int nextLevelIndex)
        {
            Runner.SetActiveScene(nextLevelIndex);
        }

        protected override IEnumerator SwitchScene(SceneRef prevScene, SceneRef newScene, FinishedLoadingDelegate finished)
        {
            Debug.Log($"Switching Scene from {prevScene} to {newScene}");
            if (newScene <= 0)
            {
                finished(new List<NetworkObject>());
                yield break;
            }

            if (Runner.IsServer)
                GameManager.playState = PlayState.TRANSITION;

            InputController.fetchInput = false;

            if (prevScene > 0)
            {
                yield return new WaitForSeconds(1.0f);

                // Despawn players with a small delay between each one
                Debug.Log("De-spawning all players");
                for (int i = 0; i < PlayerManager.allPlayers.Count; i++)
                {
                    Debug.Log($"de-spawning player character {i}:{PlayerManager.allPlayers[i]}");
                    PlayerManager.allPlayers[i].DespawnCharacter();
                    yield return new WaitForSeconds(0.1f);
                }

                yield return new WaitForSeconds(1.5f);

                //if (winner != -1)
                //{
                //    _scoreManager.UpdateScore(winner, PlayerManager.GetPlayerFromID(winner).score);
                //    yield return new WaitForSeconds(1.5f);
                //    _scoreManager.HideUiScoreAndReset(false);
                //}
                //}
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
            if (newScene >= 0)
            {
                yield return SceneManager.LoadSceneAsync(newScene, LoadSceneMode.Additive);
                _loadedScene = SceneManager.GetSceneByBuildIndex(newScene);
                Debug.Log($"Loaded scene {newScene}: {_loadedScene}");
                sceneObjects = FindNetworkObjects(_loadedScene, disable: false);
            }

            _mainSceneEssentials.SetActive(_loadedScene.buildIndex == 0);

            // Delay one frame
            yield return null;

            launcher.SetConnectionStatus(FusionLauncher.ConnectionStatus.Loaded, "");

            Debug.Log($"Switched Scene from {prevScene} to {newScene} - loaded {sceneObjects.Count} scene objects");
            finished(sceneObjects);

            GameManager.playState = _loadedScene.buildIndex == 0 ? PlayState.LOBBY : PlayState.LEVEL;
            InputController.fetchInput = true;
        }
    }
}