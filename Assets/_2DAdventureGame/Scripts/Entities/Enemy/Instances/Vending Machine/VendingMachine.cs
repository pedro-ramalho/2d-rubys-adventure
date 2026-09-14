using AdventureGame.Core.Constants;
using AdventureGame.Core.Quest;
using AdventureGame.Entities.Enemy.Instances.Vending_Machine.States;
using UnityEngine;
using UnityEngine.Serialization;

namespace AdventureGame.Entities.Enemy.Instances.Vending_Machine
{
    public class VendingMachine : Enemy
    {  
        [Header("Tint")]
        [FormerlySerializedAs("chargeTint")]
        [SerializeField] private Color m_ChargeTint = new Color(1f, 0.35f, 0.35f, 1f);
        
        [FormerlySerializedAs("stunnedTint")]
        [SerializeField] private Color m_StunnedTint = new Color(0.5f, 0.5f, 0.5f, 1f);
        public Color ChargeTint => m_ChargeTint;
        public Color StunnedTint => m_StunnedTint;

        public SpriteRenderer SpriteRenderer { get; private set; }
        public Color BaseColor { get; private set; }

        [Header("Audio")]
        [FormerlySerializedAs("walkingClip")]
        [SerializeField] private AudioClip m_WalkingClip;

        [FormerlySerializedAs("windupClip")]
        [SerializeField] private AudioClip m_WindupClip;

        [FormerlySerializedAs("chargeClip")]
        [SerializeField] private AudioClip m_ChargeClip;

        [FormerlySerializedAs("stunnedClip")]
        [SerializeField] private AudioClip m_StunnedClip;
        public AudioClip WalkingClip => m_WalkingClip;
        public AudioClip WindupClip => m_WindupClip;
        public AudioClip ChargeClip => m_ChargeClip;
        public AudioClip StunnedClip => m_StunnedClip;

        [Header("Collision Behavior")]
        [FormerlySerializedAs("explosionPrefab")]
        [SerializeField] private GameObject m_ExplosionPrefab;

        [Header("Charging Properties")]
        [FormerlySerializedAs("detectionRadius")]
        [SerializeField] private float m_DetectionRadius = 5f;

        [FormerlySerializedAs("windupDuration")]
        [SerializeField] private float m_WindupDuration = 0.3f;

        [FormerlySerializedAs("speedCurve")]
        [SerializeField] private AnimationCurve m_AnimationSpeedCurve;

        [FormerlySerializedAs("maxSpeed")]
        [SerializeField] private float m_MaxSpeed;

        [FormerlySerializedAs("chargeDuration")]
        [SerializeField] private float m_ChargeDuration;

        [FormerlySerializedAs("stunnedDuration")]
        [SerializeField] private float m_StunnedDuration;
        public float DetectionRadius => m_DetectionRadius;
        public float WindupDuration => m_WindupDuration;
        public AnimationCurve SpeedCurve => m_AnimationSpeedCurve;
        public float MaxSpeed => m_MaxSpeed;
        public float ChargeDuration => m_ChargeDuration;
        public float StunnedDuration => m_StunnedDuration;

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
                SpawnExplosion(SpriteRenderer.bounds.center);
                GetComponent<QuestReporter>()?.Report();
                Destroy(gameObject);
            
                return;
            }

            if (collision.gameObject.TryGetComponent(out Player.Player player))
            {
                player.ApplyDamage(Data.ContactDamage);

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
            if (m_ExplosionPrefab != null)
                Instantiate(m_ExplosionPrefab, position, Quaternion.identity);
        }

        public void ChangeState(VendingMachineState newState)
        {
            CurrentState.Exit(this);
            CurrentState = newState;
            CurrentState.Enter(this);
        }

        protected override void OnProjectileHit()
        {
            SpawnExplosion(SpriteRenderer.bounds.center);
            GetComponent<QuestReporter>()?.Report();
            Destroy(gameObject);
        }
    }
}
