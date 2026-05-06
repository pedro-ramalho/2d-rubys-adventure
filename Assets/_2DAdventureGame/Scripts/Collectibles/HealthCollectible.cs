using UnityEngine;

public class HealthCollectible : MonoBehaviour
{
    [SerializeField] private AudioClip collectedClip;

    void OnTriggerEnter2D(Collider2D other)
    {
        Player player = other.GetComponent<Player>();
        if (player == null) return;

        if (player.CurrentHealth < player.Data.maxHealth)
        {
            player.TakeDamage(-1);
            other.GetComponent<AudioSource>().PlayOneShot(collectedClip);
            Destroy(gameObject);
        }
    }
}