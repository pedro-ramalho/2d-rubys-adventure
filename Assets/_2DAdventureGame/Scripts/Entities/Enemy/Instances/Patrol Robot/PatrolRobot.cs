using UnityEngine;

public enum PatrolDirection { Horizontal, Vertical }

public class PatrolRobot : Enemy
{
    public static readonly int MoveXHash = Animator.StringToHash("Move X");
    public static readonly int MoveYHash = Animator.StringToHash("Move Y");
    public static readonly int FixedHash = Animator.StringToHash("Fixed");

    [Header("Patrol Robot Assets")]
    [SerializeField] private ParticleSystem smokeEffect;
    [SerializeField] private AudioClip fixedClip;
    [SerializeField] private AudioClip hitClip;
    public ParticleSystem SmokeEffect => smokeEffect;
    public AudioClip FixedClip => fixedClip;
    public AudioClip HitClip => hitClip;

    [Header("Patrolling Properties")]
    [SerializeField] private PatrolDirection patrolDirection;
    [SerializeField] private float patrolDuration;
    [SerializeField] private float speed;
    public PatrolDirection PatrolDirection => patrolDirection;
    public float PatrolDuration => patrolDuration;
    public float Speed => speed;

    [Header("Combat")]
    [SerializeField] private int contactDamage = 1;

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

    void OnTriggerStay2D(Collider2D other)
    {
        if (CurrentState == FixedState) return;
        if (!other.TryGetComponent(out Player player)) return;
        if (player.IsInvincible) return;

        player.ApplyDamage(contactDamage);
        if (hitClip != null) AudioSource.PlayClipAtPoint(hitClip, transform.position);
    }
}
