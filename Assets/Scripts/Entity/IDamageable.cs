using Fusion;
using UnityEngine;

namespace TowerBomber
{
    public interface IDamageable
    {
        void ApplyDamage(byte damage, PlayerRef source);
    }
}
