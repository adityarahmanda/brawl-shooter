using Fusion;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

namespace BrawlShooter
{
    public class NetworkSceneManager : NetworkSceneManagerDefault
    {
        protected override IEnumerator SwitchSceneSinglePeer(SceneRef prevScene, SceneRef newScene, FinishedLoadingDelegate finished) 
        {
            yield return base.SwitchSceneSinglePeer(prevScene, newScene, finished);
            EventManager.TriggerEvent(new SceneLoadedEvent());
        }

        protected override IEnumerator SwitchSceneMultiplePeer(SceneRef prevScene, SceneRef newScene, FinishedLoadingDelegate finished) 
        {
            Scene activeScene = SceneManager.GetActiveScene();

            bool canTakeOverActiveScene = prevScene == default && IsScenePathOrNameEqual(activeScene, newScene);

            LogTrace($"Start loading scene {newScene} in multi peer mode");
            var loadSceneParameters = new LoadSceneParameters(LoadSceneMode.Additive);

            var sceneToUnload = Runner.MultiplePeerUnityScene;
            var tempSceneSpawnedPrefabs = Runner.IsMultiplePeerSceneTemp ? sceneToUnload.GetRootGameObjects() : Array.Empty<GameObject>();

            if (canTakeOverActiveScene && NetworkRunner.GetRunnerForScene(activeScene) == null && SceneManager.sceneCount > 1)
            {
                LogTrace("Going to attempt to unload the initial scene as it needs a separate Physics stage");
                yield return UnloadSceneAsync(activeScene);
            }

            if (SceneManager.sceneCount == 1 && tempSceneSpawnedPrefabs.Length == 0)
            {
                // can load non-additively, stuff will simply get unloaded
                LogTrace($"Only one scene remained, going to load non-additively");
                loadSceneParameters.loadSceneMode = LoadSceneMode.Single;
            }
            else if (sceneToUnload.IsValid())
            {
                // need a new temp scene here; otherwise calls to PhysicsStage will fail
                if (Runner.TryMultiplePeerAssignTempScene())
                {
                    LogTrace($"Unloading previous scene: {sceneToUnload}, temp scene created");
                    yield return UnloadSceneAsync(sceneToUnload);
                }
            }

            LogTrace($"Loading scene {newScene} with parameters: {JsonUtility.ToJson(loadSceneParameters)}");

            Scene loadedScene = default;
            yield return LoadSceneAsync(newScene, loadSceneParameters, scene => loadedScene = scene);

            LogTrace($"Loaded scene {newScene} with parameters: {JsonUtility.ToJson(loadSceneParameters)}: {loadedScene}");

            if (!loadedScene.IsValid())
            {
                throw new InvalidOperationException($"Failed to load scene {newScene}: async op failed");
            }

            var sceneObjects = FindNetworkObjects(loadedScene, disable: true, addVisibilityNodes: true);

            // unload temp scene
            var tempScene = Runner.MultiplePeerUnityScene;
            Runner.MultiplePeerUnityScene = loadedScene;
            if (tempScene.IsValid())
            {
                if (tempSceneSpawnedPrefabs.Length > 0)
                {
                    LogTrace($"Temp scene has {tempSceneSpawnedPrefabs.Length} spawned prefabs, need to move them to the loaded scene.");
                    foreach (var go in tempSceneSpawnedPrefabs)
                    {
                        Assert.Check(go.GetComponent<NetworkObject>(), $"Expected {nameof(NetworkObject)} on a GameObject spawned on the temp scene {tempScene.name}");
                        SceneManager.MoveGameObjectToScene(go, loadedScene);
                    }
                }
                LogTrace($"Unloading temp scene {tempScene}");
                yield return UnloadSceneAsync(tempScene);
            }

            finished(sceneObjects);

            EventManager.TriggerEvent(new SceneLoadedEvent());
        }
    }

    public struct SceneLoadedEvent { }
}
