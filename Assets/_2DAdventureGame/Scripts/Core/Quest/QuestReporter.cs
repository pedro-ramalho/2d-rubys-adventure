using UnityEngine;
using UnityEngine.Serialization;

namespace AdventureGame.Core.Quest
{
    public class QuestReporter : MonoBehaviour
    {
        [FormerlySerializedAs("quest")]
        [SerializeField] private QuestData m_QuestData;

        [FormerlySerializedAs("worldId")]
        [SerializeField] private string m_WorldId;

        public QuestData Quest => m_QuestData;
        public string WorldId => m_WorldId;

        public void Report()
        {
            CountedQuestController controller = GetCountedController();
            if (controller != null) 
                controller.TryReport(m_WorldId);
        }

        public bool CanReport()
        {
            CountedQuestController controller = GetCountedController();
            return controller != null && controller.CanReport(m_WorldId);
        }

        public bool IsConsumed()
        {
            CountedQuestController controller = GetCountedController();
            return controller != null && controller.IsConsumed(m_WorldId);
        }

        CountedQuestController GetCountedController() =>
            QuestManager.Instance != null ? QuestManager.Instance.Get(m_QuestData) as CountedQuestController : null;
    }
}
