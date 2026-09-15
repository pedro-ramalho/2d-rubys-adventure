using System.Collections;
using AdventureGame.Core.Managers;
using AdventureGame.Entities.Player;
using UnityEngine;
using UnityEngine.UIElements;

namespace AdventureGame.UI
{
    [RequireComponent(typeof(UIDocument))]
    public class UIHandler : MonoBehaviour
    {
        public static UIHandler Instance { get; private set; }

        [SerializeField]
        private float displayTime = 4.0f;

        [SerializeField]
        private AudioClip clickClip;

        [Header("Typewriter")]
        [SerializeField]
        private float typeInterval = 0.03f;

        [SerializeField]
        private AudioClip typeClip;

        [Tooltip("Play the type SFX every Nth visible character.")]
        [SerializeField]
        private int typeClipEveryNChars = 2;

        [SerializeField]
        private float typeClipPitchJitter = 0.08f;

        [Header("Prompt Fade")]
        [SerializeField]
        private float promptFadeDuration = 0.15f;

        private VisualElement hud;
        private VisualElement healthBar;
        private VisualElement dialoguePanel;
        private Label dialogueText;
        private VisualElement winScreen;
        private VisualElement loseScreen;

        [Header("Dialogue Range")]
        [SerializeField]
        private float dialogueMaxDistance = 5f;

        [Header("End Screens")]
        [SerializeField]
        private AudioSource stingerSource;

        [SerializeField]
        private AudioClip defeatSting;

        [SerializeField]
        private float musicFadeOnEndScreen = 2f;

        private Player player;
        private Coroutine typeRoutine;
        private string currentLine;
        private bool isShowingPrompt;
        private bool dialogueActive;
        private Coroutine promptFadeRoutine;
        private float originalOneShotVolume = 1f;
        private Transform currentSpeaker;

        public bool IsTyping => typeRoutine != null;

        AudioSource OneShot() => player != null ? player.OneShotSource : null;

        void Awake()
        {
            if (Instance == null)
                Instance = this;
        }

        void Update()
        {
            if (!dialogueActive || currentSpeaker == null)
                return;

            Player player = Player.Instance;
            if (player == null)
                return;

            float distance = (player.transform.position - currentSpeaker.position).sqrMagnitude;
            if (distance > dialogueMaxDistance * dialogueMaxDistance)
                HideDialogue();
        }

        void Start()
        {
            UIDocument uiDocument = GetComponent<UIDocument>();
            hud = uiDocument.rootVisualElement.Q<VisualElement>("HealthBarBackground");
            healthBar = uiDocument.rootVisualElement.Q<VisualElement>("HealthBar");

            dialoguePanel = uiDocument.rootVisualElement.Q<VisualElement>("NPCDialogue");
            dialogueText = dialoguePanel.Q<Label>("DialogueText");

            loseScreen = uiDocument.rootVisualElement.Q<VisualElement>("LoseScreenContainer");
            winScreen = uiDocument.rootVisualElement.Q<VisualElement>("WinScreenContainer");

            dialoguePanel.style.display = DisplayStyle.None;

            player = Player.Instance;
            player.OnHealthChanged += SetHealthValue;
            SetHealthValue(player.CurrentHealth / (float)player.Data.MaxHealth);
        }

        void OnDestroy()
        {
            if (player != null)
                player.OnHealthChanged -= SetHealthValue;
        }

        void SetHealthValue(float percentage)
        {
            if (healthBar != null)
                healthBar.style.width = Length.Percent(100 * percentage);
        }

        void RestoreTypingAudio()
        {
            AudioSource audio = OneShot();
            if (audio == null)
                return;

            audio.pitch = 1f;
            audio.volume = originalOneShotVolume;
        }

        public void SetHUDVisible(bool visible)
        {
            if (hud == null)
                return;

            hud.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
        }

        public void HideHUD() => SetHUDVisible(false);

        public void ShowHUD() => SetHUDVisible(true);

        public void DisplayDialogueWithLine(string line) => DisplayDialogueWithLine(line, null);

