using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    [SerializeField] private float maxLifetime = 3f;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Launch(Vector2 direction, float force)
    {
        rb.AddForce(direction * force);
        Destroy(gameObject, maxLifetime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out EnemyController enemy)) enemy.Fix();

        Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
    }
}
