using UnityEngine;

public class VendingMachineChargingState : VendingMachineState
{
    public override string ID => "Charging";

    private float timer;
    private Vector2 direction;

    public override void Enter(VendingMachine owner)
    {
        timer = 0f;
        direction = owner.Player.Rigidbody.position - owner.Rigidbody.position;
    }

    public override void Update(VendingMachine owner)
    {
        timer += Time.deltaTime;
        if (timer >= owner.Data.chargeDuration)
            owner.ChangeState(owner.StunnedState);
        
    }

    public override void FixedUpdate(VendingMachine owner)
    {
        float progress = timer / owner.Data.chargeDuration;
        float speed = owner.Data.speedCurve.Evaluate(progress) * owner.Data.maxSpeed;

        owner.Rigidbody.MovePosition(
            owner.Rigidbody.position + direction * (speed * Time.deltaTime)
        );
    }
}
