using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelMenu : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject m_mainPanel;
    [SerializeField] private GameObject m_gameOverPanel;
    
    [Header("UI Reference")]
    [SerializeField] private TextMeshProUGUI m_timeSurvivedText;
    [SerializeField] private TextMeshProUGUI m_timeSurvivedScoreText;
    [SerializeField] private Button m_restartButton;
    [SerializeField] private Button m_mainMenuButton;

    private void Start() 
    {
        m_mainPanel.SetActive(true);
        m_gameOverPanel.SetActive(false);

        LevelManager.i.onLevelEnded.AddListener(() => {
            var timeSurvived = LevelManager.i.timeSurvived.ToString("0.00");
            m_timeSurvivedScoreText.SetText($"Time Survived: {timeSurvived}s");
            m_mainPanel.SetActive(false);
            m_gameOverPanel.SetActive(true);
        });

        m_restartButton.onClick.AddListener(() => GameManager.i.StartGame());
        m_mainMenuButton.onClick.AddListener(() => GameManager.i.ReturnToMainMenu());
    }

    private void Update() 
    {
        if(LevelManager.i.LevelEnded) return;

        var timeSurvived = LevelManager.i.timeSurvived.ToString("0.00");
        m_timeSurvivedText.SetText($"Time Survived: {timeSurvived}s");    
    }
}
