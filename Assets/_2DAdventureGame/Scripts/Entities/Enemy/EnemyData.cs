using UnityEngine;
using UnityEngine.Serialization;

namespace AdventureGame.Entities.Enemy
{
    [CreateAssetMenu(fileName = "EnemyData", menuName = "Game/Enemy Data")]
    public class EnemyData : ScriptableObject
    {
        [Header("Health")]
        public int MaxHealth;

        [Header("Movement")]
        public float Speed;

        [Header("Combat")]
        public int ContactDamage = 1;
    }
}
