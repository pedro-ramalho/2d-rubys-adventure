using UnityEngine;

public class PatrolRobotPatrollingState : PatrolRobotState
{
    private float directionTimer;

    public override void Enter(PatrolRobot owner)
    {
        directionTimer = owner.PatrolDuration;
        owner.Direction = 1;
    }

    public override void Update(PatrolRobot owner)
    {
        directionTimer -= Time.deltaTime;
        if (directionTimer <= 0f)
        {
            owner.Direction = -owner.Direction;
            directionTimer = owner.PatrolDuration;
        }
    }

    public override void FixedUpdate(PatrolRobot owner)
    {
        Vector2 position = owner.Rigidbody.position;
        float offset = owner.Speed * owner.Direction * Time.fixedDeltaTime;

        switch (owner.PatrolDirection)
        {
            case PatrolDirection.Horizontal:
                position.x += offset;
                owner.Animator.SetFloat(PatrolRobot.MoveXHash, owner.Direction);
                owner.Animator.SetFloat(PatrolRobot.MoveYHash, 0f);
                break;
            case PatrolDirection.Vertical:
                position.y += offset;
                owner.Animator.SetFloat(PatrolRobot.MoveXHash, 0f);
                owner.Animator.SetFloat(PatrolRobot.MoveYHash, owner.Direction);
                break;
        }

        owner.Rigidbody.MovePosition(position);
    }

    public override void OnProjectileHit(PatrolRobot owner) => owner.ChangeState(owner.FixedState);
}
