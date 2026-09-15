using AdventureGame.Core.Managers;
using UnityEngine;
using UnityEngine.UIElements;

namespace AdventureGame.UI
{
    [RequireComponent(typeof(UIDocument))]
    public class EndScreenPresenter : SceneSingleton<EndScreenPresenter>
    {
        [SerializeField]
        private AudioSource m_StingerSource;

        [SerializeField]
        private AudioClip m_DefeatSting;

        [SerializeField]
        private float m_MusicFadeOnEndScreen = 2f;

        private VisualElement m_WinScreen;
        private VisualElement m_LoseScreen;

        void Start()
        {
            VisualElement root = GetComponent<UIDocument>().rootVisualElement;

            m_WinScreen = root.Q<VisualElement>("WinScreenContainer");
            m_LoseScreen = root.Q<VisualElement>("LoseScreenContainer");
        }

        public void DisplayWinScreen()
        {
            if (m_WinScreen != null)
                m_WinScreen.style.opacity = 1.0f;
        }

        public void DisplayLoseScreen()
        {
            if (m_LoseScreen != null)
                m_LoseScreen.style.opacity = 1.0f;

            MusicManager.Instance?.FadeOutAndStop(m_MusicFadeOnEndScreen);

            if (m_StingerSource != null && m_DefeatSting != null)
                m_StingerSource.PlayOneShot(m_DefeatSting);
        }
    }
}
