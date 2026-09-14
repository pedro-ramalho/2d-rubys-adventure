using UnityEngine;

namespace AdventureGame.Entities.Player
{
    [CreateAssetMenu(fileName = "PlayerData", menuName = "Game/Player Data")]
    public class PlayerData : ScriptableObject
    {
        [Header("Health")]
        public int MaxHealth = 5;
        public int StartingHealth = 3;
        public float InvincibilityDuration = 2f;

        [Header("Movement")]
        public float Speed = 3f;
        public float Acceleration = 20f;
        public float Deceleration = 25f;

        [Header("Dash")]
        public float DashSpeed = 15f;
        public float DashDuration = 0.15f;
        public float DashCooldown = 1f;

        [Header("Combat")]
        public float ProjectileLaunchForce = 300f;
        public float ShootDuration = 0.35f;

        [Header("Afterimage")]
        public Color AfterimageColor = new Color(0.5f, 0.8f, 1f, 0.6f);
        public float AfterimageInterval = 0.05f;
        public float AfterimageLingerDuration = 0.3f;
    }
}
