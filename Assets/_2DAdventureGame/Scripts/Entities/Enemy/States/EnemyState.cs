using UnityEngine;

public enum EnemyStateID { Patrolling, Fixed }
public abstract class EnemyState : State<Enemy, EnemyStateID>
{
    public abstract override EnemyStateID ID { get; }

    public override void Enter(Enemy owner) { }

    public override void Update(Enemy owner) { }

    public override void FixedUpdate(Enemy owner) { }

    public override void Exit(Enemy owner) { }

    public virtual void OnProjectileHit(Enemy owner) { }
}
