using UnityEngine;
using Fusion;

namespace BrawlShooter
{
    [RequireComponent(typeof(ProjectileManager))]
    public class Weapon : NetworkContextBehaviour
    {
        public PlayerWeapon Owner { get; private set; }
        public ProjectileManager ProjectileManager { get; private set; }

        public byte damage = 10;
        public float range = 10f;

        [SerializeField]
        private int _projectilesPerShot = 1;

        [Networked]
        public float currentClip { get; private set; }
        
        [SerializeField]
        private float clipReloadTime = 1f;
        
        public int maxClip = 3;

        [Networked]
        private TickTimer _fireCooldown { get; set; }
        
        [SerializeField]
        private float _fireCooldownTime = .3f;

        [SerializeField]
        private Transform _barrelTransform;
        public Transform BarrelTransform => _barrelTransform;

        private void Awake()
        {
            ProjectileManager = GetComponent<ProjectileManager>();
        }

        public override void Spawned()
        {
            currentClip = maxClip;
        }

        public void OnProcessFire(InputContext context)
        {
            if (IsBusy()) return;

            for (int i = 0; i < _projectilesPerShot; i++)
            {
                ProjectileManager.AddProjectile(_barrelTransform.position, context.data.aimDirection);
            }
            
            currentClip--;
            
            _fireCooldown = TickTimer.CreateFromSeconds(Runner, _fireCooldownTime);
        }

        public override void FixedUpdateNetwork()
        {
            if (currentClip < maxClip)
            {
                currentClip += Runner.DeltaTime * (clipReloadTime / 60f);
            }
        }

        public bool IsBusy()
        {
            return currentClip < 1 || !_fireCooldown.ExpiredOrNotRunning(Runner);
        }

        public void SetOwner(PlayerWeapon owner) => Owner = owner;

        //public void Shoot(Vector3 direction)
        //{
        //    if (type == Type.RapidShot && rapidBulletsLeft <= 0)
        //        return;

        //    _muzzleFlashFX.Play();

        //    if (Object.HasStateAuthority)
        //    {
        //        var key = new NetworkObjectPredictionKey { Byte0 = (byte)Object.InputAuthority.RawEncoded, Byte1 = (byte)Runner.Simulation.Tick };
        //        Runner.Spawn(_bulletPrefab, _shootPoint.position, Quaternion.identity, Object.InputAuthority, (runner, obj) =>
        //        {
        //            obj.GetComponent<Bullet>().InitNetworkState(_damage, _bulletSpeed, _range, direction);
        //        }, key);
        //    }

        //    if (type == Type.RapidShot)
        //    {
        //        rapidBulletsLeft--;

        //        if (rapidBulletsLeft <= 0)
        //        {
        //            rapidBulletsLeft = 0;
        //        }
        //    }
        //}
    }
}