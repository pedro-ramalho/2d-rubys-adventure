using AdventureGame.Core;

namespace AdventureGame.Entities.Enemy.Instances.Patrol_Robot.States
{
    public abstract class PatrolRobotState : State<PatrolRobot>
    {
        public virtual void OnProjectileHit(PatrolRobot owner) { }
    }
}
