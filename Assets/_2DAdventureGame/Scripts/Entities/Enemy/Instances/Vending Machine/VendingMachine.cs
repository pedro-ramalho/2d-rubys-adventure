using UnityEngine;

public class VendingMachine : Enemy
{  
    public static readonly int MoveXHash = Animator.StringToHash("Move X");
    public static readonly int MoveYHash = Animator.StringToHash("Move Y");

    [Header("Vending Machine Assets")]
    [SerializeField] private ParticleSystem smokeEffect;
    public ParticleSystem SmokeEffect => smokeEffect;

    [Header("Patrolling Properties")]
    [SerializeField] private PatrolDirection patrolDirection;
    [SerializeField] private float patrolDuration;
    public PatrolDirection PatrolDirection => patrolDirection;
    public float PatrolDuration => patrolDuration;

    [Header("Collision Behavior")]
    [SerializeField] private string wallTag = "Wall";
    [SerializeField] private GameObject collisionExplosionPrefab;

    public int Direction { get; set; }

    public VendingMachineState CurrentState { get; private set; }
    public VendingMachineMovingState MovingState { get; private set; }
    public VendingMachineStunnedState StunnedState { get; private set; }
    public VendingMachineChargingState ChargingState { get; private set; }

    public Player Player { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        Player = FindAnyObjectByType<Player>();

        MovingState = new VendingMachineMovingState();
        StunnedState = new VendingMachineStunnedState();
        ChargingState = new VendingMachineChargingState();

        CurrentState = MovingState;
        CurrentState.Enter(this);
    }

    void Update() => CurrentState.Update(this);

    void FixedUpdate() => CurrentState.FixedUpdate(this);

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out VendingMachine other))
        {
            if (GetInstanceID() < other.GetInstanceID() && collisionExplosionPrefab != null)
            {
                Vector2 midpoint = (Rigidbody.position + other.Rigidbody.position) * 0.5f;

                Instantiate(collisionExplosionPrefab, midpoint, Quaternion.identity);
            }

            Destroy(gameObject);

            return;
        }

        if (CurrentState == ChargingState && collision.gameObject.CompareTag(wallTag))
            ChangeState(StunnedState);
    }

    public void ChangeState(VendingMachineState newState)
    {
        CurrentState.Exit(this);
        CurrentState = newState;
        CurrentState.Enter(this);
    }

    protected override void OnProjectileHit() => Destroy(gameObject);
}
