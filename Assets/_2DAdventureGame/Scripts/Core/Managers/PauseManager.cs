using System.Collections;
using AdventureGame.Core.Constants;
using AdventureGame.Core.Scene;
using AdventureGame.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace AdventureGame.Core.Managers
{
    public class PauseManager : PersistentSingleton<PauseManager>
    {
        public static bool IsPaused { get; private set; }

        [SerializeField]
        private UIDocument m_PauseUiDocument;

        [SerializeField]
        private AudioSource m_SfxAudioSource;

        [SerializeField]
        private AudioClip m_ToggleSfx;

        private PlayerInputActions m_InputActions;
        private VisualElement m_PauseRoot;
        private Label m_SaveConfirmationLabel;
        private Coroutine m_SaveConfirmationCoroutine;

        protected override void Awake()
        {
            base.Awake();

            if (Instance != this)
                return;

            m_InputActions = InputManager.Instance.Actions;
        }

        void OnEnable()
        {
            if (Instance != this)
                return;

            m_InputActions.Player.Pause.performed += OnPausePressed;
        }

        void OnDisable()
        {
            if (Instance != this)
                return;

            m_InputActions.Player.Pause.performed -= OnPausePressed;
        }

        void Start()
        {
            if (Instance != this)
                return;

            m_PauseRoot = m_PauseUiDocument.rootVisualElement.Q<VisualElement>("PauseRoot");
            m_SaveConfirmationLabel = m_PauseRoot.Q<Label>("SaveConfirmationLabel");
            Button saveButton = m_PauseRoot.Q<Button>("SaveButton");
            Button returnButton = m_PauseRoot.Q<Button>("ReturnButton");

            saveButton.clicked += () =>
            {
                SaveManager.Instance?.WriteSave(SceneManager.GetActiveScene().name);
                ShowSaveFeedback();
            };

            returnButton.clicked += () =>
            {
                Resume();
                SceneTransitioner.Instance?.LoadSceneWithCrossfade(SceneNames.MainMenu, 0f);
            };

            SetVisible(false);
        }

        void ShowSaveFeedback()
        {
            if (m_SaveConfirmationLabel == null)
                return;

            if (m_SaveConfirmationCoroutine != null)
                StopCoroutine(m_SaveConfirmationCoroutine);

            m_SaveConfirmationCoroutine = StartCoroutine(SaveFeedbackRoutine());
        }

        IEnumerator SaveFeedbackRoutine()
        {
            m_SaveConfirmationLabel.style.opacity = 1f;

            yield return new WaitForSecondsRealtime(2f);

            m_SaveConfirmationLabel.style.opacity = 0f;
            m_SaveConfirmationCoroutine = null;
        }

        void OnPausePressed(InputAction.CallbackContext ctx)
        {
            if (SceneManager.GetActiveScene().name == SceneNames.MainMenu)
                return;

            if (SceneTransitioner.Instance != null && SceneTransitioner.Instance.IsTransitioning)
                return;

            if (IsPaused)
                Resume();
            else
                Pause();
        }

        public void Pause() => SetPaused(true);

        public void Resume() => SetPaused(false);

        void SetPaused(bool paused)
        {
            IsPaused = paused;
            Time.timeScale = paused ? 0f : 1f;
            SetVisible(paused);
            if (HealthBarHUD.Instance != null)
                HealthBarHUD.Instance.SetVisible(!paused);
            PlayToggleSfx();
        }

        void PlayToggleSfx()
        {
            if (m_SfxAudioSource != null && m_ToggleSfx != null)
                m_SfxAudioSource.PlayOneShot(m_ToggleSfx);
        }

        void SetVisible(bool visible)
        {
            if (m_PauseRoot != null)
                m_PauseRoot.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }
}
