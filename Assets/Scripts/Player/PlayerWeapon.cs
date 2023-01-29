using BrawlShooter;
using Fusion;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BrawlShooter
{
    public class PlayerWeapon : PlayerAbility
    {
        public Weapon weapon { get; private set; }

        [Networked]
        public Vector2 aimDirection { get; private set; }
        private bool _isCalculateAimDirection;

        [Networked]
        public NetworkBool isShooting { private get; set; }

        private BulletBar _bulletbar;
        private WeaponIndicator _weaponIndicator;

        private float _initRotSpeed;

        private void Awake()
        {
            weapon = GetComponentInChildren<Weapon>();
        }

        public override void Spawned()
        {
            weapon.SetOwner(this);
        }

        public override void FixedUpdateNetwork()
        {
            // Reload();
            HandleAnimation();
        }

        public override void Render()
        {
            if (!Object.HasInputAuthority) return;

            if (_isCalculateAimDirection)
            {
                _weaponIndicator?.SetDirection(aimDirection);
                // _bulletbar?.SetProgress(currWeapon._bulletsLeft, currWeapon.magazineSize);
            }
        }

        public void HandleAnimation()
        {
            if (IsProxy || !Runner.IsForward) return;

            // Character.NetworkAnimator.Animator.SetFloat("shootSpeed", currWeapon.shootSpeed);
            Character.NetworkAnimator.Animator.SetBool("isShooting", isShooting);
        }

        public override void OnProcessInput(InputContext context)
        {
            aimDirection = context.data.aimDirection;

            if (context.pressed.IsSet(InputButton.Fire))
            {
                weapon.OnProcessFire(context);
            }
        }

        //public void StartShoot()
        //{
        //    if (Object.HasInputAuthority)
        //        _weaponIndicator.Activate(false);

        //    if (!IsReadyToShoot)
        //        return;

        //    _isCalculateAimDirection = false;

        //    _initRotSpeed = Character.NetworkCharacterController.rotationSpeed;
        //    Character.NetworkCharacterController.rotationSpeed = 0;

        //    Quaternion targetRotation = Quaternion.LookRotation(new Vector3(aimDirection.x, 0, aimDirection.y), Vector3.up);
        //    transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, targetRotation.eulerAngles.y, transform.rotation.eulerAngles.z);

        //    currWeapon._bulletsLeft--;
        //    isShooting = true;
        //}

        //public void Shoot()
        //{
        //    currWeapon.Shoot(aimDirection);

        //    if (Object.HasInputAuthority)
        //        AudioManager.Instance.PlaySound2D("gunshot");
        //}

        //public void CheckEndShoot()
        //{
        //    if (currWeapon.type == Weapon.Type.SingleShot)
        //    {
        //        EndShoot();
        //    }
        //    else if (currWeapon.type == Weapon.Type.RapidShot)
        //    {
        //        if (currWeapon.rapidBulletsLeft <= 0)
        //        {
        //            EndShoot();
        //        }
        //    }
        //}

        //public void EndShoot()
        //{
        //    Character.NetworkCharacterController.rotationSpeed = _initRotSpeed;
        //    isShooting = false;
        //}

        //private void Reload()
        //{
        //    if (currWeapon == null) return;

        //    if (!isShooting && currWeapon.bulletsLeft < currWeapon.magazineSize)
        //    {
        //        _lastBulletsLeft = (int)currWeapon.bulletsLeft;
        //        currWeapon.bulletsLeft += (1 / currWeapon.bulletCooldownTime) * Runner.DeltaTime;

        //        if ((int)currWeapon.bulletsLeft > _lastBulletsLeft)
        //        {
        //            if (Object.HasInputAuthority)
        //                AudioManager.Instance.PlaySound2D("reload");
        //        }

        //        if (currWeapon.bulletsLeft > currWeapon.magazineSize)
        //        {
        //            currWeapon.bulletsLeft = currWeapon.magazineSize;
        //        }
        //    }
        //}

        //public static void OnIsShootingChanged(Changed<PlayerCharacter> changed)
        //{
        //    if (changed.Behaviour)
        //        changed.Behaviour.OnIsShootingChanged();
        //}

        //public void OnIsShootingChanged()
        //{
        //    NetworkAnimator.Animator.SetFloat("shootSpeed", _currWeapon.shootSpeed);
        //    NetworkAnimator.Animator.SetBool("isShooting", _isShooting);
        //}
    }
}