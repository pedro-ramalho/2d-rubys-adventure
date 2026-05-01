using System;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;
    public float speed = 1.0f;

    public bool vertical;

    public float changeTime = 3.0f;
    float timer;
    int direction = 1;

    bool broken = true;

    public bool isBroken { get { return broken; } }
    public event Action OnFixed;

    AudioSource audioSource;
    public ParticleSystem smokeParticleEffect;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        timer = changeTime;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer < 0)
        {
            direction = -direction;
            timer = changeTime;
        }
    }

    void FixedUpdate()
    {
        if (!broken) return;

        Vector2 position = rb.position;

        if (vertical)
        {
            position.y += speed * direction * Time.deltaTime;
            animator.SetFloat("Move X", 0);
            animator.SetFloat("Move Y", direction);
        }
        else
        {
            position.x += speed * direction * Time.deltaTime;
            animator.SetFloat("Move X", direction);
            animator.SetFloat("Move Y", 0);
        }

        rb.MovePosition(position);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth == null) return;

        playerHealth.ChangeHealth(-1);
    }

    public void Fix()
    {
        broken = false;
        rb.simulated = false;
        animator.SetTrigger("Fixed");
        audioSource.Stop();
        smokeParticleEffect.Stop();
        OnFixed?.Invoke();
    }
}
