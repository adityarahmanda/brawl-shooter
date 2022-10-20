using Fusion;
using UnityEngine;

namespace TowerBomber
{
    public class Player : NetworkBehaviour, IDamageable
    {
        public const byte MAX_HEALTH = 100;

        public int playerID { get; private set; }

        [Networked(OnChanged = nameof(OnReadyChanged))]
        private NetworkBool _ready { get; set; }
        public bool ready => _ready;

        [Networked]
        private byte _health { get; set; }
        public byte health => _health;

        [Networked]
        private TickTimer _invulnerabilityTimer { get; set; }

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

        [Networked]
        private float _bulletsLeft { get; set; }
        public float bulletsLeft => _bulletsLeft;

        [Networked]
        private Vector2 _moveDirection { get; set; }
        public Vector2 moveDirection => _moveDirection;

        [Networked]
        private Vector2 _aimDirection { get; set; }
        public Vector2 aimDirection => _aimDirection;

        private Weapon _weapon;
        public Weapon weapon => _weapon;

        private Animator _animator;
        private CharacterController _characterController;
        private NetworkCharacterControllerPrototype _networkCharacterController;

        private float _rotationOffset = .5f;
        private bool _isReadyToShoot, _isShooting;

        private Vector3 _shootingDirection;

        public bool IsActivated => (gameObject.activeInHierarchy && state == State.Active);

        public static Player local { get; set; }

        public override void Spawned()
        {
            if (Object.HasInputAuthority)
                local = this;

            _characterController = GetComponent<CharacterController>();
            _networkCharacterController = GetComponent<NetworkCharacterControllerPrototype>();

            // Getting this here because it will revert to -1 if the player disconnects, but we still want to remember the Id we were assigned for clean-up purposes
            playerID = Object.InputAuthority;
            _ready = false;

            PlayerManager.AddPlayer(this);
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            InputController.fetchInput = false;
            PlayerManager.RemovePlayer(this);
        }

        public void SpawnCharacter(Character characterPrefab)
        {
            Character character = Instantiate(characterPrefab, transform);
            _animator = character.GetComponentInChildren<Animator>();
            
            CapsuleCollider collider = character.GetComponentInChildren<CapsuleCollider>();
            _characterController.radius = collider.radius;
            _characterController.height = collider.height;
            _characterController.center = collider.center;

            _weapon = character.weapon;
            _bulletsLeft = _weapon.magazineSize;
            _networkCharacterController.InterpolationTarget = character.interpolationRoot;

            if (Object.HasInputAuthority)
            {
                // put code here
            }
        }

        public async void TriggerDespawn()
        {
            PlayerManager.RemovePlayer(this);

            if (Object == null) { return; }

            if (Object.HasStateAuthority)
            {
                Runner.Despawn(Object);
            }
        }

        public void ToggleReady()
        {
            _ready = !_ready;
        }

        public void ResetReady()
        {
            _ready = false;
        }

        public void ToggleWeaponType()
        {
            _weaponType = weaponType == Weapon.Type.SingleShot ? Weapon.Type.RapidShot : Weapon.Type.SingleShot;
        }

        public void ResetStats()
        {
            // Debug.Log($"Resetting player {playerID}, tick={Runner.Simulation.Tick}, timer={respawnTimer.IsRunning}:{respawnTimer.TargetTick}, life={life}, lives={lives}, hasAuthority={Object.HasStateAuthority} to state={state}");
            _health = MAX_HEALTH;
        }

        /// <summary>
		/// Set the direction of movement and aim
		/// </summary>
		public void SetDirections(Vector2 moveDirection)
        {
            _moveDirection = moveDirection;
            // _aimDirection = aimDirection;
        }

        public void Move()
        {
            if (!IsActivated)
                return;

            _animator.SetBool("isMoving", _moveDirection.magnitude > 0);
            _networkCharacterController.Move(new Vector3(_moveDirection.x, 0, _moveDirection.y));
        }

        public void ApplyDamage(byte damage, PlayerRef source)
        {
            if (!IsActivated || !_invulnerabilityTimer.Expired(Runner))
                return;

            if (damage >= _health)
            {
                _health = 0;
                state = State.Dead;

                GameManager.instance.OnPlayerDead();
            }
            else
            {
                _health -= damage;
                _invulnerabilityTimer = TickTimer.CreateFromSeconds(Runner, 0.1f);
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
            switch (state)
            {
                case State.Dead:
                    _animator.SetTrigger("die");
                    break;
            }
        }

        public static void OnReadyChanged(Changed<Player> changed)
        {
            if (changed.Behaviour)
                changed.Behaviour.OnReadyChanged();
        }

        public void OnReadyChanged()
        {
            LobbyManager.instance.UpdatePlayerLobbyStatus(this);
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
    }
}