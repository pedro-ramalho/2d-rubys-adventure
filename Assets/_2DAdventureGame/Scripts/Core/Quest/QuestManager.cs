using System;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    [SerializeField] private AudioClip questCompletionSfx;
    [SerializeField] private QuestData[] knownQuests;

    public Quest ActiveQuest { get; private set; }
    private readonly HashSet<QuestData> completed = new();
    private readonly HashSet<string> consumedWorldIds = new();
    private readonly Dictionary<string, QuestData> questsById = new();

    public event Action<Quest> OnQuestAccepted;
    public event Action<Quest> OnQuestCompleted;
    public event Action<QuestData> OnQuestConcluded;
    public event Action<QuestData> OnQuestEpilogueFinished;

    void Awake()
    {
        if (Instance == null) Instance = this;
        BuildQuestLookup();
        Debug.Log($"[QM] Awake in scene='{gameObject.scene.name}'. Known quests=[{string.Join(",", questsById.Keys)}]");
    }

    void Start()
    {
        bool hasSave = SaveManager.Instance != null && SaveManager.Instance.HasSave;
        Debug.Log($"[QM] Start. HasSave={hasSave}");
        if (hasSave)
            RestoreFromSave(SaveManager.Instance.Current);
    }

    void BuildQuestLookup()
    {
        if (knownQuests == null) return;
        foreach (QuestData quest in knownQuests)
            if (quest != null && !string.IsNullOrEmpty(quest.id))
                questsById[quest.id] = quest;
    }

    void RestoreFromSave(Save save)
    {
        if (save.completedQuestIds != null)
        {
            foreach (string id in save.completedQuestIds)
                if (questsById.TryGetValue(id, out QuestData data))
                {
                    completed.Add(data);
                    if (AbilityManager.Instance != null)
                        AbilityManager.Instance.Unlock(data.unlockOnAccept);
                }
        }

        if (save.consumedWorldIds != null)
        {
            foreach (string worldId in save.consumedWorldIds)
                if (!string.IsNullOrEmpty(worldId))
                    consumedWorldIds.Add(worldId);
        }

        Debug.Log($"[QM] Restored: completed=[{string.Join(",", GetCompletedQuestIds())}], consumed=[{string.Join(",", consumedWorldIds)}], save.activeId='{save.activeQuestId}' save.count={save.activeQuestCount}");

        if (!string.IsNullOrEmpty(save.activeQuestId) && questsById.TryGetValue(save.activeQuestId, out QuestData activeData))
            RestoreActiveQuest(activeData, save.activeQuestCount);
    }

    public void AcceptQuest(QuestData data)
    {
        Debug.Log($"[QM] AcceptQuest('{data?.id}')");
        StartQuest(data, initialCount: 0, raiseAccepted: true);
    }

    private void RestoreActiveQuest(QuestData data, int count)
    {
        Debug.Log($"[QM] RestoreActiveQuest('{data.id}', count={count})");
        StartQuest(data, count, raiseAccepted: false);
    }

    private void StartQuest(QuestData data, int initialCount, bool raiseAccepted)
    {
        ActiveQuest = new Quest(data, initialCount);
        if (raiseAccepted) OnQuestAccepted?.Invoke(ActiveQuest);

        if (AbilityManager.Instance != null)
            AbilityManager.Instance.Unlock(data.unlockOnAccept);

        if (MusicManager.Instance != null && data.backgroundTrack != null)
            MusicManager.Instance.Play(data.backgroundTrack);
    }

    public IEnumerable<string> GetCompletedQuestIds()
    {
        foreach (QuestData data in completed)
            yield return data.id;
    }

    public IEnumerable<string> GetConsumedWorldIds() => consumedWorldIds;

    public bool IsConsumed(string worldId) =>
        !string.IsNullOrEmpty(worldId) && consumedWorldIds.Contains(worldId);

    public void SubmitReport(QuestReport report)
    {
        Debug.Log($"[QM] SubmitReport type={report.Type} tag='{report.Tag}' worldId='{report.WorldId}' (active='{ActiveQuest?.Data.id}' count={ActiveQuest?.Count})");

        if (ActiveQuest == null || ActiveQuest.IsComplete)
        {
            Debug.Log("[QM] SubmitReport: ignored (no active quest or already complete)");
            return;
        }

        if (!string.IsNullOrEmpty(report.WorldId) && !consumedWorldIds.Add(report.WorldId))
        {
            Debug.Log($"[QM] SubmitReport: worldId '{report.WorldId}' already consumed, ignored.");
            return;
        }

        ActiveQuest.ApplyProgress(report);
        Debug.Log($"[QM] After ApplyProgress: count={ActiveQuest.Count}/{ActiveQuest.Data.objective.count}");

        if (ActiveQuest.IsComplete)
            CompleteActiveQuest();
    }

    private void CompleteActiveQuest()
    {
        Quest finished = ActiveQuest;
        completed.Add(finished.Data);

        ActiveQuest = null;

        Debug.Log($"[QM] CompleteActiveQuest('{finished.Data.id}')");

        OnQuestCompleted?.Invoke(finished);

        AudioClip defaultTrack = SceneMusicConfigManager.Instance?.DefaultTrack;
        if (MusicManager.Instance == null)
            return;

        if (defaultTrack != null)
            MusicManager.Instance.PlayWithStinger(questCompletionSfx, defaultTrack);
        else
            MusicManager.Instance.FadeOutAndStop(5f);
    }

    public bool IsCompleted(QuestData data) => completed.Contains(data);

    public void ConcludeQuest(QuestData data)
    {
        Debug.Log($"[QM] ConcludeQuest('{data?.id}')");
        OnQuestConcluded?.Invoke(data);
    }

    public void NotifyEpilogueFinished(QuestData data) => OnQuestEpilogueFinished?.Invoke(data);
}
