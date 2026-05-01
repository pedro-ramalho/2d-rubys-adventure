using UnityEngine;

public class HealthCollectible : MonoBehaviour
{
    [SerializeField] private AudioClip collectedClip;

    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth == null) return;

        if (playerHealth.Health < playerHealth.MaxHealth)
        {
            playerHealth.ChangeHealth(1);
            other.GetComponent<AudioSource>().PlayOneShot(collectedClip);
            Destroy(gameObject);
        }
    }
}