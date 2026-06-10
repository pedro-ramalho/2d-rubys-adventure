using System;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    [SerializeField] private AudioClip questCompletionSfx;
    [SerializeField] private QuestData[] knownQuests;

    public Quest ActiveQuest { get; private set; }
    private readonly HashSet<QuestData> completed = new();
    private readonly Dictionary<string, QuestData> questsById = new();

    public event Action<Quest> OnQuestAccepted;
    public event Action<Quest> OnQuestCompleted;
    public event Action<QuestData> OnQuestConcluded;
    public event Action<QuestData> OnQuestEpilogueFinished;

    void Awake()
    {
        if (Instance == null) Instance = this;
        BuildQuestLookup();
    }

    void Start()
    {
        if (SaveManager.Instance != null && SaveManager.Instance.HasSave)
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
                    completed.Add(data);
        }

        if (!string.IsNullOrEmpty(save.activeQuestId) && questsById.TryGetValue(save.activeQuestId, out QuestData activeData))
            RestoreActiveQuest(activeData, save.activeQuestCount);
    }

    public void AcceptQuest(QuestData data) => StartQuest(data, initialCount: 0, raiseAccepted: true);

    private void RestoreActiveQuest(QuestData data, int count) => StartQuest(data, count, raiseAccepted: false);

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

    public void SubmitReport(QuestReport report)
    {
        if (ActiveQuest == null || ActiveQuest.IsComplete) 
            return;

        ActiveQuest.ApplyProgress(report);

        if (ActiveQuest.IsComplete) 
            CompleteActiveQuest();
    }

    private void CompleteActiveQuest()
    {
        Quest finished = ActiveQuest;
        completed.Add(finished.Data);

        ActiveQuest = null;

        OnQuestCompleted?.Invoke(finished);

        AudioClip defaultTrack = SceneMusicConfigManager.Instance?.DefaultTrack;
        if (MusicManager.Instance != null && defaultTrack != null)
            MusicManager.Instance.PlayWithStinger(questCompletionSfx, defaultTrack);
    }

    public bool IsCompleted(QuestData data) => completed.Contains(data);

    public void ConcludeQuest(QuestData data) => OnQuestConcluded?.Invoke(data);

    public void NotifyEpilogueFinished(QuestData data) => OnQuestEpilogueFinished?.Invoke(data);
}
