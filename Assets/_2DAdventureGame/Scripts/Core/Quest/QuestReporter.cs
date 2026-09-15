using UnityEngine;
using UnityEngine.Serialization;

namespace AdventureGame.Core.Quests
{
    public class QuestReporter : MonoBehaviour
    {
        [SerializeField] private QuestData m_QuestData;

        [SerializeField] private string m_WorldId;

        public QuestData Data => m_QuestData;

        public void Report()
        {
            Quest quest = GetQuest();
            if (quest != null) 
                quest.TryReport(m_WorldId);
        }

        public bool CanReport()
        {
            Quest quest = GetQuest();

            return quest != null && quest.CanReport(m_WorldId);
        }

        public bool IsConsumed()
        {
            Quest quest = GetQuest();

            return quest != null && quest.IsConsumed(m_WorldId);
        }

        Quest GetQuest() =>
            QuestManager.Instance != null ? QuestManager.Instance.Get(m_QuestData) : null;
    }
}
