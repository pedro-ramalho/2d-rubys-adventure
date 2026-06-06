using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class UIHandler : MonoBehaviour
{
    public static UIHandler Instance { get; private set; }

    [SerializeField] private float displayTime = 4.0f;
    [SerializeField] private AudioClip clickClip;

    [Header("Typewriter")]
    [SerializeField] private float typeInterval = 0.03f;
    [SerializeField] private AudioClip typeClip;
    [Tooltip("Play the type SFX every Nth visible character.")]
    [SerializeField] private int typeClipEveryNChars = 2;
    [SerializeField] private float typeClipPitchJitter = 0.08f;

    private VisualElement healthBar;
    private VisualElement dialoguePanel;
    private Label dialogueText;
    private VisualElement winScreen;
    private VisualElement loseScreen;

    private Player player;
    private Coroutine typeRoutine;
    private string currentLine;
    private bool isShowingPrompt;

    public bool IsTyping => typeRoutine != null;

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

        player = Player.Instance;
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
        if (clickClip != null && player != null)
            player.OneShotSource.PlayOneShot(clickClip);

        CancelInvoke(nameof(HideDialogue));
        if (typeRoutine != null) StopCoroutine(typeRoutine);

        isShowingPrompt = false;
        currentLine = line;
        dialogueText.text = string.Empty;
        dialoguePanel.style.display = DisplayStyle.Flex;
        typeRoutine = StartCoroutine(TypeLine(line));
    }

    public void ShowInteractPrompt(string text)
    {
        if (!isShowingPrompt && dialoguePanel.style.display == DisplayStyle.Flex) return;
        if (isShowingPrompt && dialogueText.text == text) return;

        dialogueText.text = text;
        dialoguePanel.style.display = DisplayStyle.Flex;
        isShowingPrompt = true;
    }

    public void HideInteractPrompt()
    {
        if (!isShowingPrompt) return;
        dialoguePanel.style.display = DisplayStyle.None;
        isShowingPrompt = false;
    }

    public void Skip()
    {
        if (typeRoutine == null) return;
        StopCoroutine(typeRoutine);
        typeRoutine = null;
        dialogueText.text = currentLine;
        Invoke(nameof(HideDialogue), displayTime);
    }

    private IEnumerator TypeLine(string line)
    {
        player.OneShotSource.volume = 0.75f;

        int visibleCount = 0;
        for (int i = 1; i <= line.Length; i++)
        {
            dialogueText.text = line.Substring(0, i);
            char c = line[i - 1];

            if (!char.IsWhiteSpace(c))
            {
                visibleCount++;
                if (typeClip != null && player != null && visibleCount % typeClipEveryNChars == 0)
                {
                    float pitch = 1f + Random.Range(-typeClipPitchJitter, typeClipPitchJitter);
                    player.OneShotSource.pitch = pitch;
                    player.OneShotSource.PlayOneShot(typeClip);
                }
            }

            yield return new WaitForSeconds(typeInterval);
        }
        if (player != null) player.OneShotSource.pitch = 1f;
        typeRoutine = null;
        Invoke(nameof(HideDialogue), displayTime);
    }

    public void HideDialogue()
    {
        CancelInvoke(nameof(HideDialogue));
        if (typeRoutine != null)
        {
            StopCoroutine(typeRoutine);
            typeRoutine = null;
            if (player != null) player.OneShotSource.pitch = 1f;
        }
        dialoguePanel.style.display = DisplayStyle.None;
        isShowingPrompt = false;
    }
    
    public void DisplayWinScreen() => winScreen.style.opacity = 1.0f;
    
    public void DisplayLoseScreen() => loseScreen.style.opacity = 1.0f;
}
