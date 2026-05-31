using UnityEngine;

public class VendingMachine : Enemy
{  
    public static readonly int MoveXHash = Animator.StringToHash("Move X");
    public static readonly int MoveYHash = Animator.StringToHash("Move Y");
    public static readonly int ChargingHorizontalHash = Animator.StringToHash("ChargingHorizontal");

    [Header("Vending Machine Assets")]
    [SerializeField] private ParticleSystem smokeEffect;
    public ParticleSystem SmokeEffect => smokeEffect;

    [Header("Tint")]
    [SerializeField] private Color chargeTint = new Color(1f, 0.35f, 0.35f, 1f);
    [SerializeField] private Color stunnedTint = new Color(0.5f, 0.5f, 0.5f, 1f);
    public Color ChargeTint => chargeTint;
    public Color StunnedTint => stunnedTint;

    public SpriteRenderer SpriteRenderer { get; private set; }
    public Color BaseColor { get; private set; }

    [Header("Collision Behavior")]
    [SerializeField] private string wallTag = "Wall";
    [SerializeField] private GameObject collisionExplosionPrefab;

    [Header("Charging Properties")]
    [SerializeField] private float detectionRadius = 5f;
    [SerializeField] private float windupDuration = 0.3f;
    [SerializeField] private AnimationCurve speedCurve;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float chargeDuration;
    [SerializeField] private float stunnedDuration;
    public float DetectionRadius => detectionRadius;
    public float WindupDuration => windupDuration;
    public AnimationCurve SpeedCurve => speedCurve;
    public float MaxSpeed => maxSpeed;
    public float ChargeDuration => chargeDuration;
    public float StunnedDuration => stunnedDuration;

    public int Direction { get; set; }
    public Vector2 ChargeDirection { get; set; }
    public bool IsChargeHorizontal => Mathf.Abs(ChargeDirection.x) > Mathf.Abs(ChargeDirection.y);

    public VendingMachineState CurrentState { get; private set; }
    public VendingMachineMovingState MovingState { get; private set; }
    public VendingMachineWindupState WindupState { get; private set; }
    public VendingMachineChargingState ChargingState { get; private set; }
    public VendingMachineStunnedState StunnedState { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        SpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        BaseColor = SpriteRenderer.color;

        MovingState = new VendingMachineMovingState();
        WindupState = new VendingMachineWindupState();
        ChargingState = new VendingMachineChargingState();
        StunnedState = new VendingMachineStunnedState();

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
