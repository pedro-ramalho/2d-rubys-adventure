using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Game/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("Health")]
    public int maxHealth;

    [Header("Movement")]
    public float speed;

    [Header("Quest Report Tag")]
    public string reportTag;

    [Header("Vending Machine Properties")]
    public float detectionRadius = 5f;
    public AnimationCurve speedCurve;
    public float maxSpeed;
    public float chargeDuration;
    public float stunnedDuration;
}
