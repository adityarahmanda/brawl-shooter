using Fusion;
using UnityEngine;

namespace BrawlShooter
{
    public class Player : NetworkBehaviour, IDamageable
    {
        public const byte MAX_HEALTH = 100;

        public int playerID { get; private set; }

        [Networked(OnChanged = nameof(OnReadyChanged))]
        private NetworkBool _ready { get; set; }
        public bool IsReady => GameManager.playState == GameManager.PlayState.LOBBY && _ready;

        [Networked(OnChanged = nameof(OnHealthChanged))]
        private byte _health { get; set; }
        public byte health => _health;

        [Networked]
        private TickTimer _invulnerabilityTimer { get; set; }
        private static float INVULNERABILITY_TIME = 0.1f;

        public enum State
        {
            InLobby,
            Active,
            Dead,
            Despawned
        }

        [Networked(OnChanged = nameof(OnStateChanged))]
        public State state { get; set; }

        [Networked(OnChanged = nameof(OnWeaponTypeChanged))]
        private Weapon.Type _weaponType { get; set; }
        public Weapon.Type weaponType => _weaponType;
        
        private Weapon[] _weapons;
        private Weapon _currWeapon;

        private int _lastBulletsLeft;

        private Character _character;

        [Networked]
        private Vector2 _moveDirection { get; set; }
        public Vector2 moveDirection => _moveDirection;

        [Networked]
        private Vector2 _aimDirection { get; set; }
        public Vector2 aimDirection => _aimDirection;

        private Vector2 _currAimDirection { get; set; }

        private Animator _animator;
        private NetworkCharacterControllerPrototype _networkCharacterController;

        private ProgressBar _healthbar;
        [SerializeField] 
        private Vector3 _healthbarOffset;

        private BulletBar _bulletbar;
        [SerializeField] 
        private Vector3 _bulletbarOffset;

        private WeaponIndicator _weaponIndicator;
        [SerializeField] 
        private Vector3 _weaponIndicatorOffset;

        private float _initRotSpeed;

        private bool _isCalculateAimDirection;        
        public bool IsActivated => gameObject.activeInHierarchy && state == State.Active;
        public bool IsCalculateAimDirection => IsActivated && _isCalculateAimDirection;
        public bool IsReadyToShoot => IsActivated && _currWeapon.bulletsLeft >= 1 && !_isShooting;
        
        [Networked(OnChanged = nameof(OnIsShootingChanged))]
        private NetworkBool _isShooting { get; set; }

        [SerializeField]
        private ParticleSystem _teleportFX;

        public static Player local { get; set; }

        public override void Spawned()
        {
            if (Object.HasInputAuthority)
            {
                local = this;
                InputController.fetchInput = true;
            }

            _networkCharacterController = GetComponent<NetworkCharacterControllerPrototype>();

            _weapons = GetComponentsInChildren<Weapon>(true);
            _animator = GetComponentInChildren<Animator>(true);

            _character = GetComponentInChildren<Character>(true);
            _character.gameObject.SetActive(false);

            // Getting this here because it will revert to -1 if the player disconnects, but we still want to remember the Id we were assigned for clean-up purposes
            playerID = Object.InputAuthority;
            _ready = false;

            PlayerManager.AddPlayer(this);
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            InputController.fetchInput = false;

            LobbyManager.instance.RemovePlayer(this);
            DespawnCharacterHUD();

            PlayerManager.RemovePlayer(this);

            if (GameManager.playState == GameManager.PlayState.LEVEL)
            {
                GameManager.instance.OnPlayerDeadOrDisconnected();
            }
        }

        public override void Render()
        {
            if (!IsActivated)
                return;

            LocalPlayerRender();
        }

        private void LocalPlayerRender()
        {
            if (!Object.HasInputAuthority)
                return;

            if(_bulletbar != null)
                _bulletbar.SetProgress(_currWeapon.bulletsLeft, _currWeapon.magazineSize);

            if (IsCalculateAimDirection && _weaponIndicator != null)
            {
                _weaponIndicator.SetDirection(_aimDirection);
                _bulletbar.SetProgress(_currWeapon.bulletsLeft, _currWeapon.magazineSize);
            }

        }

        public override void FixedUpdateNetwork()
        {
            Reload();
        }

