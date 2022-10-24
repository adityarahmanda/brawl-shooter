using UnityEngine;
using Fusion;
using PlayState = BrawlShooter.GameManager.PlayState;

namespace BrawlShooter
{
    public class WeaponController : NetworkBehaviour
    {
        

        // [SerializeField] private ParticleSystem m_bulletShooterFX;
        // [SerializeField] private ParticleSystem m_muzzleFX;

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

        //_weapon._bulletsLeft = _weapon.magazineSize;

        //InputManager.i.AttackInput.onDrag.AddListener(OnDragAttackInput);
        //InputManager.i.AttackInput.onPointerUp.AddListener(OnPointerUpAttackInput);
        //}

        //private void Update()
        //{
        //    if (_weapon._bulletsLeft < _weapon.magazineSize)
        //    {
        //        _weapon._bulletsLeft += (1 / _weapon.bulletsCooldownTime) * Time.deltaTime;
        //    }
        //    else
        //    {
        //        _weapon._bulletsLeft = _weapon.magazineSize;
        //    }
        //}

        //private void Shoot()
        //{
        //    _isShooting = true;
        //    _isReadyToShoot = false;

        //    StartCoroutine(ShootCoroutine());
        //}

        //private IEnumerator ShootCoroutine()
        //{
        //    Quaternion targetRotation = Quaternion.LookRotation(_shootingDirection, Vector3.up);
        //    yield return new WaitUntil(() => m_moveController.root.rotation.eulerAngles.y >= targetRotation.eulerAngles.y - _rotationOffset && m_moveController.root.rotation.eulerAngles.y <= targetRotation.eulerAngles.y + _rotationOffset);

        //    _animator.SetBool("isShoot", true);
        //    //m_bulletShooterFX.Play();
        //    m_muzzleFX.Play();
        //    _weapon._bulletsLeft--;

        //    Invoke("ResetShoot", _weapon.timeBetweenShoot);
        //}

        //public void ResetShoot()
        //{
        //    _animator.SetBool("isShoot", false);
        //    _isShooting = false;
        //    _isReadyToShoot = true;
        //}

        //private void OnDragAttackInput(Vector2 direction)
        //{
        //    if (!_isShooting)
        //    {
        //        _shootingDirection = new Vector3(direction.x, 0, direction.y);
        //    }
        //}

        //private void OnPointerUpAttackInput()
        //{
        //    if (_weapon._bulletsLeft >= 1f && _isReadyToShoot)
        //    {
        //        Shoot();
        //    }
        //}

        
    }
}