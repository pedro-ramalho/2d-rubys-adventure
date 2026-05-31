using UnityEngine;

public class VendingMachineStunnedState : VendingMachineState
{
    private float timer;

    public override void Enter(VendingMachine owner)
    {
        timer = 0f;
        owner.SpriteRenderer.color = owner.StunnedTint;
    }

    public override void Update(VendingMachine owner)
    {
        timer += Time.deltaTime;

        float t = Mathf.Clamp01(timer / owner.StunnedDuration);
        owner.SpriteRenderer.color = Color.Lerp(owner.StunnedTint, owner.BaseColor, t);

        if (timer >= owner.StunnedDuration)
            owner.ChangeState(owner.MovingState);
    }
}
