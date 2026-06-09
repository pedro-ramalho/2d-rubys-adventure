using System.Collections.Generic;
using UnityEngine;

public class QuestBinding : MonoBehaviour
{
    [SerializeField] private QuestData boundQuest;

    [Tooltip("If true, the SpriteRenderers and Collider2Ds on this GameObject (and its children) are hidden/disabled until the bound quest is accepted. Use for items that should not appear in the world at all before the quest starts.")]
    [SerializeField] private bool activateOnQuestAccept;

    [Tooltip("If true, the Collider2Ds on this GameObject (and its children) start as solid (Is Trigger = false) and switch to triggers (Is Trigger = true) when the bound quest is accepted. Use for items that should physically block the player until they become interactable.")]
    [SerializeField] private bool enableTriggerOnQuestAccept;

    [SerializeField] private bool deactivateOnComplete = true;

    [Tooltip("Components disabled until the bound quest is active (e.g. the Collectible script). Use for items that remain visible but should not yet be interactable.")]
    [SerializeField] private Behaviour[] interactables;

    public QuestData BoundQuest => boundQuest;

    private readonly List<SpriteRenderer> renderers = new();
    private readonly List<Collider2D> colliders = new();

    void Awake()
    {
        if (activateOnQuestAccept || enableTriggerOnQuestAccept)
            GetComponentsInChildren(true, colliders);

        if (activateOnQuestAccept)
            GetComponentsInChildren(true, renderers);

        SetInteractable(false);
    }

    void Start()
    {
        if (QuestManager.Instance == null) return;

        QuestManager.Instance.OnQuestAccepted += HandleAccepted;
        QuestManager.Instance.OnQuestCompleted += HandleCompleted;

        bool interactable = QuestManager.Instance.ActiveQuest?.Data == boundQuest;
        SetInteractable(interactable);
    }

    void OnDestroy()
    {
        if (QuestManager.Instance == null) return;

        QuestManager.Instance.OnQuestAccepted -= HandleAccepted;
        QuestManager.Instance.OnQuestCompleted -= HandleCompleted;
    }

    void HandleAccepted(Quest quest)
    {
        if (quest.Data == boundQuest) SetInteractable(true);
    }

    void HandleCompleted(Quest quest)
    {
        if (quest.Data == boundQuest && deactivateOnComplete) SetInteractable(false);
    }

    void SetInteractable(bool value)
    {
        foreach (Behaviour b in interactables)
            if (b != null) b.enabled = value;

        if (activateOnQuestAccept)
        {
            foreach (SpriteRenderer r in renderers)
                if (r != null) r.enabled = value;
            foreach (Collider2D c in colliders)
                if (c != null) c.enabled = value;
        }

        if (enableTriggerOnQuestAccept)
        {
            foreach (Collider2D c in colliders)
                if (c != null) c.isTrigger = value;
        }
    }
}
