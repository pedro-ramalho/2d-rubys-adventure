using AdventureGame.Entities.Player;
using UnityEngine;
using UnityEngine.Serialization;

namespace AdventureGame.Environment.Collectibles
{
    public class HealthCollectible : Collectible
    {
        [FormerlySerializedAs("healAmount")]
        [SerializeField] private int m_HealAmount;

        protected override void ApplyEffect(Player player)
        {
            if (player.CurrentHealth < player.Data.MaxHealth) 
                player.Heal(m_HealAmount);
        }
    }
}