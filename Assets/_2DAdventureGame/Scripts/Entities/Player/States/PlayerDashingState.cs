using AdventureGame.Core.Constants;
using UnityEngine;

namespace AdventureGame.Entities.Player.States
{
    public class PlayerDashingState : PlayerState
    {
        private float m_DashTimer;
        private float m_AfterimageTimer;
        private Vector2 m_DashDirection;

        public override void Enter(Player owner)
        {
            m_DashTimer = owner.Data.DashDuration;
            m_DashDirection = owner.MoveDirection;

            owner.DashCooldownTimer = owner.Data.DashCooldown;
            owner.IsInvincible = true;
            owner.DamageCooldown = owner.Data.DashDuration;
            owner.Animator.SetFloat(AnimatorHashes.Speed, 1f);
            owner.OneShotSource.PlayOneShot(owner.DashClip);
            owner.CurrentVelocity = Vector2.zero;
        }

        public override void Update(Player owner)
        {
            if (m_DashTimer <= 0)
            {
                owner.ChangeState(owner.GroundedState);
            
                return;
            }

            if (m_AfterimageTimer <= 0)
                SpawnAfterimages(owner);

            m_DashTimer -= Time.deltaTime;
            m_AfterimageTimer -= Time.deltaTime;
        }

        public override void FixedUpdate(Player owner)
        {
            Vector2 offset = m_DashDirection * owner.Data.DashSpeed * Time.fixedDeltaTime;

            owner.Rigidbody.MovePosition(owner.Rigidbody.position + offset);
        }

        private void SpawnAfterimages(Player owner)
        {
            GameObject ghost = Object.Instantiate(
                owner.AfterimagePrefab,
                owner.transform.position,
                owner.transform.rotation
            );

            if (ghost.TryGetComponent(out DashAfterimage afterimage))
            {
                afterimage.Initialize(
                    owner.SpriteRenderer.sprite,
                    owner.transform.localScale,
                    owner.SpriteRenderer.flipX,
                    owner.Data.AfterimageColor,
                    owner.Data.AfterimageLingerDuration
                );
            }

            m_AfterimageTimer = owner.Data.AfterimageInterval;
        }
    }
}
