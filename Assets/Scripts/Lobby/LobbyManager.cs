using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace BrawlShooter
{
    public class LobbyManager : MonoBehaviour
    {
        public static LobbyManager instance { get; private set; }

        [SerializeField] private Transform _lobbyStatusParent;
        [SerializeField] private LobbyStatus _lobbyStatusPrefab;
        [SerializeField] private WeaponSelector _weaponSelector;
        [SerializeField] private TextMeshProUGUI _readyText;

        private static Dictionary<Player, LobbyStatus> _lobbyStatuses = new Dictionary<Player, LobbyStatus>();

        private void Awake()
        {
            if (instance == null)
                instance = this;
            else
                Destroy(this);
        }

        public void AddPlayer(Player player)
        {
            LobbyStatus lobbyStatus = Instantiate(_lobbyStatusPrefab, _lobbyStatusParent);
            lobbyStatus.Init(player);
            _lobbyStatuses.Add(player, lobbyStatus);

            if (player.Object.HasInputAuthority)
            {
                _readyText.text = "READY";
                _weaponSelector.UpdateWeapon(player.weaponType);
            }
        }

        public void RemovePlayer(Player player)
        {
            if (player == null || !_lobbyStatuses.ContainsKey(player))
                return;

            Debug.Log("Removing Player " + player.playerID + " from lobby");
            Destroy(_lobbyStatuses[player].gameObject);
            _lobbyStatuses.Remove(player);
        }

        public void ChangeWeapon()
        {
            foreach (InputController ic in FindObjectsOfType<InputController>())
            {
                if (ic.Object.HasInputAuthority)
                    ic.ToggleChangeWeapon = true;
            }
        }

        public void Ready()
        {
            foreach (InputController ic in FindObjectsOfType<InputController>())
            {
                if (ic.Object.HasInputAuthority)
                    ic.ToggleReady = true;
            }
        }

        public void UpdatePlayerLobbyStatus(Player player)
        {
            _lobbyStatuses[player].UpdateStatus();

            if (player.Object.HasInputAuthority)
                _readyText.text = Player.local.IsReady ? "CANCEL READY" : "READY";
        }

        public void UpdatePlayerWeapon(Player player)
        {
            if (player.Object.HasInputAuthority)
                _weaponSelector.UpdateWeapon(player.weaponType);
        }
    }
}
