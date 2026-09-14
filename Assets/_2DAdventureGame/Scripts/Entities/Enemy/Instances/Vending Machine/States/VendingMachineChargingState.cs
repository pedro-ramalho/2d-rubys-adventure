using AdventureGame.Core.Constants;
using UnityEngine;

namespace AdventureGame.Entities.Enemy.Instances.Vending_Machine.States
{
    public class VendingMachineChargingState : VendingMachineState
    {
        private float m_Timer;
        private Vector2 m_Direction;

        public override void Enter(VendingMachine owner)
        {
            m_Timer = 0f;
            m_Direction = owner.ChargeDirection;

            owner.Animator.SetFloat(AnimatorHashes.MoveX, m_Direction.x);
            owner.Animator.SetFloat(AnimatorHashes.MoveY, m_Direction.y);
            owner.Animator.SetBool(AnimatorHashes.ChargingHorizontal, owner.IsChargeHorizontal);

            owner.AudioSource.PlayOneShot(owner.ChargeClip);
        }

        public override void Exit(VendingMachine owner) => owner.Animator.SetBool(AnimatorHashes.ChargingHorizontal, false);
    
        public override void Update(VendingMachine owner)
        {
            m_Timer += Time.deltaTime;
            if (m_Timer >= owner.ChargeDuration)
                owner.ChangeState(owner.StunnedState);
        }

        public override void FixedUpdate(VendingMachine owner)
        {
            float progress = m_Timer / owner.ChargeDuration;
            float speed = owner.SpeedCurve.Evaluate(progress) * owner.MaxSpeed;

            owner.Rigidbody.MovePosition(
                owner.Rigidbody.position + m_Direction * (speed * Time.fixedDeltaTime)
            );
        }
    }
}
