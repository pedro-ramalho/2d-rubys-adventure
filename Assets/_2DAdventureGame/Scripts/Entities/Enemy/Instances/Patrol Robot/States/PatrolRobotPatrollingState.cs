using AdventureGame.Core.Constants;
using AdventureGame.Core.Quests;
using UnityEngine;

namespace AdventureGame.Entities.Enemy.Instances.Patrol_Robot.States
{
    public class PatrolRobotPatrollingState : PatrolRobotState
    {
        private float m_DirectionTimer;

        public override void Enter(PatrolRobot owner)
        {
            m_DirectionTimer = owner.PatrolDuration;
            owner.Direction = 1;
        }

        public override void Update(PatrolRobot owner)
        {
            m_DirectionTimer -= Time.deltaTime;
            if (m_DirectionTimer <= 0f)
            {
                owner.Direction = -owner.Direction;
                m_DirectionTimer = owner.PatrolDuration;
            }
        }

        public override void FixedUpdate(PatrolRobot owner)
        {
            Vector2 position = owner.Rigidbody.position;
            float offset = owner.PatrolSpeed * owner.Direction * Time.fixedDeltaTime;

            switch (owner.PatrolDirection)
            {
                case PatrolDirection.Horizontal:
                    position.x += offset;
                    owner.Animator.SetFloat(AnimatorHashes.MoveX, owner.Direction);
                    owner.Animator.SetFloat(AnimatorHashes.MoveY, 0f);
                    break;

                case PatrolDirection.Vertical:
                    position.y += offset;
                    owner.Animator.SetFloat(AnimatorHashes.MoveX, 0f);
                    owner.Animator.SetFloat(AnimatorHashes.MoveY, owner.Direction);
                    break;
            }

            owner.Rigidbody.MovePosition(position);
        }

        public override void OnProjectileHit(PatrolRobot owner)
        {
            QuestReporter reporter = owner.GetComponent<QuestReporter>();
            if (reporter != null && reporter.Data != null && !reporter.CanReport())
                return;

            owner.ChangeState(owner.FixedState);
        }
    }
}
