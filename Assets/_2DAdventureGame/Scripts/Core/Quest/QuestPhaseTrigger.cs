using System.Collections.Generic;
using UnityEngine;

namespace AdventureGame.Core.Quests
{
    public class QuestPhaseTrigger : MonoBehaviour
    {
        [SerializeField]
        private QuestData m_QuestData;

        [SerializeField]
        private QuestState m_EnableTriggerFrom = QuestState.Active;

        private readonly List<Collider2D> m_Colliders = new();

        void Awake()
        {
            GetComponentsInChildren(true, m_Colliders);

            SetTrigger(false);

            Quest.OnAnyPhaseChanged += OnQuestPhaseChanged;
        }

        void Start()
        {
            if (QuestManager.Instance == null)
                return;

            Quest quest = QuestManager.Instance.Get(m_QuestData);
            if (quest != null && HasReached(quest.State))
                SetTrigger(true);
        }

        void OnDestroy() => Quest.OnAnyPhaseChanged -= OnQuestPhaseChanged;

        void OnQuestPhaseChanged(Quest quest)
        {
            if (quest.Data != m_QuestData)
                return;

            if (HasReached(quest.State))
                SetTrigger(true);
        }

        void SetTrigger(bool value)
        {
            foreach (Collider2D collider in m_Colliders)
                if (collider != null)
                    collider.isTrigger = value;
        }

        bool HasReached(QuestState current) => (int)current >= (int)m_EnableTriggerFrom;
    }
}
