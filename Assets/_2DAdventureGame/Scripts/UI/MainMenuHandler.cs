using System.Collections;
using AdventureGame.Core.Constants;
using AdventureGame.Core.Managers;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace AdventureGame.UI
{
    [RequireComponent(typeof(UIDocument))]
    [RequireComponent(typeof(OptionsHandler))]
    public class MainMenuHandler : MonoBehaviour
    {
        [SerializeField] private string m_FirstLevelSceneName = SceneNames.Level0;

        [Header("Background Pan")]
        [SerializeField] private float m_BackgroundPanSpeed = 0.15f;

        [SerializeField] private float m_BackgroundPanAmplitudeX = 40f;

        [SerializeField] private float m_BackgroundPanAmplitudeY = 20f;

        [SerializeField] private float m_BackgroundScale = 1.15f;

        [Header("Title Bob")]
        [SerializeField] private float m_TitleBobSpeed = 1.5f;

        [SerializeField] private float m_TitleBobAmplitude = 12f;

        [Header("Click Sound")]
        [SerializeField] private AudioSource m_ClickAudioSource;

        [SerializeField] private AudioClip m_ClickClip;

        [Header("Start Game")]
        [SerializeField] private float m_StartGameFadeDuration = 3f;

        private VisualElement m_BackgroundPanel;
        private Label m_TitleLabel;
        private VisualElement m_ButtonContainer;
        private Button m_StartButton;
        private Button m_ContinueButton;
        private Button m_OptionsButton;
        private Button m_QuitButton;
        private OptionsHandler m_OptionsHandler;
        private bool m_IsStartingGame;

        void Start()
        {
            VisualElement root = GetComponent<UIDocument>().rootVisualElement;
            m_OptionsHandler = GetComponent<OptionsHandler>();

            m_BackgroundPanel = root.Q<VisualElement>("Background");
            m_TitleLabel = root.Q<Label>("Title");
            m_ButtonContainer = root.Q<VisualElement>("ButtonContainer");

            if (m_BackgroundPanel != null)
                m_BackgroundPanel.style.scale = new Scale(new Vector3(m_BackgroundScale, m_BackgroundScale, 1f));

            m_StartButton = root.Q<Button>("StartButton");
            m_ContinueButton = root.Q<Button>("ContinueButton");
            m_OptionsButton = root.Q<Button>("OptionsButton");
            m_QuitButton = root.Q<Button>("QuitButton");

            m_StartButton.clicked += PlayClick;
            m_OptionsButton.clicked += PlayClick;
            m_QuitButton.clicked += PlayClick;
            m_ContinueButton.clicked += PlayClick;

            m_StartButton.clicked += StartGame;
            m_OptionsButton.clicked += m_OptionsHandler.Open;
            m_QuitButton.clicked += QuitGame;

            m_OptionsHandler.Opened += () => m_ButtonContainer.style.display = DisplayStyle.None;
            m_OptionsHandler.Closed += () => m_ButtonContainer.style.display = DisplayStyle.Flex;

            if (SaveManager.Instance != null && SaveManager.Instance.HasSave)
            {
                m_ContinueButton.SetEnabled(true);
                m_ContinueButton.clicked += ContinueGame;
            }
            else
            {
                m_ContinueButton.SetEnabled(false);
            }

            if (SaveManager.Instance != null)
                SaveManager.Instance.SaveDeleted += OnSaveDeleted;
        }

        void OnDestroy()
        {
            if (SaveManager.Instance != null)
                SaveManager.Instance.SaveDeleted -= OnSaveDeleted;
        }

        void OnSaveDeleted()
        {
            if (m_ContinueButton == null) 
                return;
        
            m_ContinueButton.SetEnabled(false);
            m_ContinueButton.clicked -= ContinueGame;
        }

        public void PlayClick()
        {
            if (m_ClickAudioSource != null && m_ClickClip != null)
                m_ClickAudioSource.PlayOneShot(m_ClickClip);
        }

        void ContinueGame()
        {
            if (m_IsStartingGame) 
                return;
        
            if (SaveManager.Instance == null || !SaveManager.Instance.HasSave) 
                return;

            BeginSceneFadeAndLoad(SaveManager.Instance.Current.SceneName, writeSave: false);
        }

        void Update()
        {
            if (m_BackgroundPanel != null)
            {
                float bx = Mathf.Sin(Time.time * m_BackgroundPanSpeed) * m_BackgroundPanAmplitudeX;
                float by = Mathf.Cos(Time.time * m_BackgroundPanSpeed * 0.7f) * m_BackgroundPanAmplitudeY;
            
                m_BackgroundPanel.style.translate = new Translate(bx, by);
            }

            if (m_TitleLabel != null)
            {
                float ty = Mathf.Sin(Time.time * m_TitleBobSpeed) * m_TitleBobAmplitude;
            
                m_TitleLabel.style.translate = new Translate(0, ty);
            }
        }

        void StartGame()
        {
            if (m_IsStartingGame) 
                return;
        
            if (SaveManager.Instance != null) 
                SaveManager.Instance.DeleteSave();
        
            BeginSceneFadeAndLoad(m_FirstLevelSceneName, writeSave: true);
        }

        void BeginSceneFadeAndLoad(string sceneName, bool writeSave)
        {
            m_IsStartingGame = true;

            m_StartButton.SetEnabled(false);
            m_ContinueButton.SetEnabled(false);
            m_OptionsButton.SetEnabled(false);
            m_QuitButton.SetEnabled(false);

            if (MusicManager.Instance != null)
                MusicManager.Instance.FadeOutAndStop(m_StartGameFadeDuration);

            StartCoroutine(FadeOutAndLoad(sceneName, writeSave));
        }

        IEnumerator FadeOutAndLoad(string sceneName, bool writeSave)
        {
            VisualElement root = GetComponent<UIDocument>().rootVisualElement;
            VisualElement overlay = root.Q<VisualElement>("FadeOverlay");

            float elapsed = 0f;
            while (elapsed < m_StartGameFadeDuration)
            {
                elapsed += Time.deltaTime;
                if (overlay != null)
                    overlay.style.opacity = Mathf.Lerp(0f, 1f, elapsed / m_StartGameFadeDuration);
            
                yield return null;
            }

            if (overlay != null) 
                overlay.style.opacity = 1f;

            if (writeSave && sceneName != SceneNames.MainMenu && SaveManager.Instance != null)
                SaveManager.Instance.WriteSave(sceneName);

            SceneManager.LoadScene(sceneName);
        }

        void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        }
    }
}
