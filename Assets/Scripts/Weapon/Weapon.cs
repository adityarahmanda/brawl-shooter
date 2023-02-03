using Fusion;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace BrawlShooter
{
    [RequireComponent(typeof(ProjectileManager))]
    public class Weapon : NetworkBehaviour
    {
        public PlayerWeapon Owner { get; private set; }
        public ProjectileManager ProjectileManager { get; private set; }

        public byte damage = 10;
        public float range = 10f;

        [SerializeField]
        private int _projectilesPerShot = 1;
        private int _projectilesShotCount = 0;

        [Networked]
        private Vector3 _aimDirection { get; set; }

        [Networked]
        public float currentClip { get; private set; }

        [Networked]
        public NetworkBool isShooting { get; private set; }

        [SerializeField]
        private float clipReloadTime = 1f;
        
        public int maxClip = 3;

        [Networked]
        private TickTimer _fireCooldown { get; set; }

        [Networked]
        private TickTimer _shootRateTimer { get; set; }

        [SerializeField]
        private float _shootRateTime = .1f;

        [SerializeField]
        private Transform _barrelTransform;
        public Transform BarrelTransform => _barrelTransform;

        public UnityEvent OnShootStarted;
        public UnityEvent OnShootEnded;

        private void Awake()
        {
            ProjectileManager = GetComponent<ProjectileManager>();
        }

        public override void Spawned()
        {
            currentClip = maxClip;
        }

        public void OnProcessFire(Vector3 aimDirection)
        {
            if (IsShootAllowed()) return;
            
            _aimDirection = aimDirection;
            _projectilesShotCount = 0;
            currentClip--;
            isShooting = true;
            
            OnShootStarted.Invoke();
        }

        public override void FixedUpdateNetwork()
        {
            OnShooting();
            OnReloading();
        }

        private void OnShooting()
        {
            if (!isShooting) return;

            if (_shootRateTimer.ExpiredOrNotRunning(Runner))
            {
                ProjectileManager.AddProjectile(_barrelTransform.position, _aimDirection, range);
                _projectilesShotCount++;

                if (_projectilesShotCount >= _projectilesPerShot)
                {
                    isShooting = false;
                    OnShootEnded.Invoke();
                }
                else
                {
                    _shootRateTimer = TickTimer.CreateFromSeconds(Runner, _shootRateTime);
                }
            }
        }

        private void OnReloading()
        {
            if (isShooting) return;

            if (currentClip < maxClip)
            {
                currentClip += Runner.DeltaTime * clipReloadTime;
            }
        }

        public bool IsShootAllowed()
        {
            return currentClip < 1 || isShooting;
        }

        public void SetOwner(PlayerWeapon owner) => Owner = owner;
    }
}