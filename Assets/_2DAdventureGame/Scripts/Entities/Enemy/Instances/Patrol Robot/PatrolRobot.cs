using AdventureGame.Core.Constants;
using AdventureGame.Core.Quests;
using AdventureGame.Entities.Enemy.Instances.Patrol_Robot.States;
using UnityEngine;
using UnityEngine.Serialization;

namespace AdventureGame.Entities.Enemy.Instances.Patrol_Robot
{
    public enum PatrolDirection { Horizontal, Vertical }

    public class PatrolRobot : Enemy
    {
        public new PatrolRobotData Data => (PatrolRobotData)base.Data;

        [Header("Patrol Robot Assets")]
        [SerializeField] private ParticleSystem m_SmokeEffect;
        public ParticleSystem SmokeEffect => m_SmokeEffect;

        [Header("Patrolling Properties")]
        [SerializeField] private PatrolDirection m_PatrolDirection;

        [SerializeField] private float m_PatrolDuration;

        [SerializeField] private float m_PatrolSpeed;
        public PatrolDirection PatrolDirection => m_PatrolDirection;
        public float PatrolDuration => m_PatrolDuration;
        public float PatrolSpeed => m_PatrolSpeed;

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
            if (Data.HitClip != null)
                AudioSource.PlayOneShot(Data.HitClip);
        }
    }
}
