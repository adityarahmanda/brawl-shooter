using UnityEngine;
using Fusion;
using PlayState = TowerBomber.GameManager.PlayState;

namespace TowerBomber
{
    public class WeaponController : NetworkBehaviour
    {
        [Networked(OnChanged = nameof(OnWeaponTypeChanged))]
        public WeaponType weaponType { get; set; }

        public enum WeaponType
        {
            SingleShot,
            RapidShot
        };

        [Range(0, 30f)] public float range = 10f;

        [SerializeField, Range(0, 10)] 
        private int m_magazineSize = 3;
        public int magazineSize => m_magazineSize;

        [SerializeField, Range(0, 10f)] 
        private float m_bulletsCooldownTime = .3f;
        public float bulletsCooldownTime => m_bulletsCooldownTime;

        [SerializeField, Range(0, 1f)] 
        private float m_timeBetweenShoot = .3f;
        public float timeBetweenShoot => m_timeBetweenShoot;

        [Networked] 
        public float bulletsLeft { get; set; }

        // [SerializeField] private ParticleSystem m_bulletShooterFX;
        [SerializeField] private ParticleSystem m_muzzleFX;

        private Animator m_animator;
        private PlayerMoveController m_moveController;
        private float m_rotationOffset = .5f;

        private Vector3 m_shootingDirection;
        public Vector3 shootingDirection => m_shootingDirection;

        private bool m_isReadyToShoot = true;

        private bool m_isShooting;
        public bool IsShooting => m_isShooting;

        //private void Awake()
        //{
        //    m_animator = GetComponentInChildren<Animator>();
        //    m_moveController = GetComponent<PlayerMoveController>();
        //}

        //public override void Spawned()
        //{
            //m_bulletShooterFX.Stop();
            //var bulletShooterModule = m_bulletShooterFX.main;
            //bulletShooterModule.startLifetime = _weapon.range / bulletShooterModule.startSpeed.constant;

            //if (_weapon.type == WeaponType.RapidShot)
            //{
            //    bulletShooterModule.duration = _weapon.timeBetweenShoot;
            //}

            //m_muzzleFX.Stop();
            //var muzzleModule = m_muzzleFX.main;
            //muzzleModule.duration = _weapon.timeBetweenShoot;

            //_weapon.bulletsLeft = _weapon.magazineSize;

            //InputManager.i.AttackInput.onDrag.AddListener(OnDragAttackInput);
            //InputManager.i.AttackInput.onPointerUp.AddListener(OnPointerUpAttackInput);
        //}

        //private void Update()
        //{
        //    if (_weapon.bulletsLeft < _weapon.magazineSize)
        //    {
        //        _weapon.bulletsLeft += (1 / _weapon.bulletsCooldownTime) * Time.deltaTime;
        //    }
        //    else
        //    {
        //        _weapon.bulletsLeft = _weapon.magazineSize;
        //    }
        //}

        //private void Shoot()
        //{
        //    m_isShooting = true;
        //    m_isReadyToShoot = false;

        //    StartCoroutine(ShootCoroutine());
        //}

        //private IEnumerator ShootCoroutine()
        //{
        //    Quaternion targetRotation = Quaternion.LookRotation(m_shootingDirection, Vector3.up);
        //    yield return new WaitUntil(() => m_moveController.root.rotation.eulerAngles.y >= targetRotation.eulerAngles.y - m_rotationOffset && m_moveController.root.rotation.eulerAngles.y <= targetRotation.eulerAngles.y + m_rotationOffset);

        //    _animator.SetBool("isShoot", true);
        //    //m_bulletShooterFX.Play();
        //    m_muzzleFX.Play();
        //    _weapon.bulletsLeft--;

        //    Invoke("ResetShoot", _weapon.timeBetweenShoot);
        //}

        //public void ResetShoot()
        //{
        //    _animator.SetBool("isShoot", false);
        //    m_isShooting = false;
        //    m_isReadyToShoot = true;
        //}

        //private void OnDragAttackInput(Vector2 direction)
        //{
        //    if (!m_isShooting)
        //    {
        //        m_shootingDirection = new Vector3(direction.x, 0, direction.y);
        //    }
        //}

        //private void OnPointerUpAttackInput()
        //{
        //    if (_weapon.bulletsLeft >= 1f && m_isReadyToShoot)
        //    {
        //        Shoot();
        //    }
        //}

        public void ChangeWeaponType()
        {
            weaponType = weaponType == WeaponType.SingleShot ? WeaponType.RapidShot : WeaponType.SingleShot;
        }

        public static void OnWeaponTypeChanged(Changed<WeaponController> changed)
        {
            if (changed.Behaviour)
                changed.Behaviour.OnWeaponTypeChanged();
        }

        public void OnWeaponTypeChanged()
        {
            LobbyManager.instance.UpdateWeapon(this);
        }
    }
}