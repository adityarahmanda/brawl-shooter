using UnityEngine;
using UnityEngine.UI;

namespace TowerBomber
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private Button m_playButton;

        private void Start()
        {
            // m_playButton.onClick.AddListener(GameManager.instance.StartGame);
        }
    }
}