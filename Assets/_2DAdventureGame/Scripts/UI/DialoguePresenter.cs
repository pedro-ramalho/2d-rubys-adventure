using System.Collections;
using AdventureGame.Entities.Player;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.UIElements;

namespace AdventureGame.UI
{
    [MovedFrom(autoUpdateAPI: true, sourceClassName: "UIHandler")]
    [RequireComponent(typeof(UIDocument))]
    public class DialoguePresenter : MonoBehaviour
    {
        public static DialoguePresenter Instance { get; private set; }

        [SerializeField] private float displayTime = 4.0f;
        [SerializeField] private AudioClip clickClip;

        [Header("Typewriter")]
        [SerializeField] private float typeInterval = 0.03f;
        [SerializeField] private AudioClip typeClip;
        [Tooltip("Play the type SFX every Nth visible character.")]
        [SerializeField] private int typeClipEveryNChars = 2;
        [SerializeField] private float typeClipPitchJitter = 0.08f;

        private VisualElement dialoguePanel;
        private Label dialogueText;

        [Header("Dialogue Range")]
        [SerializeField] private float dialogueMaxDistance = 5f;

        private Player player;
        private Coroutine typeRoutine;
        private string currentLine;
        private bool dialogueActive;
        private float originalOneShotVolume = 1f;
        private Transform currentSpeaker;

        public bool IsTyping => typeRoutine != null;
        public bool IsDialogueActive => dialogueActive;

        AudioSource OneShot() => player != null ? player.OneShotSource : null;

        void Awake()
        {
            if (Instance == null) Instance = this;
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
        
            dialoguePanel = uiDocument.rootVisualElement.Q<VisualElement>("NPCDialogue");
            dialogueText = dialoguePanel.Q<Label>("DialogueText");

            dialoguePanel.style.display = DisplayStyle.None;

            player = Player.Instance;
        }

        void RestoreTypingAudio()
        {
            AudioSource audio = OneShot();
            if (audio == null)
                return;

            audio.pitch = 1f;
            audio.volume = originalOneShotVolume;
        }

        public void DisplayDialogueWithLine(string line) => DisplayDialogueWithLine(line, null);
        public void DisplayDialogueWithLine(string line, Transform speaker)
        {
            currentSpeaker = speaker;

            if (InteractPromptPresenter.Instance != null)
                InteractPromptPresenter.Instance.HideInteractPrompt();

            AudioSource audio = OneShot();
            if (clickClip != null && audio != null)
                audio.PlayOneShot(clickClip);

            CancelInvoke(nameof(HideDialogue));
        
            if (typeRoutine != null)
                StopCoroutine(typeRoutine);

            dialogueActive = true;
            currentLine = line;
            dialogueText.text = string.Empty;
            dialoguePanel.style.opacity = 1f;
            dialoguePanel.style.display = DisplayStyle.Flex;
            typeRoutine = StartCoroutine(TypeLine(line));
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
                    if (typeClip != null && audio != null && visibleCount % typeClipEveryNChars == 0)
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

            dialoguePanel.style.display = DisplayStyle.None;
            dialoguePanel.style.opacity = 1f;
            dialogueActive = false;
            currentSpeaker = null;
        }
    }
}
