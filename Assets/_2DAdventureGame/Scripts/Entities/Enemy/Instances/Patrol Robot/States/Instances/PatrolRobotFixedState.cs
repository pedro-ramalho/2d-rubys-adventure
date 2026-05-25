using UnityEngine;

public class PatrolRobotFixedState : PatrolRobotState
{
    public override string ID => "Fixed";

    public override void Enter(PatrolRobot owner)
    {
        owner.Rigidbody.simulated = false;
        owner.Animator.SetTrigger(PatrolRobot.FixedHash);
        owner.AudioSource.Stop();
        if (owner.FixedClip != null) owner.AudioSource.PlayOneShot(owner.FixedClip);
        owner.SmokeEffect.Stop();
        owner.RaiseOnFixed();
        owner.GetComponent<QuestReporter>()?.Report();
    }
}
