using UnityEngine;
using WeaponType = TowerBomber.WeaponController.WeaponType;

namespace TowerBomber
{
    public class WeaponSelector : MonoBehaviour
    {
        [SerializeField] private WeaponShowcase[] _weapons;

        private Animator _animator;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public void UpdateWeapon(WeaponType weaponType)
        {
            foreach(WeaponShowcase weapon in _weapons)
            {
                if (weapon.type == weaponType)
                    weapon.gameObject.SetActive(true);
                else
                    weapon.gameObject.SetActive(false);
            }

            switch (weaponType)
            {
                case WeaponType.SingleShot:
                    _animator.SetTrigger("gun");
                    break;
                case WeaponType.RapidShot:
                    _animator.SetTrigger("machineGun");
                    break;
            }
        }
    }
}
