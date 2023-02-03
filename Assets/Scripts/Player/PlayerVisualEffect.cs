using Fusion;
using UnityEngine;
using Lean.Pool;

namespace BrawlShooter
{
    public class PlayerVisualEffect : PlayerAbility
    {
        [SerializeField]
        private ParticleSystem _teleportFXPrefab;

        public override void Spawned()
        {
            if(_teleportFXPrefab == null) return;

            var effect = LeanPool.Spawn(_teleportFXPrefab, transform.position, Quaternion.identity);
            Runner.MoveToRunnerScene(effect);

            if (Object.HasInputAuthority)
            {
                // AudioManager.Instance.PlaySound2D("teleportIn");
            }
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            if (_teleportFXPrefab == null) return;

            var effect = LeanPool.Spawn(_teleportFXPrefab, transform.position, Quaternion.identity);
            Runner.MoveToRunnerScene(effect);

            if (Object.HasInputAuthority)
            {
                // AudioManager.Instance.PlaySound2D("teleportOut");
            }
        }
    }
}
