using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using TowerBomber;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager instance { get; private set; }

    [SerializeField] private Transform _lobbyStatusParent;
    [SerializeField] private LobbyStatus _lobbyStatusPrefab;

    private static Dictionary<Player, LobbyStatus> _playerInLobby = new Dictionary<Player, LobbyStatus>();

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
        _playerInLobby.Add(player, lobbyStatus);
    }

    public void RemovePlayer(Player player)
    {
        _playerInLobby.Remove(player);
    }

    public void ClearAllPlayers()
    {
        _playerInLobby.Clear();
    }

    public void Ready()
    {
        if (GameManager.playState != GameManager.PlayState.LOBBY)
            return;

        Player.local.ToggleReady();

        Debug.Log("UwU");
    }
}
