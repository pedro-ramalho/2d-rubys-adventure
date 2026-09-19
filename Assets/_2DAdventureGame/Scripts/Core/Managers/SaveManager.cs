using System;
using System.Collections.Generic;
using System.IO;
using AdventureGame.Core.Quests;
using AdventureGame.Entities.Player;
using UnityEngine;

namespace AdventureGame.Core.Managers
{
    public class SaveManager : PersistentSingleton<SaveManager>
    {
        private const string k_SaveFileName = "save.json";
        private const int k_CurrentSaveVersion = 4;
        private const int k_NoStoredHealth = -1;

        public bool HasSave { get; private set; }
        public Save Current { get; private set; }

        public event Action SaveDeleted;

        private string SavePath => Path.Combine(Application.persistentDataPath, k_SaveFileName);

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
                if (loaded?.Version != k_CurrentSaveVersion)
                    return;

                Current = loaded;
                HasSave = true;
            }
            catch { }
        }

        public void WriteSave(string sceneName)
        {
            List<QuestSaveData> quests =
                QuestManager.Instance != null
                    ? QuestManager.Instance.CaptureAll()
                    : PreserveQuestsFromCurrent();

            Save save = new Save
            {
                Version = k_CurrentSaveVersion,
                SceneName = sceneName,
                PlayerHealth =
                    Player.Instance != null ? Player.Instance.CurrentHealth : k_NoStoredHealth,
                Quests = quests,
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
            if (Current?.Quests == null)
                return new List<QuestSaveData>();

            List<QuestSaveData> copy = new();

            foreach (QuestSaveData q in Current.Quests)
                copy.Add(
                    new QuestSaveData
                    {
                        QuestId = q.QuestId,
                        State = q.State,
                        ConsumedIds =
                            q.ConsumedIds != null ? new List<string>(q.ConsumedIds) : null,
                    }
                );

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
