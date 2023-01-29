using Fusion;
using UnityEngine;

namespace BrawlShooter
{
    [RequireComponent(typeof(NetworkObject), typeof(NetworkMecanimAnimator), typeof(NetworkCharacterControllerPrototype))]
    public class PlayerAgent : NetworkBehaviour, IDamageable
    {
        public Player Owner { get; private set; }
        public CharacterData CharacterData => Owner.CharacterData;

        [Networked(OnChanged = nameof(OnHealthChanged))]
        public byte health { get; set; } = 100;

        [Networked]
        private TickTimer _invulnerabilityTimer { get; set; }

        public NetworkMecanimAnimator NetworkAnimator { get; private set; }
        public NetworkCharacterControllerPrototype NetworkCharacterController { get; private set; }
        public NetworkInputController NetworkInput { get; private set; }

        private PlayerAbility[] _abilities;
        
        private ProgressBar _healthbar;
        
        public const byte MAX_HEALTH = 100;
        private const float INVULNERABILITY_TIME = 0.1f;

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
        }

        public void TriggerDespawn()
        {
            if (Object == null) { return; }

            if (Object.HasStateAuthority)
                Runner.Despawn(Object);
        }

        public void ResetStats()
        {
            health = MAX_HEALTH;
        }

        public void ProcessInput(InputContext context)
        {
            foreach(PlayerAbility ability in _abilities)
            {
                ability.OnProcessInput(context);
            }
        }

        public void ApplyDamage(byte damage, PlayerRef source)
        {
            if (!_invulnerabilityTimer.ExpiredOrNotRunning(Runner))
                return;

            if (damage >= health)
            {
                health = 0;
                // game event
            }
            else
            {
                health -= damage;
                _invulnerabilityTimer = TickTimer.CreateFromSeconds(Runner, INVULNERABILITY_TIME);
            }
        }

        public static void OnHealthChanged(Changed<PlayerAgent> changed)
        {
            if (changed.Behaviour)
                changed.Behaviour.OnHealthChanged();
        }

        public void OnHealthChanged()
        {
            if (_healthbar == null)
                return;

            _healthbar.SetProgress(health, MAX_HEALTH);
        }

        public void SetOwner(Player player) => Owner = player;
    }
}