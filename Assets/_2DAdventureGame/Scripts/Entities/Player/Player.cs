using System;
using AdventureGame.Core.Managers;
using AdventureGame.Entities.Player.States;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace AdventureGame.Entities.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class Player : SceneSingleton<Player>, IDamageable
    {
        // Components
        public Rigidbody2D Rigidbody { get; private set; }
        public Animator Animator { get; private set; }
        public SpriteRenderer SpriteRenderer { get; private set; }

        [Header("Player Data")]
        [FormerlySerializedAs("data")]
        [SerializeField] private PlayerData m_PlayerData;
        public PlayerData Data => m_PlayerData;

        [Header("Player Input")]
        private PlayerInputActions m_InputActions;
        public InputAction MoveAction => m_InputActions.Player.Movement;
        public InputAction DashAction => m_InputActions.Player.Dash;
        public InputAction ShootAction => m_InputActions.Player.Shoot;
        public InputAction TalkAction => m_InputActions.Player.Talk;

        [Header("Player Assets")]
        [field: SerializeField]
        public AudioSource OneShotSource { get; private set; }

        [field: SerializeField]
        public AudioClip DashClip { get; private set; }
    
        [field: SerializeField]
        public AudioClip HitClip { get; private set; }
    
        [field: SerializeField]
        public AudioClip LaunchClip { get; private set; }
    
        [field: SerializeField]
        public GameObject AfterimagePrefab { get; private set; }
    
        [field: SerializeField]
        public GameObject ProjectilePrefab { get; private set; }

        public int CurrentHealth { get; set; }
        public bool IsInvincible { get; set; }
        public float DamageCooldown { get; set; }

        public Vector2 MoveDirection { get; set; } = Vector2.up;
        public Vector2 CurrentVelocity { get; set; }
        public float DashCooldownTimer { get; set; }

        public PlayerState CurrentState { get; private set; }
        public event Action<float> OnHealthChanged;
        public event Action OnDied;

        public PlayerGroundedState GroundedState { get; private set; }
        public PlayerDashingState DashingState { get; private set; }
        public PlayerShootingState ShootingState { get; private set; }
        public PlayerDeadState DeadState { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            if (Instance != this) return;

            Rigidbody = GetComponent<Rigidbody2D>();
            Animator = GetComponent<Animator>();
            SpriteRenderer = GetComponent<SpriteRenderer>();

            int health = SaveManager.Instance != null && SaveManager.Instance.HasSave && SaveManager.Instance.Current.PlayerHealth >= 0
                ? SaveManager.Instance.Current.PlayerHealth
                : m_PlayerData.StartingHealth;
            CurrentHealth = Mathf.Clamp(health, 0, m_PlayerData.MaxHealth);

            m_InputActions = InputManager.Instance.Actions;

            GroundedState = new PlayerGroundedState();
            DashingState = new PlayerDashingState();
            ShootingState = new PlayerShootingState();
            DeadState = new PlayerDeadState();

            CurrentState = GroundedState;
            CurrentState.Enter(this);
        }

        void Update()
        {
            if (PauseManager.IsPaused) 
                return;
        
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
}
