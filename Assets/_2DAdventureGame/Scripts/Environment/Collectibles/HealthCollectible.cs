using UnityEngine;

public class HealthCollectible : MonoBehaviour
{
    [SerializeField] private string reportTag;
    [SerializeField] private AudioClip collectedClip;

    void OnTriggerEnter2D(Collider2D other)
    {
        Player player = other.GetComponent<Player>();
        if (player == null) return;

        if (player.CurrentHealth < player.Data.maxHealth)
        {
            player.Heal(1);
            other.GetComponent<AudioSource>().PlayOneShot(collectedClip);
            QuestManager.Instance?.Report(new QuestReport(QuestObjectiveType.Fetch, reportTag));
            Destroy(gameObject);
        }
    }
}