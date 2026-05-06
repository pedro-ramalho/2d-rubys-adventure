using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Game/Player Data")]
public class PlayerData : ScriptableObject
{
    [Header("Health")]
    public int maxHealth = 5;
    public int startingHealth = 3;
    public float invincibilityDuration = 2f;

    [Header("Movement")]
    public float speed = 3f;
    public float acceleration = 20f;
    public float deceleration = 25f;

    [Header("Dash")]
    public float dashSpeed = 15f;
    public float dashDuration = 0.15f;
    public float dashCooldown = 1f;

    [Header("Combat")]
    public float projectileLaunchForce = 300f;

    [Header("Afterimage")]
    public Color afterimageColor = new Color(0.5f, 0.8f, 1f, 0.6f);
    public float afterimageInterval = 0.05f;
    public float afterimageLingerDuration = 0.3f;
}
