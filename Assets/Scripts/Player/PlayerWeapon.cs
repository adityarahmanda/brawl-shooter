using Fusion;
using UnityEngine;

namespace BrawlShooter
{
    public class PlayerWeapon : PlayerAbility
    {
        public Weapon weapon { get; private set; }

        private Vector3 _aimDirection;

        [Networked]
        public bool isShooting { private get; set; }

        [SerializeField]
        private SegmentedProgressBar _weaponClipBar;

        [SerializeField]
        private AimIndicator _aimIndicatorPrefab;
        private AimIndicator _aimIndicator;

        private float _initRotationSpeed;

        private void Awake()
        {
            weapon = GetComponentInChildren<Weapon>();
        }

        private void OnEnable()
        {
            weapon.OnShootStarted.AddListener(OnShootStarted);
            weapon.OnShootEnded.AddListener(OnShootEnded);
        }

        private void OnDisable()
        {
            weapon.OnShootStarted.RemoveListener(OnShootStarted);
            weapon.OnShootEnded.RemoveListener(OnShootEnded);
        }

        public override void Spawned()
        {
            weapon.SetOwner(this);

            _weaponClipBar.numberOfSegments = weapon.maxClip;
            _weaponClipBar.Initialize();

            _aimIndicator = Instantiate(_aimIndicatorPrefab, transform.position, Quaternion.identity);
            _aimIndicator.range = weapon.range;
            _aimIndicator.follow = transform;
            _aimIndicator.ShowIndicator(false);
        }

        public void LateUpdate()
        {
            _aimIndicator?.UpdateIndicator(_aimDirection);
        }

        public override void FixedUpdateNetwork()
        {
            UpdateWeaponClipBar();
        }

        public void UpdateWeaponClipBar()
        {
            if (_weaponClipBar == null) return;

            _weaponClipBar.fillAmount = weapon.currentClip / weapon.maxClip;
        }

        public void OnShootStarted()
        {
            _initRotationSpeed = Agent.NetworkCharacterController.rotationSpeed;
            Agent.NetworkCharacterController.rotationSpeed = 0;

            Quaternion targetRot = Quaternion.LookRotation(_aimDirection, Vector3.up);
            Agent.NetworkCharacterController.TeleportToRotation(targetRot);

            SetShootingAnimation(true);
        }

        public void OnShootEnded()
        {
            Agent.NetworkCharacterController.rotationSpeed = _initRotationSpeed;
            SetShootingAnimation(false);
        }

        public void SetShootingAnimation(bool value)
        {
            if (IsProxy || !Runner.IsForward) return;

            Agent.NetworkAnimator.Animator.SetBool("isShooting", value);
        }

        public override void OnProcessInput(InputContext context)
        {
            _aimDirection = context.data.aimDirection;
            
            if (context.pressed.IsSet(InputButton.Fire))
            {
                _aimIndicator?.ShowIndicator(true);
            }

            if (context.released.IsSet(InputButton.Fire))
            {
                weapon.OnProcessFire(_aimDirection);
                _aimIndicator?.ShowIndicator(false);
            }
        }
    }
}