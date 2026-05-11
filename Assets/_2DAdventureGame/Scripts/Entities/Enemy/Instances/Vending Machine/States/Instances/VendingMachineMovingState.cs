using UnityEngine;

public class VendingMachineMovingState : VendingMachineState
{
    public override string ID => "Moving";

    private float directionTimer;

    public override void Enter(VendingMachine owner)
    {
        directionTimer = owner.Data.patrolDuration;
        owner.Direction = 1;
    }

    public override void Update(VendingMachine owner)
    {
        directionTimer -= Time.deltaTime;
        if (directionTimer <= 0f)
        {
            owner.Direction = -owner.Direction;
            directionTimer = owner.Data.patrolDuration;
        }
    }

    public override void FixedUpdate(VendingMachine owner)
    {
        Vector2 position = owner.Rigidbody.position;
        float offset = owner.Data.speed * owner.Direction * Time.fixedDeltaTime;

        switch (owner.Data.patrolDirection)
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
}
