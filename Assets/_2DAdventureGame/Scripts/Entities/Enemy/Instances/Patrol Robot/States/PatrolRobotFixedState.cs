using UnityEngine;

public class PatrolRobotFixedState : PatrolRobotState
{
    public override void Enter(PatrolRobot owner)
    {
        owner.Rigidbody.simulated = false;
        owner.Animator.SetTrigger(AnimatorHashes.Fixed);
        owner.AudioSource.Stop();
        
        if (owner.FixedClip != null) 
            owner.AudioSource.PlayOneShot(owner.FixedClip);
        
        if (owner.FixedEffectPrefab != null)
            Object.Instantiate(owner.FixedEffectPrefab, owner.SpriteRenderer.bounds.center, Quaternion.identity);
        
        if (owner.SmokeEffect != null) 
            owner.SmokeEffect.Stop();

        QuestReporter reporter = owner.GetComponent<QuestReporter>();
        if (reporter != null) 
            reporter.Report();
    }
}
