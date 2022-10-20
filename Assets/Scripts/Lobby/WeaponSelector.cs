using UnityEngine;

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

        public void UpdateWeapon(Weapon.Type weaponType)
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
                case Weapon.Type.SingleShot:
                    _animator.SetTrigger("gun");
                    break;
                case Weapon.Type.RapidShot:
                    _animator.SetTrigger("machineGun");
                    break;
            }
        }
    }
}
