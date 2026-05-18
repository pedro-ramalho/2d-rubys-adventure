using UnityEngine;

public enum PatrolDirection { Horizontal, Vertical }

public class PatrolRobot : Enemy
{
    public static readonly int MoveXHash = Animator.StringToHash("Move X");
    public static readonly int MoveYHash = Animator.StringToHash("Move Y");
    public static readonly int FixedHash = Animator.StringToHash("Fixed");

    [Header("Patrol Robot Assets")]
    [SerializeField] private ParticleSystem smokeEffect;
    public ParticleSystem SmokeEffect => smokeEffect;

    [Header("Patrolling Properties")]
    [SerializeField] private PatrolDirection patrolDirection;
    [SerializeField] private float patrolDuration;
    public PatrolDirection PatrolDirection => patrolDirection;
    public float PatrolDuration => patrolDuration;

    public int Direction { get; set; }

    public PatrolRobotState CurrentState { get; private set; }
    public PatrolRobotPatrollingState PatrollingState { get; private set; }
    public PatrolRobotFixedState FixedState { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        PatrollingState = new PatrolRobotPatrollingState();
        FixedState = new PatrolRobotFixedState();

        CurrentState = PatrollingState;
        CurrentState.Enter(this);
    }

    void Update() => CurrentState.Update(this);

    void FixedUpdate() => CurrentState.FixedUpdate(this);

    public void ChangeState(PatrolRobotState newState)
    {
        CurrentState.Exit(this);
        CurrentState = newState;
        CurrentState.Enter(this);
    }

    protected override void OnProjectileHit() => CurrentState.OnProjectileHit(this);
}
