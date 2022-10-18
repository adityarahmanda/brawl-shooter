using System.Threading.Tasks;
using Fusion;
using UnityEngine;

namespace TowerBomber
{
    public class Player : NetworkBehaviour, IDamageable
    {
        public int playerID { get; private set; }

        [Networked] 
        public NetworkString<_128> playerName { get; set; }

        [Networked]
        public NetworkBool ready { get; set; }

        [Networked] 
        public byte health { get; set; }

        public const byte MAX_HEALTH = 100;

        public enum State
        {
            Joined,
            Despawned,
            Spawning,
            Active,
            Dead
        }

        [Networked(OnChanged = nameof(OnStateChanged))]
        public State state { get; set; }

        public static Player local { get; set; }

        public override void Spawned()
        {
            if (Object.HasInputAuthority)
                local = this;

            // Getting this here because it will revert to -1 if the player disconnects, but we still want to remember the Id we were assigned for clean-up purposes
            playerID = Object.InputAuthority;
            ready = false;

            PlayerManager.AddPlayer(this);
        }

        public void InitNetworkState()
        {
            state = State.Joined;
            health = MAX_HEALTH;
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
                case State.Joined:
                    // LobbyManager
                    break;
                case State.Spawning:
                    //_teleportIn.StartTeleport();
                    break;
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
            ready = !ready;
        }

        public void ApplyDamage(Vector3 impulse, byte damage, PlayerRef source)
        {
            
        }
    }
}