using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // Actions
    public InputAction moveAction;
    public InputAction launchAction;
    public InputAction talkAction;

    private Rigidbody2D rb;
    private Vector2 move;

    // Player health
    public int maxHealth = 5;
    public int health { get { return currentHealth; } }
    int currentHealth;

    // Player speed
    public float speed = 3.0f;

    // Invicibility
    public float timeInvicible = 2.0f;
    bool isInvicible;
    float damageCooldown;

    // Projectile
    public GameObject projectile;

    // Animation
    private Animator animator;
    Vector2 moveDirection = new Vector2(1, 0);

    // NPC
    private NPC lastNPC;

    // Audio
    private AudioSource audioSource;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moveAction.Enable();
        launchAction.Enable();
        talkAction.Enable();

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;

        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        move = moveAction.ReadValue<Vector2>();

        if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            moveDirection.Set(move.x, move.y);
            moveDirection.Normalize();
        }

        animator.SetFloat("Look X", moveDirection.x);
        animator.SetFloat("Look Y", moveDirection.y);
        animator.SetFloat("Speed", move.magnitude);

        if (isInvicible)
        {
            damageCooldown -= Time.deltaTime;
            if (damageCooldown < 0)
            {
                isInvicible = false;
            }
        }

        if (launchAction.WasPressedThisFrame())
        {
            Launch();
        }

        RaycastHit2D hit = Physics2D.Raycast(rb.position + Vector2.up * 0.2f, moveDirection, 1.5f, LayerMask.GetMask("NPC"));
        if (hit.collider != null)
        {
            NPC npc = hit.collider.GetComponent<NPC>();
            npc.dialogueBubble.SetActive(true);
            lastNPC = npc;
            FindFriend();
        }
        else
        {
            if (lastNPC != null)
            {
                lastNPC.dialogueBubble.SetActive(false);
                lastNPC = null;
            }
        }
    }

    void FixedUpdate()
    {
        Vector2 position = rb.position + move * speed * Time.deltaTime;
        rb.MovePosition(position);
    }

    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            if (isInvicible) return;

            isInvicible = true;
            damageCooldown = timeInvicible;

            animator.SetTrigger("Hit");
        }

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        UIHandler.instance.SetHealthValue(currentHealth / (float)maxHealth);
    }

    void Launch()
    {
        GameObject projectileObject = Instantiate(projectile, rb.position + Vector2.up * 0.5f, Quaternion.identity);
        Projectile p = projectileObject.GetComponent<Projectile>();
        p.Launch(moveDirection, 300);
        animator.SetTrigger("Launch");
    }

    void FindFriend()
    {
        if (talkAction.WasPressedThisFrame())
        {
            UIHandler.instance.DisplayDialogue();
        }
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
}
