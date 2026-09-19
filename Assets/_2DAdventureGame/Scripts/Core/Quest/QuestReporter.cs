using UnityEngine;
using UnityEngine.Serialization;

namespace AdventureGame.Core.Quests
{
    public class QuestReporter : MonoBehaviour
    {
        [FormerlySerializedAs("quest")]
        [SerializeField]
        private QuestDefinition m_QuestData;

        public QuestDefinition Data => m_QuestData;

        public void Report()
        {
            Quest quest = GetQuest();
            if (quest != null)
                quest.TryReport();
        }

        public bool CanReport()
        {
            Quest quest = GetQuest();

            return quest != null && quest.CanReport;
        }

        public bool IsConsumed()
        {
            Quest quest = GetQuest();

            return quest != null && (int)quest.State >= (int)QuestState.Complete;
        }

        Quest GetQuest() =>
            QuestManager.Instance != null ? QuestManager.Instance.Get(m_QuestData) : null;
    }
}
