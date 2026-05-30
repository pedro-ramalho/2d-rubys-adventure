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

    private const string CompletionMessage = "Quest complete! Return and speak with the NPC.";

    void Awake() => inputActions = new PlayerInputActions();

    void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Tracker.performed += OnTrackerPressed;
    }

    void OnDisable()
    {
        inputActions.Player.Tracker.performed -= OnTrackerPressed;
        inputActions.Player.Disable();
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

        if (descriptionLabel != null)
            descriptionLabel.text = quest.Data.description;

        if (progressLabel != null)
            progressLabel.text = $"{quest.Count} / {quest.Data.objective.count}";

        SetVisible(true);
    }

    void SetVisible(bool visible)
    {
        if (trackerRoot != null)
            trackerRoot.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
    }
}
