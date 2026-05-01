using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerController : MonoBehaviour
{
    public InputAction launchAction;
    public InputAction talkAction;

    private Rigidbody2D rb;
    private Animator animator;
    private AudioSource audioSource;
    private PlayerMovement playerMovement;

    // Player health
    public int maxHealth = 5;
    public int health { get { return currentHealth; } }
    int currentHealth;

    // Invicibility
    public float timeInvicible = 2.0f;
    bool isInvicible;
    float damageCooldown;

    // Projectile
    public GameObject projectile;

    // NPC
    private NPC lastNPC;

    // Audio
    public AudioClip throwProjectileClip;
    public AudioClip playerHitClip;

    void Start()
    {
        launchAction.Enable();
        talkAction.Enable();

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        playerMovement = GetComponent<PlayerMovement>();
        currentHealth = maxHealth;
    }

    void Update()
    {
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

        RaycastHit2D hit = Physics2D.Raycast(rb.position + Vector2.up * 0.2f, playerMovement.MoveDirection, 1.5f, LayerMask.GetMask("NPC"));
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

    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            if (isInvicible) return;

            isInvicible = true;
            damageCooldown = timeInvicible;

            animator.SetTrigger("Hit");
            PlaySound(playerHitClip);
        }

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        UIHandler.instance.SetHealthValue(currentHealth / (float)maxHealth);
    }

    void Launch()
    {
        GameObject projectileObject = Instantiate(projectile, rb.position + Vector2.up * 0.5f, Quaternion.identity);
        Projectile p = projectileObject.GetComponent<Projectile>();
        p.Launch(playerMovement.MoveDirection, 300);
        animator.SetTrigger("Launch");
        PlaySound(throwProjectileClip);
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