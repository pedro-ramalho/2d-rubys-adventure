using AdventureGame.Entities.Player;
using UnityEngine;
using UnityEngine.UIElements;

namespace AdventureGame.UI
{
    [RequireComponent(typeof(UIDocument))]
    public class HealthBarHUD : SceneSingleton<HealthBarHUD>
    {
        private VisualElement m_Hud;
        private VisualElement m_HealthBar;
        private Player m_Player;

        void Start()
        {
            VisualElement root = GetComponent<UIDocument>().rootVisualElement;

            m_Hud = root.Q<VisualElement>("HealthBarBackground");
            m_HealthBar = root.Q<VisualElement>("HealthBar");

            m_Player = Player.Instance;
            m_Player.OnHealthChanged += OnPlayerHealthChanged;

            OnPlayerHealthChanged(m_Player.CurrentHealth / (float)m_Player.Data.MaxHealth);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (m_Player != null)
                m_Player.OnHealthChanged -= OnPlayerHealthChanged;
        }

        public void SetVisible(bool visible)
        {
            if (m_Hud == null)
                return;

            m_Hud.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
        }

        public void Show() => SetVisible(true);

        public void Hide() => SetVisible(false);

        private void OnPlayerHealthChanged(float percentage)
        {
            if (m_HealthBar != null)
                m_HealthBar.style.width = Length.Percent(100 * percentage);
        }
    }
}
