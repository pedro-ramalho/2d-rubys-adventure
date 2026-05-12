using UnityEngine;

public class VendingMachineStunnedState : VendingMachineState
{
    public override string ID => "Stunned";

    private float timer;

    public override void Enter(VendingMachine owner)
    {
        timer = 0f;
    }

    public override void Update(VendingMachine owner)
    {
        timer += Time.deltaTime;
        if (timer >= owner.Data.stunnedDuration)
            owner.ChangeState(owner.MovingState);
    }
}
