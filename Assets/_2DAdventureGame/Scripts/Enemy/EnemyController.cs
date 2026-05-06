using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
public class EnemyController : MonoBehaviour
{
    [SerializeField] private float speed = 1.0f;
    [SerializeField] private bool vertical;
    [SerializeField] private float changeTime = 3.0f;
    [SerializeField] private ParticleSystem smokeParticleEffect;

    private Rigidbody2D rb;
    private Animator animator;
    private AudioSource audioSource;

    private float timer;
    private int direction = 1;
    private bool isFixed = false;

    static readonly int MoveXHash = Animator.StringToHash("Move X");
    static readonly int MoveYHash = Animator.StringToHash("Move Y");
    static readonly int FixedHash = Animator.StringToHash("Fixed");

    public event Action OnFixed;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        timer = changeTime;
    }

    void Update()
    {
        if (isFixed) return;

        timer -= Time.deltaTime;
        if (timer < 0)
        {
            direction = -direction;
            timer = changeTime;
        }
    }

    void FixedUpdate()
    {
        if (isFixed) return;

        Vector2 position = rb.position;

        if (vertical)
        {
            position.y += speed * direction * Time.deltaTime;
            animator.SetFloat(MoveXHash, 0);
            animator.SetFloat(MoveYHash, direction);
        }
        else
        {
            position.x += speed * direction * Time.deltaTime;
            animator.SetFloat(MoveXHash, direction);
            animator.SetFloat(MoveYHash, 0);
        }

        rb.MovePosition(position);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Player player = other.GetComponent<Player>();
        if (player == null) return;

        player.TakeDamage(1);
    }

    public void Fix()
    {
        isFixed = true;
        rb.simulated = false;
        animator.SetTrigger(FixedHash);
        audioSource.Stop();
        smokeParticleEffect.Stop();
        OnFixed?.Invoke();
    }
}
