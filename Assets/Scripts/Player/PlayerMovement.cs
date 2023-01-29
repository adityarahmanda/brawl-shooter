using Fusion;
using UnityEngine;

namespace BrawlShooter
{
    public class PlayerMovement : PlayerAbility
    {
        [Networked]
        public Vector2 moveDirection { get; private set; }
        
        public override void FixedUpdateNetwork()
        {
            Move();
            HandleAnimation();
        }

        public override void OnProcessInput(InputContext context)
        {
            moveDirection = context.data.moveDirection;
        }

        public void HandleAnimation()
        {
            if (IsProxy || !Runner.IsForward) return;

            Character.NetworkAnimator.Animator.SetBool("isMoving", moveDirection.magnitude > 0);
        }

        public void Move()
        {
            Character.NetworkCharacterController.Move(new Vector3(moveDirection.x, 0, moveDirection.y));
        }
    }
}