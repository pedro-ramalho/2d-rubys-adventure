using System.Collections.Generic;
using UnityEngine;

public class QuestBinding : MonoBehaviour
{
    [SerializeField] private QuestData boundQuest;

    [SerializeField] private bool activateOnQuestAccept;

    [SerializeField] private bool enableTriggerOnQuestAccept;

    [SerializeField] private bool deactivateOnComplete = true;

    [SerializeField] private Behaviour[] interactables;

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
        if (QuestManager.Instance == null) 
            return;

        controller = QuestManager.Instance.Get(boundQuest);
        if (controller == null) 
            return;

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
                if (r != null) 
                    r.enabled = value;

            foreach (Collider2D c in colliders)
                if (c != null) 
                    c.enabled = value;
        }

        if (enableTriggerOnQuestAccept)
        {
            foreach (Collider2D c in colliders)
                if (c != null) 
                    c.isTrigger = value;
        }
    }
}
