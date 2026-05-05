using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(SpriteRenderer))]
public class Player : MonoBehaviour
{
    // Components
    public Rigidbody2D Rigidbody { get; private set; }
    public Animator Animator { get; private set; }
    public AudioSource AudioSource { get; private set; }
    public SpriteRenderer SpriteRenderer { get; private set; }

    // Data
    [SerializeField] private PlayerData data;
    public PlayerData Data => data;

    // Input Actions
    [SerializeField] private InputAction moveAction;
    [SerializeField] private InputAction dashAction;
    [SerializeField] private InputAction shootAction;
    [SerializeField] private InputAction talkAction;
    [SerializeField] private InputAction pauseAction;
    public InputAction MoveAction => moveAction;
    public InputAction DashAction => dashAction;
    public InputAction ShootAction => shootAction;
    public InputAction TalkAction => talkAction;
    public InputAction PauseAction => pauseAction;

    // Assets
    [SerializeField] private AudioClip walkClip;
    [SerializeField] private AudioClip dashClip;
    [SerializeField] private AudioClip hitClip;
    [SerializeField] private AudioClip launchClip;
    [SerializeField] private GameObject afterimagePrefab;
    [SerializeField] private GameObject projectilePrefab;
    public AudioClip WalkClip => walkClip;
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
    public PlayerDeadState DeadState { get; private set; }

    void Awake()
    {
        Rigidbody = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();
        AudioSource = GetComponent<AudioSource>();
        SpriteRenderer = GetComponent<SpriteRenderer>();

        CurrentHealth = Mathf.Clamp(data.startingHealth, 0, data.maxHealth);

        GroundedState = new PlayerGroundedState();
        DashingState = new PlayerDashingState();
        DeadState = new PlayerDeadState();

        CurrentState = GroundedState;
        CurrentState.Enter(this);
    }

    void OnEnable() => EnableAllActions(true);
    void OnDisable() => EnableAllActions(false);

    void Update() => CurrentState.Update(this);
    void FixedUpdate() => CurrentState.FixedUpdate(this);

    public void ChangeState(PlayerState newState)
    {
        CurrentState.Exit(this);
        CurrentState = newState;
        CurrentState.Enter(this);
    }

    public void TakeDamage(int amount)
    {
        if (CurrentState is PlayerDeadState) return;
        CurrentState.HandleDamage(this, amount);
    }

    public void RaiseOnHealthChanged(float percentage) => OnHealthChanged?.Invoke(percentage);
    public void RaiseOnDied() => OnDied?.Invoke();

    void EnableAllActions(bool enable)
    {
        if (enable)
        {
            moveAction.Enable();
            dashAction.Enable();
            shootAction.Enable();
            talkAction.Enable();
            pauseAction.Enable();
        }
        else
        {
            moveAction.Disable();
            dashAction.Disable();
            shootAction.Disable();
            talkAction.Disable();
            pauseAction.Disable();
        }
    }
}
