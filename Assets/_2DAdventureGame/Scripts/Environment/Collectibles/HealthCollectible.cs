using AdventureGame.Entities.Player;
using UnityEngine;

namespace AdventureGame.Environment.Collectibles
{
    public class HealthCollectible : Collectible
    {
        [SerializeField] private int healAmount;

        protected override void ApplyEffect(Player player)
        {
            if (player.CurrentHealth < player.Data.maxHealth) 
                player.Heal(healAmount);
        }
    }
}