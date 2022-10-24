using Fusion;

namespace BrawlShooter
{
    public interface IDamageable
    {
        void ApplyDamage(byte damage, PlayerRef source);
    }
}
