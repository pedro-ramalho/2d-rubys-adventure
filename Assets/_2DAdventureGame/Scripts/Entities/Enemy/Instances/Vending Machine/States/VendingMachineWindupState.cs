using AdventureGame.Core.Constants;
using UnityEngine;

namespace AdventureGame.Entities.Enemy.Instances.Vending_Machine.States
{
    public class VendingMachineWindupState : VendingMachineState
    {
        private float timer;

        public override void Enter(VendingMachine owner)
        {
            timer = 0f;

            owner.ChargeDirection = (Player.Player.Instance.Rigidbody.position - owner.Rigidbody.position).normalized;

            owner.Animator.SetFloat(AnimatorHashes.MoveX, owner.ChargeDirection.x);
            owner.Animator.SetFloat(AnimatorHashes.MoveY, owner.ChargeDirection.y);

            owner.Rigidbody.linearVelocity = Vector2.zero;

            owner.AudioSource.PlayOneShot(owner.WindupClip);
        }

        public override void Update(VendingMachine owner)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(timer / owner.WindupDuration);
            owner.SpriteRenderer.color = Color.Lerp(owner.BaseColor, owner.ChargeTint, t);

            if (timer >= owner.WindupDuration)
                owner.ChangeState(owner.ChargingState);
        }
    }
}
