using AdventureGame.Environment.Projectiles;
using UnityEngine;

namespace AdventureGame.Entities.Enemy
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(AudioSource))]
    public abstract class Enemy : MonoBehaviour
    {
        [Header("Enemy Data")]
        [SerializeField]
        private EnemyData m_EnemyData;
        public EnemyData Data => m_EnemyData;

        // Components
        public Rigidbody2D Rigidbody { get; private set; }
        public Animator Animator { get; private set; }
        public AudioSource AudioSource { get; private set; }

        protected virtual void Awake()
        {
            Rigidbody = GetComponent<Rigidbody2D>();
            Animator = GetComponent<Animator>();
            AudioSource = GetComponent<AudioSource>();
        }

        protected virtual void OnEnable()
        {
            if (Player.Player.Instance != null)
                Player.Player.Instance.OnDied += HandlePlayerDied;
        }

        protected virtual void OnDisable()
        {
            if (Player.Player.Instance != null)
                Player.Player.Instance.OnDied -= HandlePlayerDied;
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out Projectile _))
                OnProjectileHit();
        }

        void HandlePlayerDied()
        {
            if (AudioSource != null)
                AudioSource.Stop();
            if (Animator != null)
                Animator.enabled = false;
            if (Rigidbody != null)
                Rigidbody.simulated = false;
            enabled = false;
        }

        protected abstract void OnProjectileHit();
    }
}
