using AdventureGame.Core.Constants;
using UnityEngine;

namespace AdventureGame.Entities.Enemy.Instances.Vending_Machine.States
{
    public class VendingMachineWindupState : VendingMachineState
    {
        private float m_Timer;

        public override void Enter(VendingMachine owner)
        {
            m_Timer = 0f;

            owner.ChargeDirection = (Player.Player.Instance.Rigidbody.position - owner.Rigidbody.position).normalized;

            owner.Animator.SetFloat(AnimatorHashes.MoveX, owner.ChargeDirection.x);
            owner.Animator.SetFloat(AnimatorHashes.MoveY, owner.ChargeDirection.y);

            owner.Rigidbody.linearVelocity = Vector2.zero;

            owner.AudioSource.PlayOneShot(owner.Data.WindupClip);
        }

        public override void Update(VendingMachine owner)
        {
            m_Timer += Time.deltaTime;

            float t = Mathf.Clamp01(m_Timer / owner.Data.WindupDuration);
            owner.SpriteRenderer.color = Color.Lerp(owner.BaseColor, owner.Data.ChargeTint, t);

            if (m_Timer >= owner.Data.WindupDuration)
                owner.ChangeState(owner.ChargingState);
        }
    }
}
