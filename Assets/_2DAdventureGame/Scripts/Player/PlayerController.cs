using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private InputAction launchAction;
    [SerializeField] private InputAction talkAction;

    private Rigidbody2D rb;
    private Animator animator;
    private AudioSource audioSource;
    private PlayerMovement playerMovement;

    [SerializeField] private GameObject projectile;
    [SerializeField] private AudioClip throwProjectileClip;

    private NPC lastNPC;

    void Start()
    {
        launchAction.Enable();
        talkAction.Enable();

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
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
            TryTalkToNPC();
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

    void Launch()
    {
        GameObject projectileObject = Instantiate(projectile, rb.position + Vector2.up * 0.5f, Quaternion.identity);
        Projectile p = projectileObject.GetComponent<Projectile>();
        p.Launch(playerMovement.MoveDirection, 300);
        animator.SetTrigger("Launch");
        PlaySound(throwProjectileClip);
    }

    void TryTalkToNPC()
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