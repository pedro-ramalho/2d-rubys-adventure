using System.Collections.Generic;
using UnityEngine;

public class QuestBinding : MonoBehaviour
{
    [SerializeField] private QuestData boundQuest;

    [Tooltip("If true, the SpriteRenderers and Collider2Ds on this GameObject (and its children) are hidden/disabled until the bound quest enters During. Use for items that should not appear in the world at all before the quest starts (e.g. the Kiwi).")]
    [SerializeField] private bool activateOnQuestAccept;

    [Tooltip("If true, the Collider2Ds on this GameObject (and its children) start as solid (Is Trigger = false) and switch to triggers (Is Trigger = true) when the bound quest enters During.")]
    [SerializeField] private bool enableTriggerOnQuestAccept;

    [SerializeField] private bool deactivateOnComplete = true;

    [Tooltip("Components disabled while the bound quest is not in During (e.g. the Collectible script).")]
    [SerializeField] private Behaviour[] interactables;

    public QuestData BoundQuest => boundQuest;

    private readonly List<SpriteRenderer> renderers = new();
    private readonly List<Collider2D> colliders = new();
    private QuestController controller;

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

        controller = QuestManager.Instance.Get(boundQuest);
        if (controller == null) return;

        QuestReporter reporter = GetComponent<QuestReporter>();
        if (reporter != null && reporter.IsConsumed())
        {
            gameObject.SetActive(false);
            return;
        }

        controller.OnPhaseChanged += HandlePhaseChanged;
        SetInteractable(controller.Phase == QuestPhase.During);
    }

    void OnDestroy()
    {
        if (controller != null)
            controller.OnPhaseChanged -= HandlePhaseChanged;
    }

    void HandlePhaseChanged(QuestController c)
    {
        if (c.Phase == QuestPhase.During)
        {
            SetInteractable(true);
        }
        else if (c.Phase == QuestPhase.After && deactivateOnComplete)
        {
            SetInteractable(false);
        }
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
