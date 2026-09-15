using UnityEngine;

namespace AdventureGame.Entities.Enemy
{
    [CreateAssetMenu(fileName = "PatrolRobotData", menuName = "Game/Patrol Robot Data")]
    public class PatrolRobotData : EnemyData
    {
        [Header("Fixed Properties")]
        public GameObject FixedEffectPrefab;
        public AudioClip FixedClip;

        [Header("Hit Properties")]
        public AudioClip HitClip;
    }
}
