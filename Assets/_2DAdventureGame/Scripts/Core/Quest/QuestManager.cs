using System.Collections.Generic;
using AdventureGame.Core.Managers;
using UnityEngine;

namespace AdventureGame.Core.Quests
{
    [DefaultExecutionOrder(-100)]
    public class QuestManager : SceneSingleton<QuestManager>
    {
        private readonly Dictionary<string, Quest> m_Quests = new();

        void Start()
        {
            if (SaveManager.Instance != null && SaveManager.Instance.HasSave)
                RestoreFromSave(SaveManager.Instance.Current);
        }

        public void Register(Quest quest)
        {
            if (quest == null || quest.Data == null || string.IsNullOrEmpty(quest.Data.Id))
                return;

            m_Quests[quest.Data.Id] = quest;
        }

        public void Unregister(Quest quest)
        {
            if (quest == null || quest.Data == null)
                return;

            if (m_Quests.TryGetValue(quest.Data.Id, out Quest current) && current == quest)
                m_Quests.Remove(quest.Data.Id);
        }

        public Quest Get(string questId) =>
            !string.IsNullOrEmpty(questId) && m_Quests.TryGetValue(questId, out Quest q) ? q : null;

        public Quest Get(QuestData questData) =>
            questData != null ? Get(questData.Id) : null;

        public IEnumerable<Quest> All => m_Quests.Values;

        public List<QuestSaveData> CaptureAll()
        {
            List<QuestSaveData> list = new();

            foreach (Quest quest in m_Quests.Values)
                list.Add(quest.Capture());

            return list;
        }

        void RestoreFromSave(Save save)
        {
            if (save?.Quests == null)
                return;

            foreach (QuestSaveData saved in save.Quests)
                if (saved != null && m_Quests.TryGetValue(saved.QuestId, out Quest quest))
                    quest.Restore(saved);
        }
    }
}
