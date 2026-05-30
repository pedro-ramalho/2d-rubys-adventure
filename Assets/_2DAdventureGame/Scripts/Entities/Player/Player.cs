using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class Player : MonoBehaviour, IDamageable
{
    public static Player Instance { get; private set; }

    // Components
    public Rigidbody2D Rigidbody { get; private set; }
    public Animator Animator { get; private set; }
    public SpriteRenderer SpriteRenderer { get; private set; }

    [Header("Player Data")]
    [SerializeField] private PlayerData data;
    public PlayerData Data => data;

    [Header("Player Input")]
    private PlayerInputActions inputActions;
    public InputAction MoveAction => inputActions.Player.Movement;
    public InputAction DashAction => inputActions.Player.Dash;
    public InputAction ShootAction => inputActions.Player.Shoot;
    public InputAction TalkAction => inputActions.Player.Talk;

    [Header("Player Assets")]
    [SerializeField] private AudioSource oneShotSource;
    [SerializeField] private AudioClip dashClip;
    [SerializeField] private AudioClip hitClip;
    [SerializeField] private AudioClip launchClip;
    [SerializeField] private GameObject afterimagePrefab;
    [SerializeField] private GameObject projectilePrefab;
    public AudioSource OneShotSource => oneShotSource;
    public AudioClip DashClip => dashClip;
    public AudioClip HitClip => hitClip;
    public AudioClip LaunchClip => launchClip;
    public GameObject AfterimagePrefab => afterimagePrefab;
    public GameObject ProjectilePrefab => projectilePrefab;

    // Animator Hashes
    public static readonly int LookXHash = Animator.StringToHash("Look X");
    public static readonly int LookYHash = Animator.StringToHash("Look Y");
    public static readonly int SpeedHash = Animator.StringToHash("Speed");
    public static readonly int HitHash = Animator.StringToHash("Hit");
    public static readonly int ShootHash = Animator.StringToHash("Launch");

    // Health
    public int CurrentHealth { get; set; }
    public bool IsInvincible { get; set; }
    public float DamageCooldown { get; set; }

    // Movement
    public Vector2 MoveDirection { get; set; } = Vector2.right;
    public Vector2 CurrentVelocity { get; set; }
    public float DashCooldownTimer { get; set; }

    // Interaction
    public NPC LastNPC { get; set; }

    // State
    public PlayerState CurrentState { get; private set; }
    public event Action<float> OnHealthChanged;
    public event Action OnDied;

    // State instances
    public PlayerGroundedState GroundedState { get; private set; }
    public PlayerDashingState DashingState { get; private set; }
    public PlayerShootingState ShootingState { get; private set; }
    public PlayerDeadState DeadState { get; private set; }

    void Awake()
    {
        if (Instance == null) Instance = this;

        Rigidbody = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();
        SpriteRenderer = GetComponent<SpriteRenderer>();

        CurrentHealth = Mathf.Clamp(data.startingHealth, 0, data.maxHealth);

        inputActions = new PlayerInputActions();

        GroundedState = new PlayerGroundedState();
        DashingState = new PlayerDashingState();
        ShootingState = new PlayerShootingState();
        DeadState = new PlayerDeadState();

        CurrentState = GroundedState;
        CurrentState.Enter(this);
    }

    void OnEnable() => inputActions.Player.Enable();
    void OnDisable() => inputActions.Player.Disable();

    void Update()
    {
        if (PauseManager.IsPaused) return;
        UpdateTimers();
        CurrentState.Update(this);
    }

    void FixedUpdate() => CurrentState.FixedUpdate(this);

    void UpdateTimers()
    {
        if (IsInvincible)
        {
            DamageCooldown -= Time.deltaTime;
            if (DamageCooldown <= 0f)
                IsInvincible = false;
        }

        if (DashCooldownTimer > 0f)
            DashCooldownTimer -= Time.deltaTime;    
    }

    public void ChangeState(PlayerState newState)
    {
        CurrentState.Exit(this);
        CurrentState = newState;
        CurrentState.Enter(this);
    }

    public void Heal(int amount) => CurrentState.HandleHeal(this, amount);

    public void ApplyDamage(int amount) => CurrentState.HandleDamage(this, amount);

    public void RaiseOnHealthChanged(float percentage) => OnHealthChanged?.Invoke(percentage);
    public void RaiseOnDied() => OnDied?.Invoke();
}
