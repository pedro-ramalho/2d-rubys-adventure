using UnityEngine;

public class PatrolRobotFixedState : PatrolRobotState
{
    public override string ID => "Fixed";

    public override void Enter(PatrolRobot owner)
    {
        owner.Rigidbody.simulated = false;
        owner.Animator.SetTrigger(PatrolRobot.FixedHash);
        owner.AudioSource.Stop();
        owner.SmokeEffect.Stop();
        owner.RaiseOnFixed();

        QuestManager.Instance?.Report(new QuestReport(QuestObjectiveType.Kill, owner.Data.reportTag));
    }
}