        public void SpawnCharacter(ProgressBar _healthbarPrefab, BulletBar _bulletbarPrefab, WeaponIndicator _weaponIndicatorPrefab)
        {
            _teleportFX.Play();
            if (Object.HasInputAuthority)
                AudioManager.instance.PlaySound2D("teleportIn");

            _character.gameObject.SetActive(true);
            _character.SetPlayer(this);

            for(int i = 0; i < _weapons.Length; i++)
            {
                if(weaponType == _weapons[i].type)
                {
                    _weapons[i].gameObject.SetActive(true);
                    _currWeapon = _weapons[i];
                    _currWeapon.bulletsLeft = _currWeapon.magazineSize;
                }
                else 
                {
                    _weapons[i].gameObject.SetActive(false);
                }
            }

            _animator.runtimeAnimatorController = _currWeapon.charAnimControl;

            SpawnCharacterHUD(_healthbarPrefab, _bulletbarPrefab, _weaponIndicatorPrefab);
        }

        public void SpawnCharacterHUD(ProgressBar _healthbarPrefab, BulletBar _bulletbarPrefab, WeaponIndicator _weaponIndicatorPrefab)
        {
            _healthbar = Instantiate(_healthbarPrefab, transform.position + _healthbarOffset, Quaternion.identity);
            _healthbar.SetTarget(transform, _healthbarOffset);

            if (Object.HasInputAuthority)
            {
                _bulletbar = Instantiate(_bulletbarPrefab, transform.position + _bulletbarOffset, Quaternion.identity);
                _bulletbar.SetTarget(transform, _bulletbarOffset);
                _bulletbar.SetMaxBullets(_currWeapon.magazineSize);

                _weaponIndicator = Instantiate(_weaponIndicatorPrefab, transform.position + _weaponIndicatorOffset, _weaponIndicatorPrefab.transform.rotation);
                _weaponIndicator.SetTarget(transform, _weaponIndicatorOffset);
                _weaponIndicator.SetRange(_currWeapon.range);
            }
        }

        public void DespawnCharacter()
        {
            _teleportFX.Play();
            if (Object.HasInputAuthority)
                AudioManager.instance.PlaySound2D("teleportOut");

            _character.gameObject.SetActive(false);
            _character.SetPlayer(null);

            for (int i = 0; i < _weapons.Length; i++)
            {
                _weapons[i].gameObject.SetActive(false);
            }

            DespawnCharacterHUD();
        }

        public void DespawnCharacterHUD()
        {
            if (_healthbar != null)
                Destroy(_healthbar.gameObject);

            if (_bulletbar != null)
                Destroy(_bulletbar.gameObject);

            if (_weaponIndicator != null)
                Destroy(_weaponIndicator.gameObject);
        }

        public void TriggerDespawn()
        {
            PlayerManager.RemovePlayer(this);

            if (Object == null) { return; }

            if (Object.HasStateAuthority)
                Runner.Despawn(Object);
        }

        public void ToggleReady()
        {
            _ready = !_ready;
        }

        public void ToggleWeaponType()
        {
            _weaponType = weaponType == Weapon.Type.SingleShot ? Weapon.Type.RapidShot : Weapon.Type.SingleShot;
        }

        public void ResetStats()
        {
            Debug.Log($"Resetting player {playerID}, tick={Runner.Simulation.Tick}, health={_health}, hasAuthority={Object.HasStateAuthority} to state={state}");
            _health = MAX_HEALTH;
            _ready = false;
        }

		public void SetMoveDirection(Vector2 moveDirection)
        {
            _moveDirection = moveDirection;
        }

        public void Move()
        {
            if (!IsActivated)
                return;

            _animator.SetBool("isMoving", _moveDirection.magnitude > 0);
            _networkCharacterController.Move(new Vector3(_moveDirection.x, 0, _moveDirection.y));
        }

        public void CalculateAimDirection()
        {
            if (!IsActivated)
                return;

            if(Object.HasInputAuthority)
                _weaponIndicator.Activate(true);
            
            _isCalculateAimDirection = true;
        }

        public void SetAimDirection(Vector2 aimDirection)
        {
            _aimDirection = aimDirection;
        }

