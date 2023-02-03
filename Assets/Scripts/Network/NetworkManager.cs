using Cinemachine;
using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace BrawlShooter
{
    public class NetworkManager : Singleton<NetworkManager>
    {
        [Header("Scene Reference")]
        [SerializeField, ScenePath]
        private string _gameplayScene;

        [Header("Essential Prefabs")]
        [SerializeField]
        private NetworkRunner _runnerPrefab;

        [SerializeField]
        private NetworkPrefabRef _playerPrefab;
        public NetworkPrefabRef playerPrefab => _playerPrefab;

        [SerializeField]
        private PlayerAgent _testingAgentPrefab;
        public PlayerAgent testingAgentPrefab => _testingAgentPrefab;
        
        [Header("Player Manager")]
        public Player localPlayer;
        public Dictionary<PlayerRef, Player> players = new Dictionary<PlayerRef, Player>();
        
        public bool isUsingMultipeer => NetworkProjectConfig.Global.PeerMode == NetworkProjectConfig.PeerModes.Multiple;

        public const int MAX_PLAYERS = 2;
        public const int EXTRA_PEERS = 1;

        private void Start()
        {
            if(SceneManager.GetActiveScene().path == _gameplayScene)
            {
                // auto start multipeer in gameplay scene
                NetworkProjectConfig.Global.PeerMode = NetworkProjectConfig.PeerModes.Multiple;
                StartCoroutine(StartSessionCoroutine(GameMode.Host));
            }
            else
            {
                // force to single peer if the game started outside gameplay scene
                NetworkProjectConfig.Global.PeerMode = NetworkProjectConfig.PeerModes.Single;
            }

            AudioManager.Instance.PlayMusic("bgm", 0f);
        }

        public void StartSession(GameMode gameMode, string sessionName = null)
        {
            StartCoroutine(StartSessionCoroutine(gameMode, sessionName));
        }

        private IEnumerator StartSessionCoroutine(GameMode gameMode, string sessionName = null)
        {
            Debug.Log("Creating Game Session...");

            var scenePath = SceneManager.GetActiveScene().path;
            scenePath = scenePath.Substring("Assets/".Length, scenePath.Length - "Assets/".Length - ".unity".Length);
            SceneRef sceneRef = SceneUtility.GetBuildIndexByScenePath(scenePath);
            
            if(string.IsNullOrEmpty(sessionName))
            {
                sessionName = Guid.NewGuid().ToString();
                Debug.Log($"Session name has been generated = {sessionName}");
            }

            int totalPeers = isUsingMultipeer ? 1 + EXTRA_PEERS : 1;
            for (int i = 0; i < totalPeers; i++)
            {
                var runner = Instantiate(_runnerPrefab);
                DontDestroyOnLoad(runner);
                
                runner.name = totalPeers > 1 ? (i == 0 ? "Host" : "Client") : gameMode.ToString();

                var objectPool = runner.GetComponent<NetworkObjectPool>();
                var sceneManager = runner.GetComponent<NetworkSceneManager>();
                var callbacks = runner.GetComponentsInChildren<NetworkRunnerCallbacks>();
                runner.AddCallbacks(callbacks);

                var initTask = runner.StartGame(new StartGameArgs()
                {
                    GameMode = i == 0 ? gameMode : GameMode.Client,
                    SessionName = sessionName,
                    Scene = sceneRef,
                    ObjectPool = objectPool,
                    SceneManager = sceneManager,
                    PlayerCount = MAX_PLAYERS
                });

                while (initTask.IsCompleted == false)
                {
                    yield return null;
                }

                if (initTask.IsFaulted == true)
                {
                    Shutdown();
                    yield break;
                }

                if (initTask.IsFaulted)
                {
                    Shutdown();
                    yield break;
                }
            }

            EventManager.TriggerEvent(new SessionCreatedEvent() { sessionName = sessionName });
        }

        public void AddPlayer(Player player)
        {
            if (players.ContainsKey(player.Ref)) return;

            if(!isUsingMultipeer && player.HasInputAuthority)
            {
                localPlayer = player;
            }

            players.Add(player.Ref, player);

            if(isUsingMultipeer && players.Count >= MAX_PLAYERS)
            {
                FindObjectOfType<LevelBehaviour>().SpawnAllPlayerAgents();
            }
        }

        public void RemovePlayer(Player player)
        {
            players.Remove(player.Ref);
        }

        public bool IsLocalPlayer(Player player)
        {
            return player.Ref == localPlayer.Ref;
        }

        public void Shutdown()
        {
            foreach (var runner in NetworkRunner.Instances)
            {
                if (runner != null && runner.IsRunning)
                {
                    runner.Shutdown();
                    Destroy(runner.gameObject);
                }
            }
        }

        public void LoadGameplay()
        {
            foreach (var runner in NetworkRunner.Instances)
            {
                if (runner.IsServer && runner.IsRunning)
                {
                    runner.SetActiveScene(_gameplayScene);
                    return;
                }
            }
        }
    }

    public struct SessionCreatedEvent
    {
        public string sessionName;
    }
}
