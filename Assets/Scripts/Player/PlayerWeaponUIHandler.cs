using Fusion;
using UnityEngine;

namespace BrawlShooter
{
    [RequireComponent(typeof(PlayerWeapon))]
    public class PlayerWeaponUIHandler : PlayerAbility
    {
        private PlayerWeapon _playerWeapon;

        [SerializeField]
        private SegmentedProgressBar _weaponClipBar;

        private void Awake()
        {
            _playerWeapon = GetComponent<PlayerWeapon>();
        }

        public override void Spawned()
        {
            _weaponClipBar.numberOfSegments = _playerWeapon.weapon.maxClip;
            _weaponClipBar.Initialize();
        }

        public override void FixedUpdateNetwork()
        {
            UpdateWeaponClipBar();
        }

        public void UpdateWeaponClipBar()
        {
            if (_weaponClipBar == null) return;

            _weaponClipBar.fillAmount = _playerWeapon.weapon.currentClip / _playerWeapon.weapon.maxClip;
        }
    }
}