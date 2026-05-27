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


        SetVisible(false);        
    }

    void Update()
    {
        if (isOpen) Refresh();
    }

    void OnTrackerPressed(InputAction.CallbackContext ctx)
    {
        if (PauseManager.IsPaused) return;
        isOpen = !isOpen;
        Refresh();
    }

    void Refresh()
    {
        Quest quest = QuestManager.Instance?.ActiveQuest;
        if (!isOpen || quest == null)
        {
            SetVisible(false);
            return;
        }

        if (descriptionLabel != null)
            descriptionLabel.text = quest.Data.description;
        
        if (progressLabel != null)
            progressLabel.text = $"{quest.Count}/{quest.Data.objective.Count}";
        
        SetVisible(true);
    }

    void SetVisible(bool visible)
    {
        if (trackerRoot != null)
            trackerRoot.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
    }
}
