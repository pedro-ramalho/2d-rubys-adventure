using UnityEngine;

public class VendingMachineChargingState : VendingMachineState
{
    private float timer;
    private Vector2 direction;

    public override void Enter(VendingMachine owner)
    {
        timer = 0f;
        direction = owner.ChargeDirection;

        owner.Animator.SetFloat(VendingMachine.MoveXHash, direction.x);
        owner.Animator.SetFloat(VendingMachine.MoveYHash, direction.y);
        owner.Animator.SetBool(VendingMachine.ChargingHorizontalHash, owner.IsChargeHorizontal);
    }

    public override void Exit(VendingMachine owner)
    {
        owner.Animator.SetBool(VendingMachine.ChargingHorizontalHash, false);
    }

    public override void Update(VendingMachine owner)
    {
        timer += Time.deltaTime;
        if (timer >= owner.ChargeDuration)
            owner.ChangeState(owner.StunnedState);
        
    }

    public override void FixedUpdate(VendingMachine owner)
    {
        float progress = timer / owner.ChargeDuration;
        float speed = owner.SpeedCurve.Evaluate(progress) * owner.MaxSpeed;

        owner.Rigidbody.MovePosition(
            owner.Rigidbody.position + direction * (speed * Time.fixedDeltaTime)
        );
    }
}
