public abstract class PatrolRobotState : State<PatrolRobot>
{
    public virtual void OnProjectileHit(PatrolRobot owner) { }
}
