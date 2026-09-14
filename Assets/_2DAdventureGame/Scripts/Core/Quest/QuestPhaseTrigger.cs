using System.Collections.Generic;
using UnityEngine;

namespace AdventureGame.Core.Quest
{
    public class QuestPhaseTrigger : MonoBehaviour
    {
        [SerializeField] private QuestData m_QuestData;
        [SerializeField] private QuestPhase m_EnableTriggerFrom = QuestPhase.During;

        private readonly List<Collider2D> m_Colliders = new();

        void Awake()
        {
            GetComponentsInChildren(true, m_Colliders);

            SetTrigger(false);

            QuestController.OnAnyPhaseChanged += OnQuestPhaseChanged;
        }

        void Start()
        {
            if (QuestManager.Instance == null)
                return;

            QuestController controller = QuestManager.Instance.Get(m_QuestData);
            if (controller != null && HasReached(controller.Phase))
                SetTrigger(true);
        }

        void OnDestroy()
            => QuestController.OnAnyPhaseChanged -= OnQuestPhaseChanged;

        void OnQuestPhaseChanged(QuestController controller)
        {
            if (controller.Data != m_QuestData)
                return;

            if (HasReached(controller.Phase))
                SetTrigger(true);
        }

        void SetTrigger(bool value)
        {
            foreach (Collider2D collider in m_Colliders)
                if (collider != null)
                    collider.isTrigger = value;
        }

        bool HasReached(QuestPhase current)
            => (int)current >= (int)m_EnableTriggerFrom;
    }
}
