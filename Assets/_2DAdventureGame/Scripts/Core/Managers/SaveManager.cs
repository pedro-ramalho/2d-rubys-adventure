using System;
using System.Collections.Generic;
using System.IO;
using AdventureGame.Core.Quest;
using AdventureGame.Entities.Player;
using UnityEngine;

namespace AdventureGame.Core.Managers
{
    public class SaveManager : PersistentSingleton<SaveManager>
    {
        private const string SaveFileName = "save.json";
        private const int CurrentSaveVersion = 4;
        private const int NoStoredHealth = -1;

        public bool HasSave { get; private set; }
        public Save Current { get; private set; }

        public event Action SaveDeleted;

        private string SavePath => Path.Combine(Application.persistentDataPath, SaveFileName);

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Bootstrap() => BootstrapIfMissing();

        protected override void Awake()
        {
            base.Awake();

            if (Instance != this) 
                return;
        
            ReadFromDisk();
        }

        void ReadFromDisk()
        {
            HasSave = false;

            if (!File.Exists(SavePath)) 
                return;

            try
            {
                Save loaded = JsonUtility.FromJson<Save>(File.ReadAllText(SavePath));
                if (loaded?.version != CurrentSaveVersion) 
                    return;

                Current = loaded;
                HasSave = true;
            }
            catch { }
        }

        public void WriteSave(string sceneName)
        {
            List<QuestSaveData> quests = QuestManager.Instance != null
                ? QuestManager.Instance.CaptureAll()
                : PreserveQuestsFromCurrent();

            Save save = new Save
            {
                version = CurrentSaveVersion,
                sceneName = sceneName,
                playerHealth = Player.Instance != null ? Player.Instance.CurrentHealth : NoStoredHealth,
                quests = quests
            };

            try
            {
                string json = JsonUtility.ToJson(save, true);
            
                File.WriteAllText(SavePath, json);
            
                Current = save;
                HasSave = true;
            }
            catch
            {
                Debug.LogError("Failed to write save file.");
            }
        }

        List<QuestSaveData> PreserveQuestsFromCurrent()
        {
            if (Current?.quests == null) return new List<QuestSaveData>();

            List<QuestSaveData> copy = new();
        
            foreach (QuestSaveData q in Current.quests)
                copy.Add(new QuestSaveData
                {
                    questId = q.questId,
                    phase = q.phase,
                    consumedIds = q.consumedIds != null ? new List<string>(q.consumedIds) : null
                });

            return copy;
        }

        public void DeleteSave()
        {
            if (File.Exists(SavePath))
                File.Delete(SavePath);

            Current = null;
            HasSave = false;
        
            SaveDeleted?.Invoke();
        }
    }
}
