using UnityEngine;
using Fusion;

namespace BrawlShooter
{
    public class Weapon : NetworkBehaviour
    {
        public enum Type
        {
            SingleShot,
            RapidShot
        };

        [SerializeField]
        private Type _type;
        public Type type => _type;

        [SerializeField]
        private byte _damage = 10;

        [SerializeField]
        private float _shootSpeed = 1f;
        public float shootSpeed => _shootSpeed;

        [SerializeField]
        private float _range = 10f;
        public float range => _range;

        [SerializeField]
        private int _magazineSize = 3;
        public int magazineSize => _magazineSize;

        [SerializeField]
        private float _bulletCooldownTime = .3f;
        public float bulletCooldownTime => _bulletCooldownTime;

        [SerializeField]
        private float _bulletSpeed = 16f;

        [Networked, HideInInspector]
        public float bulletsLeft { get; set; }

        [Networked, HideInInspector]
        public int rapidBulletsLeft { get; set; }

        [SerializeField]
        private int _rapidBulletsCount = 10;
        public int rapidBulletsCount => _rapidBulletsCount;

        [SerializeField]
        private Transform _shootPoint;

        [SerializeField]
        private Bullet _bulletPrefab;

        [SerializeField]
        private ParticleSystem _muzzleFlashFX;

        [SerializeField]
        private AnimatorOverrideController _charAnimControl;
        public AnimatorOverrideController charAnimControl => _charAnimControl;

        public void Shoot(Vector3 direction)
        {
            if (type == Type.RapidShot && rapidBulletsLeft <= 0)
                return;

            _muzzleFlashFX.Play();

            if (Object.HasStateAuthority)
            {
                var key = new NetworkObjectPredictionKey { Byte0 = (byte)Object.InputAuthority.RawEncoded, Byte1 = (byte)Runner.Simulation.Tick };
                Runner.Spawn(_bulletPrefab, _shootPoint.position, Quaternion.identity, Object.InputAuthority, (runner, obj) =>
                {
                    obj.GetComponent<Bullet>().InitNetworkState(_damage, _bulletSpeed, _range, direction);
                }, key);
            }

            if (type == Type.RapidShot)
            {
                rapidBulletsLeft--;
                
                if (rapidBulletsLeft <= 0)
                {
                    rapidBulletsLeft = 0;
                }
            }
        }
    }
}