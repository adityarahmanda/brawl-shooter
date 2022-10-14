using UnityEngine;
using UnityEngine.Events;
using Cinemachine;

public class LevelManager : MonoBehaviour 
{
    public static LevelManager i;

    [Header("Level Global Reference")]
    public Player player;
    public Tower tower;

    [Header("Level State")]
    [SerializeField] private bool m_levelEnded = false;
    public bool LevelEnded 
    { 
        get { return m_levelEnded; } 
    }
    public UnityEvent onLevelEnded;

    [SerializeField] private float m_timeSurvived;
    public float timeSurvived
    { 
        get { return m_timeSurvived; } 
    }

    [Header("Level Settings")]
    [SerializeField] private CinemachineVirtualCamera m_virtualCamera;
    [SerializeField] private Transform playerSpawnPos;
    [SerializeField] private Player[] playerPrefabs;

    private void Awake() 
    {
        if(i == null)
        {
            i = this;
        }
        else
        {
            Destroy(this);
        }

        for(int i = 0; i < playerPrefabs.Length; i++)
        {
            var playerWeaponController = playerPrefabs[i].gameObject.GetComponent<PlayerWeaponController>();
            if(playerWeaponController.weapon.type == GameManager.i.weaponType)
            {
                player = Instantiate(playerPrefabs[i], playerSpawnPos.position, playerSpawnPos.rotation) as Player;
            }
        }
    }

    private void Start() 
    {
        m_timeSurvived = 0;
        m_virtualCamera.Follow = player.transform;

        player.onDie.AddListener(EndLevel);
        tower.onDestroyed.AddListener(EndLevel);  
    }

    private void Update() 
    {
        if(m_levelEnded) return;

        m_timeSurvived += Time.deltaTime;    
    }

    private void EndLevel()
    {
        m_levelEnded = true;
        onLevelEnded.Invoke();
    }
}