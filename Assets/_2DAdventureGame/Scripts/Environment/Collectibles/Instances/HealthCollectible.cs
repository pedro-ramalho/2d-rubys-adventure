using UnityEngine;

public class HealthCollectible : Collectible
{
    [SerializeField] private string reportTag;
    [SerializeField] private int healAmount;

    public override void ApplyEffect(Player player)
    {
        if (player.CurrentHealth < player.Data.maxHealth) player.Heal(healAmount);
        
        player.GetComponent<AudioSource>()?.PlayOneShot(collectibleClip);
    }
}