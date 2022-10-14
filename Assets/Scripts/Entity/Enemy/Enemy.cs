using UnityEngine;
using UnityEngine.AI;

public class Enemy : Entity
{
    [SerializeField] private LayerMask m_whatIsPlayer, m_whatIsTower;
    [SerializeField] private float m_sightRange;
    [SerializeField] private float m_attackRange;

    [SerializeField] private Transform m_root;
    [SerializeField, Range(0f, 1f)] protected float m_rotationSmooth = .3f;

    private NavMeshAgent m_agent;
    private Animator m_animator;
    private Transform m_player, m_tower;

    private bool m_oldPlayerInSightRange, m_playerInSightRange;
    private bool m_playerInAttackRange, m_towerInAttackRange;
    private bool m_isExploding;

    private void Awake() 
    {
        m_agent = GetComponent<NavMeshAgent>();
        m_animator = GetComponentInChildren<Animator>();
    }

    private void Start() 
    {
        m_agent.updateRotation = false;
        
        if(LevelManager.i.player != null)
        {
            m_player = LevelManager.i.player.transform;
        }
        
        if(LevelManager.i.tower != null)
        {
            m_tower = LevelManager.i.tower.transform;
        }

        NavigateToTower();

        LevelManager.i.onLevelEnded.AddListener(Explode);
    }

    private void Update() 
    {
        if(m_isExploding || LevelManager.i.LevelEnded) return;

        float velocity = m_agent.velocity.magnitude;

        if (velocity > 0f) {
            m_animator.SetBool("isMove", true);
        } else {
            m_animator.SetBool("isMove", false);
        }

        Quaternion targetRotation = Quaternion.LookRotation(m_agent.velocity, Vector3.up);
        m_root.transform.rotation = Quaternion.Slerp(m_root.transform.rotation, targetRotation, m_rotationSmooth);

        if(m_player != null)
        {
            m_playerInSightRange = Physics.CheckSphere(transform.position, m_sightRange, m_whatIsPlayer);
            m_playerInAttackRange = Physics.CheckSphere(transform.position, m_attackRange, m_whatIsPlayer);
        }

        if(m_tower != null)
        {
            m_towerInAttackRange = Physics.CheckSphere(transform.position, m_attackRange, m_whatIsTower);
        }

        if((m_playerInSightRange && m_playerInAttackRange) || m_towerInAttackRange)
        {
            Explode();
        }

        if(m_playerInSightRange && !m_playerInAttackRange)
        {
            ChasePlayer();
        }

        // on exit player sight
        if(m_oldPlayerInSightRange && !m_playerInSightRange)
        {
            NavigateToTower();
        }

        m_oldPlayerInSightRange = m_playerInSightRange;
    }

    private void NavigateToTower()
    {
        if(m_tower == null) return;

        m_agent.SetDestination(m_tower.position);
    }

    private void ChasePlayer()
    {
        if(m_player == null) return;

        m_agent.SetDestination(m_player.position);
    }

    private void Explode()
    {
        m_isExploding = true;

        m_agent.ResetPath();
        m_animator.SetTrigger("explode");

        var currentClipInfo = m_animator.GetCurrentAnimatorClipInfo(0);
        float currentClipLength = currentClipInfo[0].clip.length;

        Invoke("Die", currentClipLength);
    }

    private void OnDrawGizmosSelected() 
    {    
        Gizmos.color = Color.yellow;    
        Gizmos.DrawWireSphere(transform.position, m_sightRange);
        Gizmos.color = Color.red;    
        Gizmos.DrawWireSphere(transform.position, m_attackRange);    
    }
}
