using UnityEngine;
using UnityEngine.Serialization;

namespace AdventureGame.Entities.Enemy
{
    [CreateAssetMenu(fileName = "EnemyData", menuName = "Game/Enemy Data")]
    public class EnemyData : ScriptableObject
    {
        [Header("Health")]
        [FormerlySerializedAs("maxHealth")]
        public int MaxHealth;

        [Header("Movement")]
        [FormerlySerializedAs("speed")]
        public float Speed;

        [Header("Combat")]
        [FormerlySerializedAs("contactDamage")]
        public int ContactDamage = 1;
    }
}
