using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{  
    // Components
    public Rigidbody2D Rigidbody { get; private set; }
    public Animator Animator { get; private set; }
    public AudioSource AudioSource { get; private set; }

    // Enemy Data
    [Header("Enemy Data")]
    [SerializeField] EnemyData data;
    public EnemyData Data => data;

    // Health
    public int CurrentHealth { get; set; }

    // Hashes
    public static readonly int MoveXHash = Animator.StringToHash("Move X");
    public static readonly int MoveYHash = Animator.StringToHash("Move Y");
    public static readonly int FixedHash = Animator.StringToHash("Fixed");

    // State
    public EnemyState CurrentState { get; private set; }

    // State instances 
    public PatrollingState PatrollingState { get; private set; }
    public FixedState FixedState { get; private set; }

    // Direction
    public int Direction { get; set; }

    // Events
    public event Action OnFixed;

    // Fixed
    public bool IsFixed { get; set; }
    public float DirectionTimer { get; set; }

    void Awake()
    {
        Rigidbody = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();
        AudioSource = GetComponent<AudioSource>();

        CurrentHealth = data.maxHealth;
        Direction = 1;

        PatrollingState = new PatrollingState();
        FixedState = new FixedState();

        CurrentState = PatrollingState;
        CurrentState.Enter(this);        
    }

    void Update()
    {
        if (CurrentState.ID != EnemyStateID.Patrolling || IsFixed) return;

        DirectionTimer -= Time.deltaTime;
        if (DirectionTimer < 0)
        {
            Direction = -Direction;
            DirectionTimer = Data.patrolDuration;
        }
    }

    void FixedUpdate() => CurrentState.FixedUpdate(this);

    public void ChangeState(EnemyState newState)
    {
        CurrentState.Exit(this);
        CurrentState = newState;
        CurrentState.Enter(this);
    }

    public void RaiseOnFixed() => OnFixed?.Invoke();

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("Projectile")) CurrentState.OnProjectileHit(this);
    }
}
