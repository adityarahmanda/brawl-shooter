using System.Threading.Tasks;
using Fusion;
using UnityEngine;
using PlayState = TowerBomber.GameManager.PlayState;

namespace TowerBomber
{
    public class Player : NetworkBehaviour, IDamageable
    {
        public int playerID { get; private set; }

        [Networked] 
        public NetworkString<_128> playerName { get; set; }

        [Networked(OnChanged = nameof(OnReadyChanged))]
        public NetworkBool ready { get; set; }

        [Networked] 
        public byte health { get; set; }

        public const byte MAX_HEALTH = 100;

        public enum State
        {
            InLobby,
            Despawned,
            Active,
            Dead
        }

        [Networked(OnChanged = nameof(OnStateChanged)), SerializeField]
        public State state { get; set; }

        public bool isActivated => (gameObject.activeInHierarchy && state == State.Active);

        public static Player local { get; set; }

        public override void Spawned()
        {
            if (Object.HasInputAuthority)
            {
                local = this;
                InputController.fetchInput = true;
            }

            // Getting this here because it will revert to -1 if the player disconnects, but we still want to remember the Id we were assigned for clean-up purposes
            playerID = Object.InputAuthority;
            ready = false;

            PlayerManager.AddPlayer(this);
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            PlayerManager.RemovePlayer(this);
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

        public void SetPlayerState(State state)
        {
            state = state;
        }

        public void DespawnCharacter()
        {
            if (state == State.Dead)
                return;

            state = State.Despawned;
        }

        public void ToggleReady()
        {
            ready = !ready;
        }

        public void ResetReady()
        {
            ready = false;
        }

        public void ResetStats()
        {
            // Debug.Log($"Resetting player {playerID}, tick={Runner.Simulation.Tick}, timer={respawnTimer.IsRunning}:{respawnTimer.TargetTick}, life={life}, lives={lives}, hasAuthority={Object.HasStateAuthority} to state={state}");
            health = MAX_HEALTH;
        }

        public void ApplyDamage(Vector3 impulse, byte damage, PlayerRef source)
        {
            if (!isActivated)
                return;

            if (Object.HasStateAuthority)
            {
                //_cc.Velocity += impulse / 10.0f; // Magic constant to compensate for not properly dealing with masses
                //_cc.Move(Vector3.zero); // Velocity property is only used by CC when steering, so pretend we are, without actually steering anywhere
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
                case State.Active:
                    //_damageVisuals.CleanUpDebris();
                    //_teleportIn.EndTeleport();
                    break;
                case State.Dead:
                    //_deathExplosionInstance.transform.position = transform.position;
                    //_deathExplosionInstance.SetActive(false); // dirty fix to reactivate the death explosion if the particlesystem is still active
                    //_deathExplosionInstance.SetActive(true);

                    //_visualParent.gameObject.SetActive(false);
                    //_damageVisuals.OnDeath();
                    break;
                case State.Despawned:
                    //_teleportOut.StartTeleport();
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
    }
}