using UnityEngine;

public enum PatrolDirection { Horizontal, Vertical }

public class PatrolRobot : Enemy
{
    [Header("Patrol Robot Assets")]
    [SerializeField] private ParticleSystem smokeEffect;
    [SerializeField] private GameObject fixedEffectPrefab;
    [SerializeField] private AudioClip fixedClip;
    [SerializeField] private AudioClip hitClip;
    public ParticleSystem SmokeEffect => smokeEffect;
    public GameObject FixedEffectPrefab => fixedEffectPrefab;
    public AudioClip FixedClip => fixedClip;
    public AudioClip HitClip => hitClip;

    [Header("Patrolling Properties")]
    [SerializeField] private PatrolDirection patrolDirection;
    [SerializeField] private float patrolDuration;
    [SerializeField] private float speed;
    public PatrolDirection PatrolDirection => patrolDirection;
    public float PatrolDuration => patrolDuration;
    public float Speed => speed;

    public int Direction { get; set; }

    public SpriteRenderer SpriteRenderer { get; private set; }

    public PatrolRobotState CurrentState { get; private set; }
    public PatrolRobotPatrollingState PatrollingState { get; private set; }
    public PatrolRobotFixedState FixedState { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        SpriteRenderer = GetComponentInChildren<SpriteRenderer>();

        PatrollingState = new PatrolRobotPatrollingState();
        FixedState = new PatrolRobotFixedState();

        CurrentState = PatrollingState;
        CurrentState.Enter(this);
    }

    void Start()
    {
        QuestReporter reporter = GetComponent<QuestReporter>();
        if (reporter != null && reporter.IsConsumed())
            EnterFixedSilent();
    }

    void Update() => CurrentState.Update(this);

    void FixedUpdate() => CurrentState.FixedUpdate(this);

    public void ChangeState(PatrolRobotState newState)
    {
        CurrentState.Exit(this);
        CurrentState = newState;
        CurrentState.Enter(this);
    }

    public void EnterFixedSilent()
    {
        Rigidbody.simulated = false;
        Animator.SetTrigger(AnimatorHashes.Fixed);
        AudioSource.Stop();
        
        if (smokeEffect != null) 
            smokeEffect.Stop();
        
        CurrentState = FixedState;
    }

    protected override void OnProjectileHit() => CurrentState.OnProjectileHit(this);

    void OnTriggerStay2D(Collider2D other)
    {
        if (CurrentState == FixedState) 
            return;
        
        if (!other.TryGetComponent(out Player player)) 
            return;
        
        if (player.IsInvincible) 
            return;

        player.ApplyDamage(Data.contactDamage);
        if (hitClip != null) 
            AudioSource.PlayOneShot(hitClip);
    }
}
