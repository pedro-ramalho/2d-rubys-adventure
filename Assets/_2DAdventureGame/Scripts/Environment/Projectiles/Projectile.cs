using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    [SerializeField] private float maxLifetime = 3f;
    [SerializeField] private GameObject hitEffectPrefab;

    private Rigidbody2D rb;

    void Awake() => rb = GetComponent<Rigidbody2D>();

    public void Launch(Vector2 direction, float force)
    {
        rb.AddForce(direction * force);
        Destroy(gameObject, maxLifetime);
    }

    void OnTriggerEnter2D(Collider2D other) => HandleImpact();

    void OnCollisionEnter2D(Collision2D collision) => HandleImpact();

    void HandleImpact()
    {
        if (hitEffectPrefab != null)
            Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
        
        Destroy(gameObject);
    }
}
