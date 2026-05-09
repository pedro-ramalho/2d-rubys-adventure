using System.Buffers;
using UnityEngine;

public class PatrollingState : EnemyState
{
    private float directionTimer;

    public override EnemyStateID ID => EnemyStateID.Patrolling;

    public override void Enter(Enemy owner)
    {
        directionTimer = owner.Data.patrolDuration;
        owner.Direction = 1;
    }        

    public override void Update(Enemy owner)
    {
        directionTimer -= Time.deltaTime;
        if (directionTimer < 0)
        {
            owner.Direction = -owner.Direction;
            directionTimer = owner.Data.patrolDuration;
        }
    }

    public override void FixedUpdate(Enemy owner)
    {
        Vector2 position = owner.Rigidbody.position;
        float offset = owner.Data.speed * owner.Direction * Time.deltaTime;

        switch (owner.Data.patrolDirection)
        {
            case PatrolDirection.Horizontal:
                position.x += offset;
                owner.Animator.SetFloat(Enemy.MoveXHash, owner.Direction);
                owner.Animator.SetFloat(Enemy.MoveYHash, 0);
                break;
            case PatrolDirection.Vertical:
                position.y += offset;
                owner.Animator.SetFloat(Enemy.MoveXHash, 0);
                owner.Animator.SetFloat(Enemy.MoveYHash, owner.Direction);
                break;
        }

        owner.Rigidbody.MovePosition(position);
    }

    public override void OnProjectileHit(Enemy owner) => owner.ChangeState(owner.FixedState);         
}
