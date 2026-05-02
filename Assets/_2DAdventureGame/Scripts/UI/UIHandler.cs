using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class UIHandler : MonoBehaviour
{
    public static UIHandler Instance { get; private set; }

    [SerializeField] private float displayTime = 4.0f;

    private VisualElement healthBar;
    private VisualElement dialoguePanel;
    private VisualElement winScreen;
    private VisualElement loseScreen;

    private PlayerHealth playerHealth;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        UIDocument uiDocument = GetComponent<UIDocument>();
        healthBar = uiDocument.rootVisualElement.Q<VisualElement>("HealthBar");
        dialoguePanel = uiDocument.rootVisualElement.Q<VisualElement>("NPCDialogue");
        loseScreen = uiDocument.rootVisualElement.Q<VisualElement>("LoseScreenContainer");
        winScreen = uiDocument.rootVisualElement.Q<VisualElement>("WinScreenContainer");

        dialoguePanel.style.display = DisplayStyle.None;

        playerHealth = FindAnyObjectByType<PlayerHealth>();
        playerHealth.OnHealthChanged += SetHealthValue;
        SetHealthValue(playerHealth.Health / (float)playerHealth.MaxHealth);
    }

    void OnDestroy()
    {
        if (playerHealth != null)
            playerHealth.OnHealthChanged -= SetHealthValue;
    }

    void SetHealthValue(float percentage)
    {
        healthBar.style.width = Length.Percent(100 * percentage);
    }

    public void DisplayDialogue()
    {
        dialoguePanel.style.display = DisplayStyle.Flex;
        CancelInvoke(nameof(HideDialogue));
        Invoke(nameof(HideDialogue), displayTime);
    }

    void HideDialogue()
    {
        dialoguePanel.style.display = DisplayStyle.None;
    }

    public void SetVisible(bool visible)
    {
        GetComponent<UIDocument>().rootVisualElement.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
    }

    public void DisplayWinScreen()
    {
        winScreen.style.opacity = 1.0f;
    }

    public void DisplayLoseScreen()
    {
        loseScreen.style.opacity = 1.0f;
    }
}
