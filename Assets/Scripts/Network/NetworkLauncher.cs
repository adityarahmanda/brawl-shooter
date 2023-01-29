using BrawlShooter.FusionHelpers;
using Fusion;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BrawlShooter
{
    [RequireComponent(typeof(NetworkRunner), typeof(NetworkPool), typeof(NetworkSceneManager))]
    public class NetworkLauncher : Singleton<NetworkLauncher>
    {
        public NetworkRunner Runner { get; private set; }
        public NetworkPool ObjectPool { get; private set; }
        public NetworkSceneManager SceneManager { get; private set; }
        public string gameplaySceneName = "GameplayScene";

        public Player Local { get; set; }        
        public List<Player> Players = new List<Player> ();
        public NetworkPrefabRef playerPrefab;

        private NetworkRunnerCallbacks[] _callbacks;

        protected override void Awake()
        {
            base.Awake();

            Runner = GetComponent<NetworkRunner>();
            ObjectPool = GetComponent<NetworkPool>();
            SceneManager = GetComponent<NetworkSceneManager>();

            _callbacks = GetComponentsInChildren<NetworkRunnerCallbacks>();
        }

        private void OnEnable()
        {
            foreach (var callback in _callbacks)
            {
                Runner.AddCallbacks(callback);
            }

            EventManager.AddEventListener<PlayerSpawnedEvent>(OnPlayerSpawned);
            EventManager.AddEventListener<PlayerDespawnedEvent>(OnPlayerDespawned);
        }

        private void OnDisable()
        {
            foreach (var callback in _callbacks)
            {
                Runner.RemoveCallbacks(callback);
            }

            EventManager.RemoveEventListener<PlayerSpawnedEvent>(OnPlayerSpawned);
            EventManager.RemoveEventListener<PlayerDespawnedEvent>(OnPlayerDespawned);
        }

        public async void Launch(GameMode gameMode, string sessionName)
        {
            if (Runner == null || ObjectPool == null) return;

            Runner.name = name;
            Runner.ProvideInput = gameMode != GameMode.Server;

            await Runner.StartGame(new StartGameArgs()
            {
                GameMode = gameMode,
                SessionName = sessionName,
                PlayerCount = 2,
                ObjectPool = ObjectPool,
                SceneManager = SceneManager
            });

            EventManager.TriggerEvent(new SessionCreatedEvent() 
            { 
                sessionName = Runner.SessionInfo.Name
            });
        }

        public void LoadGameplay() => Runner.SetActiveScene(gameplaySceneName);

        private void OnPlayerSpawned(PlayerSpawnedEvent e) => Players.Add(e.player);
        private void OnPlayerDespawned(PlayerDespawnedEvent e) => Players.Remove(e.player);
        
        public static void Shutdown() => Instance.Runner.Shutdown(false);
    }

    public struct SessionCreatedEvent 
    {
        public string sessionName;
    }
}