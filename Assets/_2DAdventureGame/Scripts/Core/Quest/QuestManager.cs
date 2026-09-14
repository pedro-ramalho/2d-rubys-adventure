using System.Collections.Generic;
using AdventureGame.Core.Managers;
using UnityEngine;

namespace AdventureGame.Core.Quest
{
    [DefaultExecutionOrder(-100)]
    public class QuestManager : SceneSingleton<QuestManager>
    {
        private readonly Dictionary<string, QuestController> m_QuestControllers = new();

        void Start()
        {
            if (SaveManager.Instance != null && SaveManager.Instance.HasSave)
                RestoreFromSave(SaveManager.Instance.Current);
        }

        public void Register(QuestController controller)
        {
            if (controller == null || controller.Data == null || string.IsNullOrEmpty(controller.Data.Id))
                return;
        
            m_QuestControllers[controller.Data.Id] = controller;
        }

        public void Unregister(QuestController controller)
        {
            if (controller == null || controller.Data == null) 
                return;
        
            if (m_QuestControllers.TryGetValue(controller.Data.Id, out QuestController current) && current == controller)
                m_QuestControllers.Remove(controller.Data.Id);
        }

        public QuestController Get(string questId) =>
            !string.IsNullOrEmpty(questId) && m_QuestControllers.TryGetValue(questId, out QuestController c) ? c : null;

        public QuestController Get(QuestData questData) =>
            questData != null ? Get(questData.Id) : null;

        public IEnumerable<QuestController> All => m_QuestControllers.Values;

        public List<QuestSaveData> CaptureAll()
        {
            List<QuestSaveData> list = new();

            foreach (QuestController controller in m_QuestControllers.Values)
                list.Add(controller.Capture());
        
            return list;
        }

        void RestoreFromSave(Save save)
        {
            if (save?.Quests == null) 
                return;
        
            foreach (QuestSaveData saved in save.Quests)
                if (saved != null && m_QuestControllers.TryGetValue(saved.QuestId, out QuestController controller))
                    controller.Restore(saved);
        }
    }
}
