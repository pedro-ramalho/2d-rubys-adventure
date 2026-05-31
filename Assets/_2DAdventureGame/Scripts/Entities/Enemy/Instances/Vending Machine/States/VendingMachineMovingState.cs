using UnityEngine;

public class VendingMachineMovingState : VendingMachineState
{
    public override void Update(VendingMachine owner)
    {
        float distance = Vector2.Distance(
            owner.Rigidbody.position,
            Player.Instance.Rigidbody.position
        );

        if (distance <= owner.DetectionRadius)
            owner.ChangeState(owner.WindupState);
    }

    public override void FixedUpdate(VendingMachine owner)
    {
        Vector2 toPlayer = (Player.Instance.Rigidbody.position - owner.Rigidbody.position).normalized;
        Vector2 step = toPlayer * (owner.Data.speed * Time.fixedDeltaTime);

        owner.Rigidbody.MovePosition(owner.Rigidbody.position + step);

        owner.Animator.SetFloat(VendingMachine.MoveXHash, toPlayer.x);
        owner.Animator.SetFloat(VendingMachine.MoveYHash, toPlayer.y);
    }
}
