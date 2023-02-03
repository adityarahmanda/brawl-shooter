using UnityEngine;
using TMPro;

namespace BrawlShooter
{
    public class WinnerUI : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI winnerText;

        [SerializeField]
        private TextMeshProUGUI returnToLobbyText;

        public void SetWinner(Player player)
        {
            winnerText.text = player.username + " Wins";
        }

        public void SetReturnToLobbyCountdown(int countdown)
        {
            returnToLobbyText.text = "Returning to Lobby in " + countdown;
        }
    }
}
