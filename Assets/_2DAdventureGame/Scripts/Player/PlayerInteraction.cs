using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private InputAction talkAction;

    private PlayerMovement playerMovement;
    private NPC lastNPC;

    void Start()
    {
        talkAction.Enable();
        playerMovement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position + Vector2.up * 0.2f, playerMovement.MoveDirection, 1.5f, LayerMask.GetMask("NPC"));
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
            UIHandler.Instance.DisplayDialogue();
    }
}