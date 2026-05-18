using UnityEngine;

public class QuestBinding : MonoBehaviour
{
    [SerializeField] private QuestData boundQuest;
    [SerializeField] private bool deactivateOnComplete = true;

    [Tooltip("Components disabled until the bound quest is active (e.g. the trigger Collider2D and/or Collectible script). Visual components like SpriteRenderer should NOT be included — items stay visible at all times.")]
    [SerializeField] private Behaviour[] interactables;

    public QuestData BoundQuest => boundQuest;

    void Awake() => SetInteractable(false);

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
    }
}
