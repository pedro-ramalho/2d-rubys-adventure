using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class Marshmallow : MonoBehaviour
{
    [SerializeField] private QuestData boundQuest;
    [SerializeField] private Transform exitPoint;
    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private Vector2 idleFacing = Vector2.down;

    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int LookXHash = Animator.StringToHash("Look X");
    private static readonly int LookYHash = Animator.StringToHash("Look Y");

    private Animator animator;
    private Rigidbody2D rb;
    private Vector2 startPosition;

    void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        startPosition = transform.position;
        SetFacing(idleFacing);
    }

    void OnEnable()
    {
        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.OnQuestAccepted += HandleQuestAccepted;
            QuestManager.Instance.OnQuestEpilogueFinished += HandleQuestEpilogueFinished;
        }
    }

    void OnDisable()
    {
        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.OnQuestAccepted -= HandleQuestAccepted;
            QuestManager.Instance.OnQuestEpilogueFinished -= HandleQuestEpilogueFinished;
        }
    }

    void HandleQuestAccepted(Quest quest)
    {
        if (quest.Data == boundQuest) WalkOffscreen();
    }

    void HandleQuestEpilogueFinished(QuestData data)
    {
        if (data == boundQuest) SetCollidersEnabled(false);
    }

    public void WalkOffscreen()
    {
        if (exitPoint != null) StartCoroutine(WalkTo(exitPoint.position));
    }

    public void WalkBack() => StartCoroutine(WalkTo(startPosition));

    IEnumerator WalkTo(Vector2 target)
    {
        SetCollidersEnabled(false);

        Vector2 direction = (target - rb.position).normalized;
        SetFacing(direction);
        animator.SetFloat(SpeedHash, 1f);

        WaitForFixedUpdate wait = new();
        while (Vector2.Distance(rb.position, target) > 0.01f)
        {
            Vector2 next = Vector2.MoveTowards(rb.position, target, walkSpeed * Time.fixedDeltaTime);
            rb.MovePosition(next);
            
            yield return wait;
        }

        SetFacing(idleFacing);
        animator.SetFloat(SpeedHash, 0f);

        SetCollidersEnabled(true);
    }

    void SetFacing(Vector2 direction)
    {
        animator.SetFloat(LookXHash, direction.x);
        animator.SetFloat(LookYHash, direction.y);
    }

    void SetCollidersEnabled(bool value)
    {
        foreach (Collider2D collider in GetComponents<Collider2D>())
            collider.enabled = value;
    }
}
