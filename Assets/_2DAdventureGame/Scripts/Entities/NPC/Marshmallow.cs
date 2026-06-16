using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class Marshmallow : NPC
{
    [SerializeField] private QuestData boundQuest;
    [SerializeField] private Transform exitPoint;
    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private Vector2 idleFacing = Vector2.down;

    private Animator animator;
    private Rigidbody2D rb;
    private Collider2D[] colliders;
    private Vector2 startPosition;
    private QuestController controller;

    void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        colliders = GetComponents<Collider2D>();
        startPosition = transform.position;
        SetFacing(idleFacing);
    }

    void Start()
    {
        if (QuestManager.Instance == null) return;
        controller = QuestManager.Instance.Get(boundQuest);
        if (controller == null) return;

        controller.OnPhaseChanged += HandlePhaseChanged;
        controller.OnEpilogueFinished += HandleEpilogueFinished;
    }

    void OnDestroy()
    {
        if (controller != null)
        {
            controller.OnPhaseChanged -= HandlePhaseChanged;
            controller.OnEpilogueFinished -= HandleEpilogueFinished;
        }
    }

    void HandlePhaseChanged(QuestController c)
    {
        if (c.Phase == QuestPhase.During) WalkToExit();
        else if (c.Phase == QuestPhase.After) WalkBack();
    }

    void HandleEpilogueFinished(QuestController c) => SetCollidersEnabled(false);

    public void WalkToExit()
    {
        if (exitPoint != null) StartCoroutine(WalkTo(exitPoint.position));
    }

    public void WalkBack() => StartCoroutine(WalkTo(startPosition));

    IEnumerator WalkTo(Vector2 target)
    {
        SetCollidersEnabled(false);

        Vector2 direction = (target - rb.position).normalized;
        SetFacing(direction);
        animator.SetFloat(AnimatorHashes.Speed, 1f);

        WaitForFixedUpdate wait = new();
        while (Vector2.Distance(rb.position, target) > 0.01f)
        {
            Vector2 next = Vector2.MoveTowards(rb.position, target, walkSpeed * Time.fixedDeltaTime);
            rb.MovePosition(next);

            yield return wait;
        }

        SetFacing(idleFacing);
        animator.SetFloat(AnimatorHashes.Speed, 0f);

        SetCollidersEnabled(true);
    }

    void SetFacing(Vector2 direction)
    {
        animator.SetFloat(AnimatorHashes.LookX, direction.x);
        animator.SetFloat(AnimatorHashes.LookY, direction.y);
    }

    void SetCollidersEnabled(bool value)
    {
        foreach (Collider2D collider in colliders)
            collider.enabled = value;
    }
}
