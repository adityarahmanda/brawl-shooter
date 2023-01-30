using BrawlShooter;
using Fusion;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BrawlShooter
{
    public class PlayerWeapon : PlayerAbility
    {
        public Weapon weapon { get; private set; }

        private Vector2 _aimDirection;
        public bool _isAiming;

        [Networked]
        public NetworkBool isShooting { private get; set; }

        [SerializeField]
        private SegmentedProgressBar _weaponClipBar;

        [SerializeField]
        private AimIndicator _aimIndicatorPrefab;
        private AimIndicator _aimIndicator;

        private void Awake()
        {
            weapon = GetComponentInChildren<Weapon>();
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
            HandleAnimation();
        }

        public void UpdateWeaponClipBar()
        {
            if (_weaponClipBar == null) return;

            _weaponClipBar.fillAmount = weapon.currentClip / weapon.maxClip;
        }

        public void HandleAnimation()
        {
            if (IsProxy || !Runner.IsForward) return;

            Agent.NetworkAnimator.Animator.SetBool("isShooting", isShooting);
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
                weapon.OnProcessFire(context);
                _aimIndicator?.ShowIndicator(false);
            }
        }
    }
}