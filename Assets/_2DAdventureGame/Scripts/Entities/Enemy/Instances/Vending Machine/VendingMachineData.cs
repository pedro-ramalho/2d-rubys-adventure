using UnityEngine;

namespace AdventureGame.Entities.Enemy
{
    public class VendingMachineData : EnemyData
    {
        [Header("State Tints")]
        public Color ChargeTint;
        public Color StunnedTint;

        [Header("SFX Clips")]
        public AudioClip WalkClip;
        public AudioClip WindupClip;
        public AudioClip ChargeClip;
        public AudioClip StunnedClip;

        [Header("Explosion Effect")]
        public GameObject ExplosionPrefab;

        [Header("Charging Properties")]
        public float DetectionRadius;
        public AnimationCurve SpeedCurve;
        public float MaxSpeed;
        public float WindupDuration;
        public float ChargeDuration;
        public float StunnedDuration;
    }
}
