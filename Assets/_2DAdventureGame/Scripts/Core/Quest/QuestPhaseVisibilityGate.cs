using System.Collections.Generic;
using UnityEngine;

namespace AdventureGame.Core.Quest
{
    public class QuestPhaseVisibilityGate : MonoBehaviour
    {
        [SerializeField] private QuestData m_Quest;
        [SerializeField] private QuestPhase m_ShowFrom = QuestPhase.During;

        private readonly List<SpriteRenderer> m_SpriteRenderers = new();
        private readonly List<Collider2D> m_Colliders = new();

        void Awake()
        {
            GetComponentsInChildren(true, m_SpriteRenderers);
            GetComponentsInChildren(true, m_Colliders);

            SetVisible(false);

            QuestController.OnAnyPhaseChanged += OnQuestPhaseChanged;
        }

        void Start()
        {
            if (QuestManager.Instance == null)
                return;

            QuestController controller = QuestManager.Instance.Get(m_Quest);
            if (controller != null && HasReached(controller.Phase))
                SetVisible(true);
        }

        void OnDestroy()
            => QuestController.OnAnyPhaseChanged -= OnQuestPhaseChanged;

        void OnQuestPhaseChanged(QuestController controller)
        {
            if (controller.Data != m_Quest)
                return;

            if (HasReached(controller.Phase))
                SetVisible(true);
        }

        void SetVisible(bool value)
        {
            foreach (SpriteRenderer renderer in m_SpriteRenderers)
                if (renderer != null)
                    renderer.enabled = value;

            foreach (Collider2D collider in m_Colliders)
                if (collider != null)
                    collider.enabled = value;
        }

        bool HasReached(QuestPhase current)
            => (int)current >= (int)m_ShowFrom;
    }
}
