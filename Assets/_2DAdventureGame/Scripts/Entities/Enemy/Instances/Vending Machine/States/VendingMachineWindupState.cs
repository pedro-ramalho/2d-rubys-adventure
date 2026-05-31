using UnityEngine;

public class VendingMachineWindupState : VendingMachineState
{
    private float timer;

    public override void Enter(VendingMachine owner)
    {
        timer = 0f;

        owner.ChargeDirection = (Player.Instance.Rigidbody.position - owner.Rigidbody.position).normalized;

        owner.Animator.SetFloat(VendingMachine.MoveXHash, owner.ChargeDirection.x);
        owner.Animator.SetFloat(VendingMachine.MoveYHash, owner.ChargeDirection.y);

        owner.Rigidbody.linearVelocity = Vector2.zero;
    }

    public override void Update(VendingMachine owner)
    {
        timer += Time.deltaTime;
        if (timer >= owner.WindupDuration)
            owner.ChangeState(owner.ChargingState);
    }
}
