using UnityEngine;

public class FixedState : EnemyState
{
    public override EnemyStateID ID => EnemyStateID.Fixed;

    public override void Enter(Enemy owner)
    {
        owner.Rigidbody.simulated = false;
        owner.Animator.SetTrigger(Enemy.FixedHash);
        owner.AudioSource.Stop();
        owner.Data.smokeParticleEffect.Stop();
        owner.RaiseOnFixed();
    }
}
