using System;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 5;
    [SerializeField] private int startingHealth = 3;
    [SerializeField] private float invincibilityDuration = 2.0f;
    [SerializeField] private AudioClip playerHitClip;

    private Animator animator;
    private AudioSource audioSource;

    private int currentHealth;
    private bool isInvincible;
    private float damageCooldown;

    public int Health => currentHealth;
    public int MaxHealth => maxHealth;

    public event Action<float> OnHealthChanged;
    public event Action OnDied;

    void Awake()
    {
        currentHealth = Mathf.Clamp(startingHealth, 0, maxHealth);
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (isInvincible)
        {
            damageCooldown -= Time.deltaTime;
            if (damageCooldown < 0) isInvincible = false;
        }
    }

    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            if (isInvincible) return;

            isInvincible = true;
            damageCooldown = invincibilityDuration;
            animator.SetTrigger("Hit");
            audioSource.PlayOneShot(playerHitClip);
        }

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        OnHealthChanged?.Invoke(currentHealth / (float)maxHealth);
        if (currentHealth == 0) OnDied?.Invoke();
    }
}