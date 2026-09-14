using UnityEngine;
using UnityEngine.Serialization;

namespace AdventureGame.Environment.Projectiles
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Projectile : MonoBehaviour
    {
        [FormerlySerializedAs("maxLifetime")]
        [SerializeField] private float m_MaxLifetime = 3f;

        [FormerlySerializedAs("hitEffectPrefab")]
        [SerializeField] private GameObject m_HitEffectPrefab;

        private Rigidbody2D m_Rigidbody;

        void Awake() => m_Rigidbody = GetComponent<Rigidbody2D>();

        public void Launch(Vector2 direction, float force)
        {
            m_Rigidbody.AddForce(direction * force);
            Destroy(gameObject, m_MaxLifetime);
        }

        void OnTriggerEnter2D(Collider2D other) => HandleImpact();

        void OnCollisionEnter2D(Collision2D collision) => HandleImpact();

        void HandleImpact()
        {
            if (m_HitEffectPrefab != null)
                Instantiate(m_HitEffectPrefab, transform.position, Quaternion.identity);
        
            Destroy(gameObject);
        }
    }
}
