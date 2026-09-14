using System.Collections.Generic;

namespace AdventureGame.Core.Quest
{
    public class CountedQuestController : QuestController
    {
        private readonly HashSet<string> m_ConsumedIds = new();

        public int Count => m_ConsumedIds.Count;
        public int Target => m_QuestData != null ? m_QuestData.TargetCount : 0;

        public bool IsConsumed(string worldId) =>
            !string.IsNullOrEmpty(worldId) && m_ConsumedIds.Contains(worldId);

        public bool CanReport(string worldId) =>
            Phase == QuestPhase.During &&
            !string.IsNullOrEmpty(worldId) &&
            !m_ConsumedIds.Contains(worldId);

        public bool TryReport(string worldId)
        {
            if (!CanReport(worldId)) 
                return false;
        
            m_ConsumedIds.Add(worldId);

            if (m_ConsumedIds.Count >= Target)
                MarkComplete();

            return true;
        }

        protected override List<string> CaptureConsumed() => new(m_ConsumedIds);

        protected override void RestoreData(QuestSaveData saved)
        {
            m_ConsumedIds.Clear();

            if (saved.ConsumedIds == null) 
                return;

            foreach (string id in saved.ConsumedIds)
                if (!string.IsNullOrEmpty(id))
                    m_ConsumedIds.Add(id);
        }
    }
}
