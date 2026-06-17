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
    private QuestController completionPending;

    private int lastCount = -1;
    private QuestController lastTracked;

    private const string CompletionMessage = "Quest complete! Return and speak with the NPC.";

    void Awake() => inputActions = InputManager.Instance.Actions;

    void OnEnable() => inputActions.Player.Tracker.performed += OnTrackerPressed;

    void OnDisable() => inputActions.Player.Tracker.performed -= OnTrackerPressed;

    void Start()
    {
        VisualElement root = trackerDocument.rootVisualElement;
        trackerRoot = root.Q<VisualElement>("TrackerRoot");
        descriptionLabel = root.Q<Label>("Description");
        progressLabel = root.Q<Label>("Progress");

        if (QuestManager.Instance != null)
            foreach (QuestController c in QuestManager.Instance.All)
            {
                c.OnPhaseChanged += HandlePhaseChanged;
                c.OnConcluded += HandleConcluded;
            }

        SetVisible(false);
    }

    void OnDestroy()
    {
        if (QuestManager.Instance != null)
            foreach (QuestController c in QuestManager.Instance.All)
            {
                c.OnPhaseChanged -= HandlePhaseChanged;
                c.OnConcluded -= HandleConcluded;
            }
    }

    void HandlePhaseChanged(QuestController c)
    {
        if (c.Phase == QuestPhase.After)
        {
            completionPending = c;

            Refresh();
        }
    }

    void HandleConcluded(QuestController c)
    {
        if (c != completionPending) 
            return;
        
        completionPending = null;
        isOpen = false;
        
        Refresh();
    }

    void Update()
    {
        if (PauseManager.IsPaused)
        {
            SetVisible(false);

            return;
        }

        if (isOpen || completionPending != null)
            Refresh();
    }

    void OnTrackerPressed(InputAction.CallbackContext ctx)
    {
        if (PauseManager.IsPaused) 
            return;
        
        if (completionPending != null) 
            return;

        isOpen = !isOpen;

        Refresh();
    }

    void Refresh()
    {
        if (completionPending != null)
        {
            if (descriptionLabel != null) 
                descriptionLabel.text = CompletionMessage;
            
            if (progressLabel != null) 
                progressLabel.text = string.Empty;
            
            SetVisible(true);
            
            return;
        }

        CountedQuestController tracked = FindActiveCounted();
        if (!isOpen || tracked == null)
        {
            SetVisible(false);
            
            return;
        }

        if (descriptionLabel != null && tracked != lastTracked)
        {
            descriptionLabel.text = tracked.Data.description;
            lastTracked = tracked;
        }

        if (progressLabel != null && tracked.Count != lastCount)
        {
            progressLabel.text = $"{tracked.Count} / {tracked.Target}";
            lastCount = tracked.Count;
        }

        SetVisible(true);
    }

    CountedQuestController FindActiveCounted()
    {
        if (QuestManager.Instance == null) 
            return null;
        
        foreach (QuestController c in QuestManager.Instance.All)
            if (c.Phase == QuestPhase.During && c is CountedQuestController counted)
                return counted;
        
        return null;
    }

    void SetVisible(bool visible)
    {
        if (trackerRoot != null)
            trackerRoot.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
    }
}
