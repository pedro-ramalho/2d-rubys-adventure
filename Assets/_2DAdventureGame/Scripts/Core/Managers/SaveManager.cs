using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveManager : PersistentSingleton<SaveManager>
{
    private const string SaveFileName = "save.json";
    private const int CurrentSaveVersion = 3;
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
        if (Instance != this) return;
        ReadFromDisk();
    }

    void ReadFromDisk()
    {
        HasSave = false;
        if (!File.Exists(SavePath)) return;

        try
        {
            Save loaded = JsonUtility.FromJson<Save>(File.ReadAllText(SavePath));
            if (loaded?.version != CurrentSaveVersion) return;

            Current = loaded;
            HasSave = true;
        }
        catch { }
    }

    public void WriteSave(string sceneName)
    {
        string activeQuestId = string.Empty;
        int activeQuestCount = 0;
        if (QuestManager.Instance != null && QuestManager.Instance.ActiveQuest != null)
        {
            activeQuestId = QuestManager.Instance.ActiveQuest.Data.id;
            activeQuestCount = QuestManager.Instance.ActiveQuest.Count;
        }

        HashSet<string> mergedCompleted = new();
        if (Current != null && Current.completedQuestIds != null)
            foreach (string id in Current.completedQuestIds)
                mergedCompleted.Add(id);
        if (QuestManager.Instance != null)
            foreach (string id in QuestManager.Instance.GetCompletedQuestIds())
                mergedCompleted.Add(id);

        Save save = new Save
        {
            version = CurrentSaveVersion,
            sceneName = sceneName,
            playerHealth = Player.Instance != null ? Player.Instance.CurrentHealth : NoStoredHealth,
            activeQuestId = activeQuestId,
            activeQuestCount = activeQuestCount,
            completedQuestIds = new List<string>(mergedCompleted)
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

    public void DeleteSave()
    {
        if (File.Exists(SavePath))
            File.Delete(SavePath);
        Current = null;
        HasSave = false;
        SaveDeleted?.Invoke();
    }
}
