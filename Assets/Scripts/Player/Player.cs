using Fusion;
using UnityEngine;

namespace BrawlShooter
{
    public class Player : NetworkBehaviour
    {
        public string username;
        public PlayerRef Ref => Object.InputAuthority;

        [Networked]
        public NetworkBool isReady { get; set; }

        [Networked]
        public NetworkString<_64> selectedCharacterId { get; set; }
        
        public PlayerAgent Agent { get; private set; }
        public CharacterData CharacterData => Agent.CharacterData;

        public override void Spawned()
        {
            isReady = false;

            NetworkManager.Instance.AddPlayer(this);
            EventManager.TriggerEvent(new PlayerSpawnedEvent { player = this });
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            DespawnAgent();
            NetworkManager.Instance.RemovePlayer(this);
            EventManager.TriggerEvent(new PlayerDespawnedEvent { player = this });
        }

        public void SpawnAgent(Transform spawnTransform)
        {
            SpawnAgent(spawnTransform.position, spawnTransform.rotation);
        }

        public void SpawnAgent(Vector3 position, Quaternion rotation)
        {
            var agentPrefab = NetworkManager.Instance.isUsingMultipeer 
                ? NetworkManager.Instance.testingAgentPrefab 
                : GlobalSettings.Instance.database.GetCharacterData(selectedCharacterId).agentPrefab;

            Agent = Runner.Spawn(agentPrefab, position, rotation, Ref);
            Agent.SetOwner(this);
        }

        public void DespawnAgent()
        {
            if (Agent == null) return;

            Runner.Despawn(Agent.Object);
        }

        [Rpc]
        public void RPC_ToggleReady()
        {
            if (HasStateAuthority)
            {
                isReady = !isReady;
            }
            
            EventManager.TriggerEvent(new PlayerReadyEvent { player = this });
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void RPC_SetSelectedCharacterId(string id)
        {
            selectedCharacterId = id;
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
}