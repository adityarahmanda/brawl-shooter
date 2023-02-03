using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using TMPro;
using NanoSockets;
using System.Data;
using UnityEngine.SocialPlatforms;

namespace BrawlShooter
{
    public class LobbyStatus : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _status;

        private void Awake()
        {
            _status = GetComponent<TextMeshProUGUI>();
        }

        public void UpdateStatus(Player player)
        {
            _status.text = player.username + " (" + (player.isReady ? "<color=green>Ready</color>" : "<color=red> Not Ready</color>") + ")";
        }
    }
}
