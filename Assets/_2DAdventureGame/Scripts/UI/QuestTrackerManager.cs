using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class QuestTrackerManager : MonoBehaviour
{
    [SerializeField] private UIDocument trackerDocument;

    private PlayerInputActions inputActions;
    private VisualElement trackerRoot;
    private Label descriptionLabel;
    private Label progressLabel;

    private bool isOpen;
    private QuestData completionPendingQuest;

    private int lastCount = -1;
    private QuestData lastQuest;

    private const string CompletionMessage = "Quest complete! Return and speak with the NPC.";

    void Awake() => inputActions = InputManager.Instance.Actions;

    void OnEnable()
    {
        inputActions.Player.Tracker.performed += OnTrackerPressed;
    }

    void OnDisable()
    {
        inputActions.Player.Tracker.performed -= OnTrackerPressed;
    }


    void Start()
    {
        VisualElement root = trackerDocument.rootVisualElement;
        trackerRoot = root.Q<VisualElement>("TrackerRoot");
        descriptionLabel = root.Q<Label>("Description");
        progressLabel = root.Q<Label>("Progress");

        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.OnQuestCompleted += HandleQuestCompleted;
            QuestManager.Instance.OnQuestConcluded += HandleQuestConcluded;
        }

        SetVisible(false);
    }

    void OnDestroy()
    {
        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.OnQuestCompleted -= HandleQuestCompleted;
            QuestManager.Instance.OnQuestConcluded -= HandleQuestConcluded;
        }
    }

    void HandleQuestCompleted(Quest quest)
    {
        completionPendingQuest = quest.Data;
        Refresh();
    }

    void HandleQuestConcluded(QuestData data)
    {
        if (data != completionPendingQuest) return;
        completionPendingQuest = null;
        isOpen = false;
        Refresh();
    }

    void Update()
    {
        if (isOpen) Refresh();
    }

    void OnTrackerPressed(InputAction.CallbackContext ctx)
    {
        if (PauseManager.IsPaused) return;
        if (completionPendingQuest != null) return;

        isOpen = !isOpen;
        Refresh();
    }

    void Refresh()
    {
        if (completionPendingQuest != null)
        {
            if (descriptionLabel != null) descriptionLabel.text = CompletionMessage;
            if (progressLabel != null) progressLabel.text = string.Empty;
            SetVisible(true);
            return;
        }

        Quest quest = QuestManager.Instance?.ActiveQuest;
        if (!isOpen || quest == null)
        {
            SetVisible(false);
            return;
        }

        if (descriptionLabel != null && quest.Data != lastQuest)
        {
            descriptionLabel.text = quest.Data.description;
            lastQuest = quest.Data;
        }

        if (progressLabel != null && quest.Count != lastCount)
        {
            progressLabel.text = $"{quest.Count} / {quest.Data.objective.count}";
            lastCount = quest.Count;
        }

        SetVisible(true);
    }

    void SetVisible(bool visible)
    {
        if (trackerRoot != null)
            trackerRoot.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
    }
}
