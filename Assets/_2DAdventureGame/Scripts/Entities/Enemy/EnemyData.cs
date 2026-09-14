using UnityEngine;

namespace AdventureGame.Entities.Enemy
{
    [CreateAssetMenu(fileName = "EnemyData", menuName = "Game/Enemy Data")]
    public class EnemyData : ScriptableObject
    {
        [Header("Health")]
        public int maxHealth;

        [Header("Movement")]
        public float speed;

        [Header("Combat")]
        public int contactDamage = 1;
    }
}
