using AdventureGame.Core.Constants;
using AdventureGame.Core.Quest;
using AdventureGame.Entities.Enemy.Instances.Patrol_Robot.States;
using UnityEngine;
using UnityEngine.Serialization;

namespace AdventureGame.Entities.Enemy.Instances.Patrol_Robot
{
    public enum PatrolDirection { Horizontal, Vertical }

    public class PatrolRobot : Enemy
    {
        [Header("Patrol Robot Assets")]
        
        [FormerlySerializedAs("smokeEffect")]
        [SerializeField] private ParticleSystem m_SmokeEffect;

        [FormerlySerializedAs("fixedEffectPrefab")]
        [SerializeField] private GameObject m_FixedEffectPrefab;

        [FormerlySerializedAs("fixedClip")]
        [SerializeField] private AudioClip m_FixedClip;

        [FormerlySerializedAs("hitClip")]
        [SerializeField] private AudioClip m_HitClip;
        public ParticleSystem SmokeEffect => m_SmokeEffect;
        public GameObject FixedEffectPrefab => m_FixedEffectPrefab;
        public AudioClip FixedClip => m_FixedClip;
        public AudioClip HitClip => m_HitClip;

        [Header("Patrolling Properties")]
        [FormerlySerializedAs("patrolDirection")]
        [SerializeField] private PatrolDirection m_PatrolDirection;

        [FormerlySerializedAs("patrolDuration")]
        [SerializeField] private float m_PatrolDuration;

        [FormerlySerializedAs("speed")]
        [SerializeField] private float m_Speed;
        public PatrolDirection PatrolDirection => m_PatrolDirection;
        public float PatrolDuration => m_PatrolDuration;
        public float Speed => m_Speed;

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
        
            if (m_SmokeEffect != null) 
                m_SmokeEffect.Stop();
        
            CurrentState = FixedState;
        }

        protected override void OnProjectileHit() => CurrentState.OnProjectileHit(this);

        void OnTriggerStay2D(Collider2D other)
        {
            if (CurrentState == FixedState) 
                return;
        
            if (!other.TryGetComponent(out Player.Player player)) 
                return;
        
            if (player.IsInvincible) 
                return;

            player.ApplyDamage(Data.ContactDamage);
            if (m_HitClip != null) 
                AudioSource.PlayOneShot(m_HitClip);
        }
    }
}