        public void StartShoot()
        {
            if (!IsActivated)
                return;

            if (Object.HasInputAuthority)
                _weaponIndicator.Activate(false);

            if (!IsReadyToShoot)
                return;

            _currAimDirection = _aimDirection;
            _isCalculateAimDirection = false;

            _initRotSpeed = _networkCharacterController.rotationSpeed;
            _networkCharacterController.rotationSpeed = 0;

            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(_currAimDirection.x, 0, _currAimDirection.y), Vector3.up);
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, targetRotation.eulerAngles.y, transform.rotation.eulerAngles.z);

            if (_currWeapon.type == Weapon.Type.RapidShot)
            {
                _currWeapon.rapidBulletsLeft = _currWeapon.rapidBulletsCount;
            }

            _currWeapon.bulletsLeft--;
            _isShooting = true;
        }

        public void Shoot()
        {
            _currWeapon.Shoot(_currAimDirection);

            if(Object.HasInputAuthority)
                AudioManager.instance.PlaySound2D("gunshot");
        }

        public void CheckEndShoot()
        {
            if(_currWeapon.type == Weapon.Type.SingleShot)
            {
                EndShoot();
            } 
            else if(_currWeapon.type == Weapon.Type.RapidShot)
            {
                if (_currWeapon.rapidBulletsLeft <= 0)
                {
                    EndShoot();
                }
            }
        }

        public void EndShoot()
        {
            if (!IsActivated)
                return;

            _networkCharacterController.rotationSpeed = _initRotSpeed;
            _isShooting = false;
        }

        private void Reload()
        {
            if (!IsActivated || _currWeapon == null)
                return;

            if (!_isShooting && _currWeapon.bulletsLeft < _currWeapon.magazineSize)
            {
                _lastBulletsLeft = (int)_currWeapon.bulletsLeft;
                _currWeapon.bulletsLeft += (1 / _currWeapon.bulletCooldownTime) * Runner.DeltaTime;

                if ((int)_currWeapon.bulletsLeft > _lastBulletsLeft)
                {
                    if (Object.HasInputAuthority)
                        AudioManager.instance.PlaySound2D("reload");
                }

                if (_currWeapon.bulletsLeft > _currWeapon.magazineSize)
                {
                    _currWeapon.bulletsLeft = _currWeapon.magazineSize;
                }
            }
        }

        public void ApplyDamage(byte damage, PlayerRef source)
        {
            if (!IsActivated || !_invulnerabilityTimer.ExpiredOrNotRunning(Runner))
                return;

            if (damage >= _health)
            {
                _health = 0;
                state = State.Dead;

                GameManager.instance.OnPlayerDeadOrDisconnected();
            }
            else
            {
                _health -= damage;
                _invulnerabilityTimer = TickTimer.CreateFromSeconds(Runner, INVULNERABILITY_TIME);
                Debug.Log($"Player {playerID} took {damage} damage, health = {_health}");
            }
        }

        public static void OnStateChanged(Changed<Player> changed)
        {
            if (changed.Behaviour)
                changed.Behaviour.OnStateChanged();
        }

        public void OnStateChanged()
        {
            if(state == State.Dead)
                _animator.SetTrigger("die");
        }

        public static void OnHealthChanged(Changed<Player> changed)
        {
            if (changed.Behaviour)
                changed.Behaviour.OnHealthChanged();
        }

        public void OnHealthChanged()
        {
            if (_healthbar == null)
                return;

            _healthbar.SetProgress(_health, MAX_HEALTH);
        }

        public static void OnReadyChanged(Changed<Player> changed)
        {
            if (changed.Behaviour)
                changed.Behaviour.OnReadyChanged();
        }

        public void OnReadyChanged()
        {
            LobbyManager.instance.UpdatePlayerLobbyStatus(this);
            
            if(PlayerManager.GetAllPlayersReady())
            {
                GameManager.instance.OnAllPlayersReady();
            }
        }

        public static void OnWeaponTypeChanged(Changed<Player> changed)
        {
            if (changed.Behaviour)
                changed.Behaviour.OnWeaponTypeChanged();
        }

        public void OnWeaponTypeChanged()
        {
            LobbyManager.instance.UpdatePlayerWeapon(this);
        }

        public static void OnIsShootingChanged(Changed<Player> changed)
        {
            if (changed.Behaviour)
                changed.Behaviour.OnIsShootingChanged();
        }

        public void OnIsShootingChanged()
        {
            _animator.SetFloat("shootSpeed", _currWeapon.shootSpeed);
            _animator.SetBool("isShooting", _isShooting);
        }
    }
}