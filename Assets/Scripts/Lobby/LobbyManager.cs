using System.Collections.Generic;
using UnityEngine;
using TMPro;
using PlayState = TowerBomber.GameManager.PlayState;

namespace TowerBomber
{
    public class LobbyManager : MonoBehaviour
    {
        public static LobbyManager instance { get; private set; }

        [SerializeField] private Transform _lobbyStatusParent;
        [SerializeField] private LobbyStatus _lobbyStatusPrefab;

        [SerializeField] private WeaponSelector _weaponSelector;
        public WeaponSelector weaponSelector => weaponSelector;

        [SerializeField] private TextMeshProUGUI _ready;
        private bool _allPlayersReady;

        private static Dictionary<Player, LobbyStatus> _lobbyStatuses = new Dictionary<Player, LobbyStatus>();

        private void Awake()
        {
            if (instance == null)
                instance = this;
            else
                Destroy(this);
        }

        private void Update()
        {
            if (_allPlayersReady || GameManager.playState == PlayState.LEVEL)
                return;

            _allPlayersReady = PlayerManager.allPlayers.Count >= 1;
            foreach (Player player in PlayerManager.allPlayers)
            {
                if (!player.ready)
                    _allPlayersReady = false;
            }

            if (_allPlayersReady)
            {
                GameManager.instance.OnAllPlayersReady();
                ClearLobby();
            }
        }

        public void AddPlayer(Player player)
        {
            LobbyStatus lobbyStatus = Instantiate(_lobbyStatusPrefab, _lobbyStatusParent);
            lobbyStatus.Init(player);
            _lobbyStatuses.Add(player, lobbyStatus);
        }

        public void RemovePlayer(Player player)
        {
            if (player == null || !_lobbyStatuses.ContainsKey(player))
                return;

            Debug.Log("Removing Player " + player.playerID + " from lobby");
            Destroy(_lobbyStatuses[player].gameObject);
            _lobbyStatuses.Remove(player);
        }

        public void ClearLobby()
        {
            if (_lobbyStatuses.Count <= 0)
                return;

            Debug.Log("Clearing Lobby");
            foreach (LobbyStatus lobbyStatus in _lobbyStatuses.Values)
            {
                Destroy(lobbyStatus.gameObject);
            }
            _lobbyStatuses.Clear();
        }

        public void ChangeWeapon()
        {
            if (GameManager.playState != PlayState.LOBBY)
                return;

            foreach (InputController ic in FindObjectsOfType<InputController>())
            {
                if (ic.Object.HasInputAuthority)
                    ic.ToggleChangeWeapon = true;
            }
        }

        public void Ready()
        {
            if (GameManager.playState != PlayState.LOBBY)
                return;

            foreach (InputController ic in FindObjectsOfType<InputController>())
            {
                if (ic.Object.HasInputAuthority)
                    ic.ToggleReady = true;
            }
        }

        public void UpdatePlayerLobbyStatus(Player player)
        {
            if (GameManager.playState != PlayState.LOBBY)
                return;

            _lobbyStatuses[player].UpdateStatus();

            if (player.Object.HasInputAuthority)
                _ready.text = Player.local.ready ? "CANCEL READY" : "READY";
        }

        public void UpdateWeapon(WeaponController weaponController)
        {
            if (GameManager.playState != PlayState.LOBBY)
                return;

            if (weaponController.Object.HasInputAuthority)
                _weaponSelector.UpdateWeapon(weaponController.weaponType);
        }
    }
}
