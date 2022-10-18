using Fusion;
using UnityEngine;

namespace TowerBomber
{
    public interface IDamageable
    {
        void ApplyDamage(Vector3 impulse, byte damage, PlayerRef source);
    }
}
