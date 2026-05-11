using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
public abstract class Enemy : MonoBehaviour
{
    [Header("Enemy Data")]
    [SerializeField] private EnemyData data;
    public EnemyData Data => data;

    // Components
    public Rigidbody2D Rigidbody { get; private set; }
    public Animator Animator { get; private set; }
    public AudioSource AudioSource { get; private set; }

    public event Action OnFixed;

    protected virtual void Awake()
    {
        Rigidbody = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();
        AudioSource = GetComponent<AudioSource>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Projectile"))
            OnProjectileHit();
    }

    protected abstract void OnProjectileHit();

    public void RaiseOnFixed() => OnFixed?.Invoke();
}
