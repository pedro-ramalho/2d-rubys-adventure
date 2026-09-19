using UnityEngine;
using UnityEngine.Events;

namespace AdventureGame.Core.Quests
{
    public class QuestStateGate : MonoBehaviour
    {
        [SerializeField]
        private QuestDefinition m_Quest;

        [SerializeField]
        private QuestState m_OpenFrom = QuestState.Active;

        [SerializeField]
        private UnityEvent m_OnOpen;

        private bool m_IsOpen;

        void Awake() => Quest.OnStateChanged += HandleStateChanged;

        void OnDestroy() => Quest.OnStateChanged -= HandleStateChanged;

        void Start()
        {
            if (QuestManager.Instance == null)
                return;

            Quest quest = QuestManager.Instance.Get(m_Quest);
            if (quest != null && HasReached(quest.State))
                Open();
        }

        void HandleStateChanged(Quest quest)
        {
            if (quest.Data != m_Quest)
                return;

            if (HasReached(quest.State))
                Open();
        }

        void Open()
        {
            if (m_IsOpen)
                return;

            m_IsOpen = true;

            m_OnOpen?.Invoke();
        }

        bool HasReached(QuestState current) => (int)current >= (int)m_OpenFrom;
    }
}