        public void DisplayDialogueWithLine(string line, Transform speaker)
        {
            currentSpeaker = speaker;

            AudioSource audio = OneShot();
            if (clickClip != null && audio != null)
                audio.PlayOneShot(clickClip);

            CancelInvoke(nameof(HideDialogue));

            if (typeRoutine != null)
                StopCoroutine(typeRoutine);

            StopPromptFade();

            isShowingPrompt = false;
            dialogueActive = true;
            currentLine = line;
            dialogueText.text = string.Empty;
            dialoguePanel.style.opacity = 1f;
            dialoguePanel.style.display = DisplayStyle.Flex;
            typeRoutine = StartCoroutine(TypeLine(line));
        }

        public void ShowInteractPrompt(string text)
        {
            if (dialogueActive)
                return;

            if (isShowingPrompt && dialogueText.text == text && promptFadeRoutine == null)
                return;

            dialogueText.text = text;
            if (dialoguePanel.style.display == DisplayStyle.None)
                dialoguePanel.style.opacity = 0f;

            dialoguePanel.style.display = DisplayStyle.Flex;
            isShowingPrompt = true;

            StartPromptFade(1f, hideAfter: false);
        }

        public void HideInteractPrompt()
        {
            if (!isShowingPrompt)
                return;

            isShowingPrompt = false;

            StartPromptFade(0f, hideAfter: true);
        }

        private void StartPromptFade(float targetOpacity, bool hideAfter)
        {
            StopPromptFade();

            promptFadeRoutine = StartCoroutine(FadePrompt(targetOpacity, hideAfter));
        }

        private void StopPromptFade()
        {
            if (promptFadeRoutine != null)
            {
                StopCoroutine(promptFadeRoutine);
                promptFadeRoutine = null;
            }
        }

        private IEnumerator FadePrompt(float targetOpacity, bool hideAfter)
        {
            float startOpacity = dialoguePanel.resolvedStyle.opacity;

            float elapsed = 0f;
            while (elapsed < promptFadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.SmoothStep(0f, 1f, elapsed / promptFadeDuration);
                dialoguePanel.style.opacity = Mathf.Lerp(startOpacity, targetOpacity, t);
                yield return null;
            }

            dialoguePanel.style.opacity = targetOpacity;

            if (hideAfter)
                dialoguePanel.style.display = DisplayStyle.None;

            promptFadeRoutine = null;
        }

        public void Skip()
        {
            if (typeRoutine == null)
                return;

            StopCoroutine(typeRoutine);

            typeRoutine = null;

            RestoreTypingAudio();

            dialogueText.text = currentLine;

            Invoke(nameof(HideDialogue), displayTime);
        }

        private IEnumerator TypeLine(string line)
        {
            AudioSource audio = OneShot();
            if (audio != null)
            {
                originalOneShotVolume = audio.volume;
                audio.volume = 0.75f;
            }

            int visibleCount = 0;
            for (int i = 1; i <= line.Length; i++)
            {
                dialogueText.text = line.Substring(0, i);
                char c = line[i - 1];

                if (!char.IsWhiteSpace(c))
                {
                    visibleCount++;
                    if (
                        typeClip != null
                        && audio != null
                        && visibleCount % typeClipEveryNChars == 0
                    )
                    {
                        float pitch = 1f + Random.Range(-typeClipPitchJitter, typeClipPitchJitter);
                        audio.pitch = pitch;
                        audio.PlayOneShot(typeClip);
                    }
                }

                yield return new WaitForSeconds(typeInterval);
            }

            RestoreTypingAudio();

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

                RestoreTypingAudio();
            }

            StopPromptFade();

            dialoguePanel.style.display = DisplayStyle.None;
            dialoguePanel.style.opacity = 1f;
            isShowingPrompt = false;
            dialogueActive = false;
            currentSpeaker = null;
        }

        public void DisplayWinScreen() => winScreen.style.opacity = 1.0f;

        public void DisplayLoseScreen()
        {
            if (loseScreen != null)
                loseScreen.style.opacity = 1.0f;

            MusicManager.Instance?.FadeOutAndStop(musicFadeOnEndScreen);

            if (stingerSource != null && defeatSting != null)
                stingerSource.PlayOneShot(defeatSting);
        }
    }
}
