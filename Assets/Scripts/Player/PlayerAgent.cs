using Fusion;
using UnityEngine;

namespace BrawlShooter
{
    [RequireComponent(typeof(NetworkObject), typeof(NetworkMecanimAnimator), typeof(NetworkCharacterControllerPrototype))]
    public class PlayerAgent : NetworkBehaviour, IDamageable
    {
        public Player Owner { get; private set; }
        public CharacterData CharacterData => Owner.CharacterData;

        [Networked]
        public byte health { get; set; } = 100;
        public byte maxHealth = 100;

        [Networked]
        private TickTimer _invulnerabilityTimer { get; set; }
        public float invulnerabilityTime = 0.1f;

        public NetworkMecanimAnimator NetworkAnimator { get; private set; }
        public NetworkCharacterControllerPrototype NetworkCharacterController { get; private set; }
        public NetworkInputController NetworkInput { get; private set; }

        private PlayerAbility[] _abilities;

        [SerializeField]
        private ProgressBar _healthBar;

        private void Awake()
        {
            NetworkAnimator = GetComponent<NetworkMecanimAnimator>();
            NetworkCharacterController = GetComponent<NetworkCharacterControllerPrototype>();
            NetworkInput = GetComponent<NetworkInputController>();

            _abilities = GetComponentsInChildren<PlayerAbility>();
        }

        private void OnEnable()
        {
            NetworkInput.OnFetchInput.AddListener(ProcessInput);
        }

        private void OnDisable()
        {
            NetworkInput.OnFetchInput.RemoveListener(ProcessInput);
        }

        public override void Spawned()
        {
            foreach (var ability in _abilities)
            {
                ability.Initialize(this);
            }

            ResetStats();
        }

        public override void FixedUpdateNetwork()
        {
            UpdateHealthBar();
        }

        public void ProcessInput(InputContext context)
        {
            foreach(PlayerAbility ability in _abilities)
            {
                ability.OnProcessInput(context);
            }
        }

        public void UpdateHealthBar()
        {
            if (_healthBar == null) return;

            _healthBar.fillAmount = health.ToFloat() / maxHealth.ToFloat();
        }

        public void ApplyDamage(byte damage, PlayerRef source)
        {
            if (!_invulnerabilityTimer.ExpiredOrNotRunning(Runner))
                return;

            if (damage >= health)
            {
                health = 0;
            }
            else
            {
                health -= damage;
                _invulnerabilityTimer = TickTimer.CreateFromSeconds(Runner, invulnerabilityTime);
            }
        }

        public void ResetStats()
        {
            health = maxHealth;
        }

        public void SetOwner(Player player) => Owner = player;
    }
}