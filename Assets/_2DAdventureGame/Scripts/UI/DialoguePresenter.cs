using System.Collections;
using AdventureGame.Entities.Player;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace AdventureGame.UI
{
    [MovedFrom(autoUpdateAPI: true, sourceClassName: "UIHandler")]
    [RequireComponent(typeof(UIDocument))]
    public class DialoguePresenter : SceneSingleton<DialoguePresenter>
    {
        [FormerlySerializedAs("displayTime")]
        [SerializeField] private float m_DisplayTime = 4.0f;

        [FormerlySerializedAs("clickClip")]
        [SerializeField] private AudioClip m_ClickClip;

        [Header("Typewriter")]
        [FormerlySerializedAs("typeInterval")]
        [SerializeField] private float m_TypeInterval = 0.03f;

        [FormerlySerializedAs("typeClip")]
        [SerializeField] private AudioClip m_TypeClip;
        [Tooltip("Play the type SFX every Nth visible character.")]

        [FormerlySerializedAs("typeClipEveryNChars")]
        [SerializeField] private int m_TypeClipEveryNChars = 2;

        [FormerlySerializedAs("typeClipPitchJitter")]
        [SerializeField] private float m_TypeClipPitchJitter = 0.08f;

        private VisualElement m_DialoguePanel;
        private Label m_DialogueLabel;

        [Header("Dialogue Range")]
        [FormerlySerializedAs("dialogueMaxDistance")]
        [SerializeField] private float m_DialogueMaxDistance = 5f;

        private Player m_Player;
        private Coroutine m_TypeCoroutine;
        private string m_CurrentLine;
        private bool m_IsDialogueActive;
        private float m_OriginalOneShotVolume = 1f;
        private Transform m_CurrentSpeaker;

        public bool IsTyping => m_TypeCoroutine != null;
        public bool IsDialogueActive => m_IsDialogueActive;

        AudioSource OneShot() => m_Player != null ? m_Player.OneShotSource : null;

        void Update()
        {
            if (!m_IsDialogueActive || m_CurrentSpeaker == null)
                return;

            Player player = Player.Instance;
            if (player == null)
                return;

            float distance = (player.transform.position - m_CurrentSpeaker.position).sqrMagnitude;
            if (distance > m_DialogueMaxDistance * m_DialogueMaxDistance)
                HideDialogue();
        }

        void Start()
        {
            UIDocument uiDocument = GetComponent<UIDocument>();
        
            m_DialoguePanel = uiDocument.rootVisualElement.Q<VisualElement>("NPCDialogue");
            m_DialogueLabel = m_DialoguePanel.Q<Label>("DialogueText");

            m_DialoguePanel.style.display = DisplayStyle.None;

            m_Player = Player.Instance;
        }

        void RestoreTypingAudio()
        {
            AudioSource audio = OneShot();
            if (audio == null)
                return;

            audio.pitch = 1f;
            audio.volume = m_OriginalOneShotVolume;
        }

        public void DisplayDialogueWithLine(string line) => DisplayDialogueWithLine(line, null);
        public void DisplayDialogueWithLine(string line, Transform speaker)
        {
            m_CurrentSpeaker = speaker;

            if (InteractPromptPresenter.Instance != null)
                InteractPromptPresenter.Instance.HideInteractPrompt();

            AudioSource audio = OneShot();
            if (m_ClickClip != null && audio != null)
                audio.PlayOneShot(m_ClickClip);

            CancelInvoke(nameof(HideDialogue));
        
            if (m_TypeCoroutine != null)
                StopCoroutine(m_TypeCoroutine);

            m_IsDialogueActive = true;
            m_CurrentLine = line;
            m_DialogueLabel.text = string.Empty;
            m_DialoguePanel.style.opacity = 1f;
            m_DialoguePanel.style.display = DisplayStyle.Flex;
            m_TypeCoroutine = StartCoroutine(TypeLine(line));
        }

        public void Skip()
        {
            if (m_TypeCoroutine == null) 
                return;
        
            StopCoroutine(m_TypeCoroutine);
        
            m_TypeCoroutine = null;
        
            RestoreTypingAudio();
        
            m_DialogueLabel.text = m_CurrentLine;
        
            Invoke(nameof(HideDialogue), m_DisplayTime);
        }

        private IEnumerator TypeLine(string line)
        {
            AudioSource audio = OneShot();
            if (audio != null)
            {
                m_OriginalOneShotVolume = audio.volume;
                audio.volume = 0.75f;
            }

            int visibleCount = 0;
            for (int i = 1; i <= line.Length; i++)
            {
                m_DialogueLabel.text = line.Substring(0, i);
                char c = line[i - 1];

                if (!char.IsWhiteSpace(c))
                {
                    visibleCount++;
                    if (m_TypeClip != null && audio != null && visibleCount % m_TypeClipEveryNChars == 0)
                    {
                        float pitch = 1f + Random.Range(-m_TypeClipPitchJitter, m_TypeClipPitchJitter);
                        audio.pitch = pitch;
                        audio.PlayOneShot(m_TypeClip);
                    }
                }

                yield return new WaitForSeconds(m_TypeInterval);
            }

            RestoreTypingAudio();
        
            m_TypeCoroutine = null;
        
            Invoke(nameof(HideDialogue), m_DisplayTime);
        }

        public void HideDialogue()
        {
            CancelInvoke(nameof(HideDialogue));
            if (m_TypeCoroutine != null)
            {
                StopCoroutine(m_TypeCoroutine);
            
                m_TypeCoroutine = null;
            
                RestoreTypingAudio();
            }

            m_DialoguePanel.style.display = DisplayStyle.None;
            m_DialoguePanel.style.opacity = 1f;
            m_IsDialogueActive = false;
            m_CurrentSpeaker = null;
        }
    }
}
