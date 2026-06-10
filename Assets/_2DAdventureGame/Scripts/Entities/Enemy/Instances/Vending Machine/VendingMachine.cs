using UnityEngine;

public class VendingMachine : Enemy
{  
    [Header("Tint")]
    [SerializeField] private Color chargeTint = new Color(1f, 0.35f, 0.35f, 1f);
    [SerializeField] private Color stunnedTint = new Color(0.5f, 0.5f, 0.5f, 1f);
    public Color ChargeTint => chargeTint;
    public Color StunnedTint => stunnedTint;

    public SpriteRenderer SpriteRenderer { get; private set; }
    public Color BaseColor { get; private set; }

    [Header("Audio")]
    [SerializeField] private AudioClip walkingClip;
    [SerializeField] private AudioClip windupClip;
    [SerializeField] private AudioClip chargeClip;
    [SerializeField] private AudioClip stunnedClip;
    public AudioClip WalkingClip => walkingClip;
    public AudioClip WindupClip => windupClip;
    public AudioClip ChargeClip => chargeClip;
    public AudioClip StunnedClip => stunnedClip;

    [Header("Collision Behavior")]
    [SerializeField] private GameObject explosionPrefab;

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
        if (collision.gameObject.TryGetComponent(out VendingMachine _))
        {
            SpawnExplosion(transform.position);
            GetComponent<QuestReporter>()?.Report();
            Destroy(gameObject);
            return;
        }

        if (collision.gameObject.TryGetComponent(out Player player))
        {
            player.ApplyDamage(Data.contactDamage);

            if (CurrentState == ChargingState)
            {
                SpawnExplosion(collision.GetContact(0).point);
                ChangeState(StunnedState);
            }
            return;
        }

        if (CurrentState == ChargingState && collision.gameObject.CompareTag(Tags.Wall))
        {
            SpawnExplosion(collision.GetContact(0).point);
            ChangeState(StunnedState);
        }
    }

    void SpawnExplosion(Vector2 position)
    {
        if (explosionPrefab != null)
            Instantiate(explosionPrefab, position, Quaternion.identity);
    }

    public void ChangeState(VendingMachineState newState)
    {
        CurrentState.Exit(this);
        CurrentState = newState;
        CurrentState.Enter(this);
    }

    protected override void OnProjectileHit()
    {
        SpawnExplosion(transform.position);
        GetComponent<QuestReporter>()?.Report();
        Destroy(gameObject);
    }
}
