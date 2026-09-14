using AdventureGame.Core.Constants;
using AdventureGame.Environment.Projectiles;
using UnityEngine;

namespace AdventureGame.Entities.Player.States
{
    public class PlayerShootingState : PlayerState
    {
        private float m_Timer;

        public override void Enter(Player owner)
        {
            m_Timer = owner.Data.ShootDuration;
            owner.CurrentVelocity = Vector2.zero;

            owner.Animator.SetTrigger(AnimatorHashes.Launch);
            owner.OneShotSource.PlayOneShot(owner.LaunchClip);

            GameObject projectileObj = Object.Instantiate(
                owner.ProjectilePrefab,
                owner.Rigidbody.position + Vector2.up * 0.5f,
                Quaternion.identity
            );

            if (projectileObj.TryGetComponent(out Projectile projectile))
                projectile.Launch(owner.MoveDirection, owner.Data.ProjectileLaunchForce);
        }

        public override void Update(Player owner)
        {
            m_Timer -= Time.deltaTime;

            if (owner.DashAction.WasPressedThisFrame() && owner.DashCooldownTimer <= 0f)
            {
                owner.ChangeState(owner.DashingState);
            
                return;
            }

            if (m_Timer <= 0f)
                owner.ChangeState(owner.GroundedState);
        }
    }
}
