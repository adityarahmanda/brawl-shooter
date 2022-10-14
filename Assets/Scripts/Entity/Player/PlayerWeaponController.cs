using System.Collections;
using UnityEngine;

public class PlayerWeaponController : MonoBehaviour 
{
    [SerializeField] private Weapon m_weapon;
    public Weapon weapon
    {
        get { return m_weapon; }
    }

    [SerializeField] private ParticleSystem m_bulletShooterFX;
    [SerializeField] private ParticleSystem m_muzzleFX;

    private Animator m_animator;
    private PlayerMoveController m_moveController;
    private float m_rotationOffset = .5f;
    
    private Vector3 m_shootingDirection;
    public Vector3 shootingDirection
    {
        get { return m_shootingDirection; }
    }
    
    private bool m_isReadyToShoot = true;
    
    private bool m_isShooting;
    public bool IsShooting
    {
        get { return m_isShooting; }
    }
        
    private void Awake() 
    {
        m_animator = GetComponentInChildren<Animator>();
        m_moveController = GetComponent<PlayerMoveController>();
    }

    private void Start() 
    {
        m_bulletShooterFX.Stop();
        var bulletShooterModule = m_bulletShooterFX.main;
        bulletShooterModule.startLifetime = m_weapon.range / bulletShooterModule.startSpeed.constant;

        if(m_weapon.type == WeaponType.RapidShot)
        {
            bulletShooterModule.duration = m_weapon.timeBetweenShoot;
        }

        m_muzzleFX.Stop();
        var muzzleModule = m_muzzleFX.main;
        muzzleModule.duration = m_weapon.timeBetweenShoot;

        m_weapon.bulletsLeft = m_weapon.magazineSize;

        InputManager.i.AttackInput.onDrag.AddListener(OnDragAttackInput);
        InputManager.i.AttackInput.onPointerUp.AddListener(OnPointerUpAttackInput);
    }

    private void Update() 
    {
        if(m_weapon.bulletsLeft < m_weapon.magazineSize)
        {
            m_weapon.bulletsLeft += (1 / m_weapon.bulletsCooldownTime) * Time.deltaTime;
        }
        else
        {
            m_weapon.bulletsLeft = m_weapon.magazineSize;
        }
    }

    private void Shoot() 
    {
        m_isShooting = true;
        m_isReadyToShoot = false;

        StartCoroutine(ShootCoroutine());
    }

    private IEnumerator ShootCoroutine()
    {
        Quaternion targetRotation = Quaternion.LookRotation(m_shootingDirection, Vector3.up); 
        yield return new WaitUntil(() => m_moveController.root.rotation.eulerAngles.y >= targetRotation.eulerAngles.y - m_rotationOffset && m_moveController.root.rotation.eulerAngles.y <= targetRotation.eulerAngles.y + m_rotationOffset);

        m_animator.SetBool("isShoot", true);
        m_bulletShooterFX.Play();
        m_muzzleFX.Play();
        m_weapon.bulletsLeft--;

        Invoke("ResetShoot", m_weapon.timeBetweenShoot);
    }

    public void ResetShoot()
    {
        m_animator.SetBool("isShoot", false);
        m_isShooting = false;
        m_isReadyToShoot = true;
    }

    private void OnDragAttackInput(Vector2 direction)
    {
        if(!m_isShooting)
        {
            m_shootingDirection = new Vector3(direction.x, 0, direction.y);
        }
    }

    private void OnPointerUpAttackInput()
    {
        if(m_weapon.bulletsLeft >= 1f && m_isReadyToShoot)
        {
            Shoot();
        }
    }
}