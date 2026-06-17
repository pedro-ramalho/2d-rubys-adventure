using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    private readonly Dictionary<string, QuestController> controllers = new();

    void Awake()
    {
        if (Instance == null) 
            Instance = this;
    }

    void Start()
    {
        if (SaveManager.Instance != null && SaveManager.Instance.HasSave)
            RestoreFromSave(SaveManager.Instance.Current);
    }

    public void Register(QuestController controller)
    {
        if (controller == null || controller.Data == null || string.IsNullOrEmpty(controller.Data.id))
            return;
        
        controllers[controller.Data.id] = controller;
    }

    public void Unregister(QuestController controller)
    {
        if (controller == null || controller.Data == null) 
            return;
        
        if (controllers.TryGetValue(controller.Data.id, out QuestController current) && current == controller)
            controllers.Remove(controller.Data.id);
    }

    public QuestController Get(string questId) =>
        !string.IsNullOrEmpty(questId) && controllers.TryGetValue(questId, out QuestController c) ? c : null;

    public QuestController Get(QuestData questData) =>
        questData != null ? Get(questData.id) : null;

    public IEnumerable<QuestController> All => controllers.Values;

    public List<QuestSaveData> CaptureAll()
    {
        List<QuestSaveData> list = new();

        foreach (QuestController controller in controllers.Values)
            list.Add(controller.Capture());
        
        return list;
    }

    void RestoreFromSave(Save save)
    {
        if (save?.quests == null) 
            return;
        
        foreach (QuestSaveData saved in save.quests)
            if (saved != null && controllers.TryGetValue(saved.questId, out QuestController controller))
                controller.Restore(saved);
    }
}
