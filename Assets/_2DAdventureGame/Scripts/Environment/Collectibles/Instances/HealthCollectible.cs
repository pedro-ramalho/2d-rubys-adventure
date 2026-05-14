using UnityEngine;

public class HealthCollectible : Collectible
{
    [SerializeField] private string reportTag;
    [SerializeField] private int healAmount;

    public override void ApplyEffect(Collider2D other)
    {
        if (other.TryGetComponent(out Player player))
        {
            if (player.CurrentHealth < player.Data.maxHealth)
                player.Heal(healAmount);
            
            player.GetComponent<AudioSource>()?.PlayOneShot(collectibleClip);

            GetComponent<QuestReporter>()?.Report();
        }
    }

}