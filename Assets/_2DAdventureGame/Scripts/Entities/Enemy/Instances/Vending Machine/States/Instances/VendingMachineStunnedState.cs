using UnityEngine;

public class VendingMachineStunnedState : VendingMachineState
{
    private float timer;

    public override void Enter(VendingMachine owner) => timer = 0f;

    public override void Update(VendingMachine owner)
    {
        timer += Time.deltaTime;
        if (timer >= owner.StunnedDuration)
            owner.ChangeState(owner.MovingState);
    }
}
