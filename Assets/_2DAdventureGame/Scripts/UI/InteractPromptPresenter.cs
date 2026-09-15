using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace AdventureGame.UI
{
    [RequireComponent(typeof(UIDocument))]
    public class InteractPromptPresenter : SceneSingleton<InteractPromptPresenter>
    {
        [SerializeField]
        private float m_PromptFadeDuration = 0.15f;

        private VisualElement m_Panel;
        private Label m_Label;
        private bool m_IsShowingPrompt;
        private Coroutine m_FadeRoutine;

        void Start()
        {
            VisualElement root = GetComponent<UIDocument>().rootVisualElement;
            m_Panel = root.Q<VisualElement>("InteractPromptPanel");
            m_Label = m_Panel.Q<Label>("InteractPromptText");

            m_Panel.style.display = DisplayStyle.None;
        }

        public void ShowInteractPrompt(string text)
        {
            if (DialoguePresenter.Instance != null && DialoguePresenter.Instance.IsDialogueActive)
                return;

            if (m_IsShowingPrompt && m_Label.text == text && m_FadeRoutine == null)
                return;

            m_Label.text = text;
            if (m_Panel.style.display == DisplayStyle.None)
                m_Panel.style.opacity = 0f;

            m_Panel.style.display = DisplayStyle.Flex;
            m_IsShowingPrompt = true;

            StartFade(1f, hideAfter: false);
        }

        public void HideInteractPrompt()
        {
            if (!m_IsShowingPrompt)
                return;

            m_IsShowingPrompt = false;

            StartFade(0f, hideAfter: true);
        }

        private void StartFade(float targetOpacity, bool hideAfter)
        {
            StopFade();
            m_FadeRoutine = StartCoroutine(Fade(targetOpacity, hideAfter));
        }

        private void StopFade()
        {
            if (m_FadeRoutine != null)
            {
                StopCoroutine(m_FadeRoutine);
                m_FadeRoutine = null;
            }
        }

        private IEnumerator Fade(float targetOpacity, bool hideAfter)
        {
            float startOpacity = m_Panel.resolvedStyle.opacity;

            float elapsed = 0f;
            while (elapsed < m_PromptFadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.SmoothStep(0f, 1f, elapsed / m_PromptFadeDuration);
                m_Panel.style.opacity = Mathf.Lerp(startOpacity, targetOpacity, t);
                yield return null;
            }

            m_Panel.style.opacity = targetOpacity;

            if (hideAfter)
                m_Panel.style.display = DisplayStyle.None;

            m_FadeRoutine = null;
        }
    }
}
