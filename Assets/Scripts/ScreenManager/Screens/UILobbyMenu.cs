using Fusion;
using Lean.Pool;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BrawlShooter
{
    public class UILobbyMenu : TweenScreen
    {
        [Header("Lobby Settings")]
        [SerializeField]
        private Transform _lobbyStatusRoot;

        [SerializeField]
        private LobbyStatus _lobbyStatusPrefab;

        [SerializeField]
        private TextMeshProUGUI _roomNameText;

        [SerializeField]
        private Button _readyButton;

        [SerializeField] 
        private TextMeshProUGUI _readyText;

        private Dictionary<PlayerRef, LobbyStatus> _lobbyStatuses = new Dictionary<PlayerRef, LobbyStatus>();

        private void OnEnable()
        {
            EventManager.AddEventListener<PlayerSpawnedEvent>(OnPlayerSpawned);
            EventManager.AddEventListener<PlayerDespawnedEvent>(OnPlayerDespawned);
            EventManager.AddEventListener<PlayerReadyEvent>(OnPlayerReady);

            _readyButton.onClick.AddListener(ToggleReady);
        }

        private void OnDisable()
        {
            EventManager.RemoveEventListener<PlayerSpawnedEvent>(OnPlayerSpawned);
            EventManager.RemoveEventListener<PlayerDespawnedEvent>(OnPlayerDespawned);
            EventManager.RemoveEventListener<PlayerReadyEvent>(OnPlayerReady);

            _readyButton.onClick.RemoveListener(ToggleReady);
        }

        private void AddLobbyStatus(Player player)
        {
            if (_lobbyStatuses.ContainsKey(player.Ref)) return;

            var lobbyStatus = LeanPool.Spawn(_lobbyStatusPrefab, _lobbyStatusRoot);
            _lobbyStatuses.Add(player.Ref, lobbyStatus);

            lobbyStatus.UpdateStatus(player);
        }

        private void RemoveLobbyStatus(Player player)
        {
            LobbyStatus lobbyStatus;
            if(_lobbyStatuses.TryGetValue(player.Ref, out lobbyStatus)) 
            {
                LeanPool.Despawn(lobbyStatus);
                _lobbyStatuses.Remove(player.Ref);
            }
        }

        private void UpdateLobbyStatus(Player player)
        {
            LobbyStatus lobbyStatus;
            if (_lobbyStatuses.TryGetValue(player.Ref, out lobbyStatus))
            {
                lobbyStatus.UpdateStatus(player);
            }
        }

        private void UpdateReadyText(Player player) => _readyText.text = player.isReady ? "CANCEL READY" : "READY";

        protected override void OnShow()
        {
            foreach (var player in Players)
            {
                AddLobbyStatus(player);
            }
        }

        private void OnPlayerSpawned(PlayerSpawnedEvent e) => AddLobbyStatus(e.player);
        private void OnPlayerDespawned(PlayerDespawnedEvent e) => RemoveLobbyStatus(e.player);
        private void OnPlayerReady(PlayerReadyEvent e)
        {
            var player = e.player;

            if (IsLocal(player))
            {
                UpdateReadyText(player);
            }

            UpdateLobbyStatus(player);
            CheckAllPlayersReady();
        }

        public void SetRoomName(string roomName) => _roomNameText.text = roomName;
        private void ToggleReady() => Local.RPC_ToggleReady();

        public void CheckAllPlayersReady()
        {
            if (!Runner.IsServer) return;

            foreach(var player in Players)
            {
                if (!player.isReady) return;
            }

            RPC_StartGame();
        }

        [Rpc]
        public void RPC_StartGame()
        {
            EventManager.TriggerEvent(new GameStartedEvent());
            ScreenManager.Instance.Show<UILoadingMenu>().StartLoading(LoadingType.StartGame);
        }
    }

    public struct GameStartedEvent { }
}