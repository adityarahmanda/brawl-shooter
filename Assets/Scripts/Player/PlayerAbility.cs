using BrawlShooter;
using Fusion;

namespace BrawlShooter
{
    public abstract class PlayerAbility : NetworkBehaviour
    {
        public Player Owner => Agent.Owner;
        public PlayerAgent Agent { get; protected set; }

        public void Initialize(PlayerAgent agent)
        {
            Agent = agent;
        }

        public virtual void OnProcessInput(InputContext context) { }
    }
}
