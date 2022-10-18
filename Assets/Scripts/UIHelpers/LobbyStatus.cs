using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using TMPro;
using NanoSockets;
using System.Data;
using UnityEngine.SocialPlatforms;

namespace TowerBomber
{
    public class LobbyStatus : MonoBehaviour
    {
        private Player _player;

        [SerializeField] private TextMeshProUGUI _status;

        private void Awake()
        {
            _status = GetComponent<TextMeshProUGUI>();
        }

        public void Init(Player player)
        {
            _player = player;
            UpdateStatus();
        }

        public void UpdateStatus()
        {
            _status.text = "Player " + _player.playerID + " (" + (_player.ready ? "<color=green>Ready</color>" : "<color=red> Not Ready</color>") + ")";
        }
    }
}
