using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

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
        if (Instance != this) return;
        Debug.Log($"[Save] Awake. Path={SavePath}");
        ReadFromDisk();
    }

    void ReadFromDisk()
    {
        HasSave = false;
        if (!File.Exists(SavePath))
        {
            Debug.Log("[Save] ReadFromDisk: no save file.");
            return;
        }

        try
        {
            string json = File.ReadAllText(SavePath);
            Save loaded = JsonUtility.FromJson<Save>(json);
            if (loaded?.version != CurrentSaveVersion)
            {
                Debug.Log($"[Save] ReadFromDisk: version mismatch (loaded={loaded?.version}, expected={CurrentSaveVersion}). Discarding.");
                return;
            }

            Current = loaded;
            HasSave = true;
            Debug.Log($"[Save] ReadFromDisk: loaded scene='{loaded.sceneName}', hp={loaded.playerHealth}, active='{loaded.activeQuestId}' count={loaded.activeQuestCount}, completed=[{Join(loaded.completedQuestIds)}], consumed=[{Join(loaded.consumedWorldIds)}]");
        }
        catch (Exception e)
        {
            Debug.LogError($"[Save] ReadFromDisk: exception {e.Message}");
        }
    }

    public void WriteSave(string sceneName)
    {
        string activeQuestId = string.Empty;
        int activeQuestCount = 0;

        if (QuestManager.Instance != null && QuestManager.Instance.ActiveQuest != null)
        {
            Quest active = QuestManager.Instance.ActiveQuest;
            bool keep = active.IsComplete || !active.Data.resetIfActiveOnReload;
            Debug.Log($"[Save] WriteSave: active quest '{active.Data.id}' count={active.Count} complete={active.IsComplete} resetFlag={active.Data.resetIfActiveOnReload} keep={keep}");
            if (keep)
            {
                activeQuestId = active.Data.id;
                activeQuestCount = active.Count;
            }
        }
        else
        {
            Debug.Log($"[Save] WriteSave: no live active quest (QM.Instance={(QuestManager.Instance != null ? "alive" : "null")})");
        }

        HashSet<string> mergedCompleted = new();
        if (Current != null && Current.completedQuestIds != null)
            foreach (string id in Current.completedQuestIds)
                mergedCompleted.Add(id);
        if (QuestManager.Instance != null)
            foreach (string id in QuestManager.Instance.GetCompletedQuestIds())
                mergedCompleted.Add(id);

        List<string> consumed = new();
        if (QuestManager.Instance != null)
            foreach (string id in QuestManager.Instance.GetConsumedWorldIds())
                consumed.Add(id);
        else if (Current != null && Current.consumedWorldIds != null)
            consumed.AddRange(Current.consumedWorldIds);

        Save save = new Save
        {
            version = CurrentSaveVersion,
            sceneName = sceneName,
            playerHealth = Player.Instance != null ? Player.Instance.CurrentHealth : NoStoredHealth,
            activeQuestId = activeQuestId,
            activeQuestCount = activeQuestCount,
            completedQuestIds = new List<string>(mergedCompleted),
            consumedWorldIds = consumed
        };

        Debug.Log($"[Save] WriteSave to scene='{sceneName}': hp={save.playerHealth}, active='{activeQuestId}' count={activeQuestCount}, completed=[{Join(save.completedQuestIds)}], consumed=[{Join(save.consumedWorldIds)}]");

        try
        {
            string json = JsonUtility.ToJson(save, true);
            File.WriteAllText(SavePath, json);
            Current = save;
            HasSave = true;
        }
        catch (Exception e)
        {
            Debug.LogError($"[Save] WriteSave: exception {e.Message}");
        }
    }

    public void DeleteSave()
    {
        bool existed = File.Exists(SavePath);
        if (existed)
            File.Delete(SavePath);
        Current = null;
        HasSave = false;
        Debug.Log($"[Save] DeleteSave: file existed={existed}, Current cleared.");
        SaveDeleted?.Invoke();
    }

    static string Join(List<string> items) => items == null ? "" : string.Join(",", items);
}
