using BrawlShooter;
using Fusion;

namespace BrawlShooter
{
    public abstract class PlayerAbility : NetworkContextBehaviour
    {
        public Player Owner => Character.Owner;
        public PlayerAgent Character { get; protected set; }

        public void Initialize(PlayerAgent character)
        {
            Character = character;
        }

        public virtual void OnProcessInput(InputContext context) { }
    }
}
