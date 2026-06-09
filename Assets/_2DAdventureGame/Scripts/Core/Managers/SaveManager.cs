using System.IO;
using UnityEngine;

public class SaveManager : PersistentSingleton<SaveManager>
{
    private const string SaveFileName = "save.json";
    private const int CurrentSaveVersion = 2;
    private const int NoStoredHealth = -1;

    public bool HasSave { get; private set; }
    public Save Current { get; private set; }

    private string SavePath => Path.Combine(Application.persistentDataPath, SaveFileName);

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Bootstrap()
    {
        if (Instance != null) return;
        GameObject go = new GameObject("SaveManager");
        go.AddComponent<SaveManager>();
    }

    protected override void Awake()
    {
        base.Awake();
        if (Instance != this) return;
        ReadFromDisk();
    }

    void ReadFromDisk()
    {
        if (!File.Exists(SavePath))
        {
            HasSave = false;
            return;
        }

        try
        {
            string json = File.ReadAllText(SavePath);
            Save loaded = JsonUtility.FromJson<Save>(json);
            if (loaded == null || loaded.version != CurrentSaveVersion)
            {
                HasSave = false;
                return;
            }
            Current = loaded;
            HasSave = true;
        }
        catch
        {
            HasSave = false;
        }
    }

    public void Save(string sceneName)
    {
        Save save = new Save
        {
            version = CurrentSaveVersion,
            sceneName = sceneName,
            playerHealth = Player.Instance != null ? Player.Instance.CurrentHealth : NoStoredHealth
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
    }
}
