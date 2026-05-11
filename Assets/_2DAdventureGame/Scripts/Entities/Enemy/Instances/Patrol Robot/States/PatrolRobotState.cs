public abstract class PatrolRobotState : State<PatrolRobot>
{
    public abstract override string ID { get; }
    public override void Enter(PatrolRobot owner) { }
    public override void Update(PatrolRobot owner) { }
    public override void FixedUpdate(PatrolRobot owner) { }
    public override void Exit(PatrolRobot owner) { }

    public virtual void OnProjectileHit(PatrolRobot owner) { }
}
