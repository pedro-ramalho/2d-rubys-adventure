using UnityEngine;

public class VendingMachineChargingState : VendingMachineState
{
    private float timer;
    private Vector2 direction;

    public override void Enter(VendingMachine owner)
    {
        timer = 0f;
        direction = owner.ChargeDirection;

        owner.Animator.SetFloat(AnimatorHashes.MoveX, direction.x);
        owner.Animator.SetFloat(AnimatorHashes.MoveY, direction.y);
        owner.Animator.SetBool(AnimatorHashes.ChargingHorizontal, owner.IsChargeHorizontal);

        owner.AudioSource.PlayOneShot(owner.ChargeClip);
    }

    public override void Exit(VendingMachine owner) => owner.Animator.SetBool(AnimatorHashes.ChargingHorizontal, false);
    
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
