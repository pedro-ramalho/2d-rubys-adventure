using AdventureGame.Core.Constants;
using UnityEngine;

namespace AdventureGame.Entities.Enemy.Instances.Vending_Machine.States
{
    public class VendingMachineMovingState : VendingMachineState
    {
        public override void Enter(VendingMachine owner)
        {
            owner.AudioSource.clip = owner.WalkingClip;
            owner.AudioSource.loop = true;
            owner.AudioSource.Play();
        }

        public override void Exit(VendingMachine owner) => owner.AudioSource.Stop();

        public override void Update(VendingMachine owner)
        {
            float distance = Vector2.Distance(
                owner.Rigidbody.position,
                Player.Player.Instance.Rigidbody.position
            );

            if (distance <= owner.DetectionRadius)
                owner.ChangeState(owner.WindupState);
        }

        public override void FixedUpdate(VendingMachine owner)
        {
            Vector2 toPlayer = (Player.Player.Instance.Rigidbody.position - owner.Rigidbody.position).normalized;
            Vector2 step = toPlayer * (owner.Data.speed * Time.fixedDeltaTime);

            owner.Rigidbody.MovePosition(owner.Rigidbody.position + step);

            owner.Animator.SetFloat(AnimatorHashes.MoveX, toPlayer.x);
            owner.Animator.SetFloat(AnimatorHashes.MoveY, toPlayer.y);
        }
    }
}
