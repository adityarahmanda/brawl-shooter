using Fusion;
using UnityEngine;

namespace BrawlShooter
{
    public class Player : NetworkContextBehaviour
    {
        public int id = -1;
        public PlayerRef Ref => Object.InputAuthority;

        [Networked]
        public NetworkBool isReady { get; set; }

        [Networked]
        public int selectedCharacterIndex { get; set; } = -1;
        public bool HasSelectedCharacter => selectedCharacterIndex > -1;

        public CharacterData CharacterData;

        public PlayerAgent Agent { get; private set; }

        public override void Spawned()
        {
            if (Object.HasInputAuthority)
            {
                NetworkLauncher.Instance.Local = this;
            }

            id = Object.InputAuthority;
            isReady = false;

            EventManager.TriggerEvent(new PlayerSpawnedEvent { player = this });
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            DespawnAgent();
            EventManager.TriggerEvent(new PlayerDespawnedEvent { player = this });
        }

        public PlayerAgent SpawnAgent(Vector3 position, Quaternion rotation)
        {
            if (CharacterData == null) return null;

            Agent = Runner.Spawn(CharacterData.agentPrefab, position, rotation, Ref);
            Agent.SetOwner(this);

            return Agent;
        }

        public void DespawnAgent()
        {
            if (Agent == null) return;

            Runner.Despawn(Agent.Object);
        }

        [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
        public void RPC_ToggleReady()
        {
            isReady = !isReady;
            EventManager.TriggerEvent(new PlayerReadyEvent { player = this });
        }

        [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
        public void RPC_SetCharacterData(int selectedCharacterIndex)
        {
            this.selectedCharacterIndex = selectedCharacterIndex;
            EventManager.TriggerEvent(new PlayerSelectCharacterEvent { player = this });
        }
    }

    public struct PlayerSpawnedEvent
    {
        public Player player;
    }

    public struct PlayerDespawnedEvent
    {
        public Player player;
    }

    public struct PlayerReadyEvent
    {
        public Player player;
    }

    public struct PlayerSelectCharacterEvent
    {
        public Player player;
    }
}