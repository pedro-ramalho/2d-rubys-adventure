using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private InputAction talkAction;

    private Rigidbody2D rb;
    private PlayerMovement playerMovement;
    private NPC lastNPC;

    void Start()
    {
        talkAction.Enable();

        rb = GetComponent<Rigidbody2D>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
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

    void TryTalkToNPC()
    {
        if (talkAction.WasPressedThisFrame())
            UIHandler.instance.DisplayDialogue();
    }
}