using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class UIHandler : MonoBehaviour
{
    public static UIHandler Instance { get; private set; }

    [SerializeField] private float displayTime = 4.0f;

    private VisualElement healthBar;
    private VisualElement dialoguePanel;
    private Label dialogueText;
    private VisualElement winScreen;
    private VisualElement loseScreen;

    private Player player;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        UIDocument uiDocument = GetComponent<UIDocument>();
        healthBar = uiDocument.rootVisualElement.Q<VisualElement>("HealthBar");
        
        dialoguePanel = uiDocument.rootVisualElement.Q<VisualElement>("NPCDialogue");
        dialogueText = dialoguePanel.Q<Label>("DialogueText");

        loseScreen = uiDocument.rootVisualElement.Q<VisualElement>("LoseScreenContainer");
        winScreen = uiDocument.rootVisualElement.Q<VisualElement>("WinScreenContainer");

        dialoguePanel.style.display = DisplayStyle.None;

        player = FindAnyObjectByType<Player>();
        player.OnHealthChanged += SetHealthValue;
        SetHealthValue(player.CurrentHealth / (float)player.Data.maxHealth);
    }

    void OnDestroy()
    {
        if (player != null)
            player.OnHealthChanged -= SetHealthValue;
    }

    void SetHealthValue(float percentage) => healthBar.style.width = Length.Percent(100 * percentage);
    
    public void DisplayDialogueWithLine(string line)
    {
        dialogueText.text = line;
        dialoguePanel.style.display = DisplayStyle.Flex;

        CancelInvoke(nameof(HideDialogue));
        Invoke(nameof(HideDialogue), displayTime);
    }

    void HideDialogue() => dialoguePanel.style.display = DisplayStyle.None;

    public void SetVisible(bool visible) => GetComponent<UIDocument>().rootVisualElement.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
    
    public void DisplayWinScreen() => winScreen.style.opacity = 1.0f;
    
    public void DisplayLoseScreen() => loseScreen.style.opacity = 1.0f;
}
