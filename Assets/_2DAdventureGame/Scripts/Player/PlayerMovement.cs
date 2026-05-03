using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(PlayerHealth))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private InputAction moveAction;
    [SerializeField] private InputAction dashAction;

    [SerializeField] private float speed = 3.0f;
    [SerializeField] private float acceleration = 20.0f;
    [SerializeField] private float deceleration = 25.0f;
    [SerializeField] private float dashSpeed = 15.0f;
    [SerializeField] private float dashDuration = 0.15f;
    [SerializeField] private float dashCooldown = 1.0f;

    private Rigidbody2D rb;
    private Animator animator;
    private AudioSource audioSource;
    private PlayerHealth playerHealth;

    [SerializeField] private AudioClip playerWalkClip;
    [SerializeField] private AudioClip dashClip;

    private Vector2 move;
    private Vector2 currentVelocity;
    private Vector2 moveDirection = new(1, 0);

    private bool isDashing;
    private float dashTimer;
    private float dashCooldownTimer;

    static readonly int LookXHash = Animator.StringToHash("Look X");
    static readonly int LookYHash = Animator.StringToHash("Look Y");
    static readonly int SpeedHash = Animator.StringToHash("Speed");

    public Vector2 MoveDirection => moveDirection;
    public bool IsDashing => isDashing;

    [SerializeField] private GameObject afterimage;
    [SerializeField] private float afterimageInterval = 0.05f;
    [SerializeField] private float afterimageDuration = 0.3f;
    [SerializeField] private Color afterimageColor = new Color(0.5f, 0.8f, 1f, 0.6f);

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        moveAction.Enable();
        dashAction.Enable();

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        playerHealth = GetComponent<PlayerHealth>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (dashTimer > 0)
            dashTimer -= Time.deltaTime;
        else
            isDashing = false;

        if (dashCooldownTimer > 0)
            dashCooldownTimer -= Time.deltaTime;

        move = moveAction.ReadValue<Vector2>();

        if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            moveDirection.Set(move.x, move.y);
            moveDirection.Normalize();
        }

        animator.SetFloat(LookXHash, moveDirection.x);
        animator.SetFloat(LookYHash, moveDirection.y);
        animator.SetFloat(SpeedHash, isDashing ? 1f : move.magnitude);

        HandleWalkAudio();

        if (dashAction.WasPressedThisFrame() && dashCooldownTimer <= 0 && !isDashing)
            StartDash();
    }

    void FixedUpdate()
    {
        if (isDashing)
        {
            rb.MovePosition(rb.position + moveDirection * (dashSpeed * Time.fixedDeltaTime));
            return;
        }

        Vector2 targetVelocity = move * speed;
        float rate = move.magnitude > 0 ? acceleration : deceleration;
        currentVelocity = Vector2.MoveTowards(currentVelocity, targetVelocity, rate * Time.fixedDeltaTime);
        rb.MovePosition(rb.position + currentVelocity * Time.fixedDeltaTime);
    }

    void StartDash()
    {
        isDashing = true;
        dashTimer = dashDuration;
        dashCooldownTimer = dashCooldown;
        currentVelocity = Vector2.zero;

        playerHealth.SetInvincible(dashDuration);

        if (dashClip != null)
        {
            audioSource.Stop();
            audioSource.PlayOneShot(dashClip);
        }

        StartCoroutine(SpawnAfterimages());
    }

    IEnumerator SpawnAfterimages()
    {
        while (isDashing)
        {
            GameObject ghost = Instantiate(afterimage, transform.position, transform.rotation);
            ghost.GetComponent<DashAfterimage>().Initialize(
                spriteRenderer.sprite,
                transform.localScale,
                spriteRenderer.flipX,
                afterimageColor,
                afterimageDuration
            );

            yield return new WaitForSeconds(afterimageInterval);
        }
    }

    void HandleWalkAudio()
    {
        if (isDashing) return;

        if (move.magnitude > 0)
        {
            if (audioSource.clip != playerWalkClip || !audioSource.isPlaying)
            {
                audioSource.clip = playerWalkClip;
                audioSource.Play();
            }
        }
        else if (audioSource.clip == playerWalkClip && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
}
