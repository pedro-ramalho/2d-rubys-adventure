using UnityEngine;

public enum PatrolDirection { Horizontal, Vertical }

[CreateAssetMenu(fileName = "EnemyData", menuName = "Game/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("Health")]
    public int maxHealth;

    [Header("Movement")]
    public float speed;

    [Header("Quest Report Tag")]
    public string reportTag;

    [Header("Patrolling Properties")] 
    public PatrolDirection patrolDirection;
    public float patrolDuration;

    [Header("Vending Machine Properties")]
    public AnimationCurve speedCurve;
    public float maxSpeed;
    public float chargeDuration;
    public float stunnedDuration;
}
