using Fusion;
using Lean.Pool;
using System.Collections.Generic;
using UnityEngine;

namespace BrawlShooter
{
    public class NetworkObjectPool : MonoBehaviour, INetworkObjectPool
    {
        private List<NetworkObject> _pools = new List<NetworkObject>();

        public NetworkObject AcquireInstance(NetworkRunner runner, NetworkPrefabInfo info)
        {
            NetworkObject prefab;
            if (NetworkProjectConfig.Global.PrefabTable.TryGetPrefab(info.Prefab, out prefab))
            {
                var networkObject = LeanPool.Spawn(prefab, Vector3.zero, Quaternion.identity);
                _pools.Add(networkObject);
                return networkObject;
            }

            Debug.LogError("No prefab for " + info.Prefab);
            return null;
        }

        public void ReleaseInstance(NetworkRunner runner, NetworkObject networkObject, bool isSceneObject)
        {
            Debug.Log($"Releasing {networkObject} instance, isSceneObject={isSceneObject}");

            if (networkObject != null)
            {
                _pools.Remove(networkObject);
                LeanPool.Despawn(networkObject);
            }
        }

        public void ClearPools()
        {
            foreach (NetworkObject networkObject in _pools)
            {
                LeanPool.Detach(networkObject);
                Destroy(networkObject);
            }

            _pools.Clear();
        }
    }
}