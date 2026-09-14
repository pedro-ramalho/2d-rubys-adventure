using UnityEngine;

namespace AdventureGame.Entities.Enemy.Instances.Vending_Machine.States
{
    public class VendingMachineStunnedState : VendingMachineState
    {
        private float m_Timer;

        public override void Enter(VendingMachine owner)
        {
            m_Timer = 0f;
            owner.SpriteRenderer.color = owner.StunnedTint;

            owner.AudioSource.PlayOneShot(owner.StunnedClip);
        }

        public override void Update(VendingMachine owner)
        {
            m_Timer += Time.deltaTime;

            float t = Mathf.Clamp01(m_Timer / owner.StunnedDuration);
            owner.SpriteRenderer.color = Color.Lerp(owner.StunnedTint, owner.BaseColor, t);

            if (m_Timer >= owner.StunnedDuration)
                owner.ChangeState(owner.MovingState);
        }
    }
}
