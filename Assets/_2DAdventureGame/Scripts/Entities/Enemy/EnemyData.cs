using UnityEngine;

public enum PatrolDirection { Horizontal, Vertical }

[CreateAssetMenu(fileName = "EnemyData", menuName = "Game/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("Health")]
    public int maxHealth;

    [Header("Movement")]
    public float speed;

    [Header("Patrol Direction")] 
    public PatrolDirection patrolDirection;   
}
