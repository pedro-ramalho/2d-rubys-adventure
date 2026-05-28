using System;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    [SerializeField] private AudioClip questCompletionSfx;

    public Quest ActiveQuest { get; private set; }
    private readonly HashSet<QuestData> completed = new();

    public event Action<Quest> OnQuestAccepted;
    public event Action<Quest> OnQuestCompleted;
    public event Action<QuestData> OnQuestConcluded;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void AcceptQuest(QuestData data)
    {
        Quest quest = new Quest(data);
        ActiveQuest = quest;
        OnQuestAccepted?.Invoke(quest);

        if (AbilityManager.Instance != null)
            AbilityManager.Instance.Unlock(data.unlockOnAccept);

        if (MusicManager.Instance != null && data.backgroundTrack != null)
            MusicManager.Instance.Play(data.backgroundTrack);
    }

    public void Report(QuestReport report)
    {
        if (ActiveQuest == null || ActiveQuest.IsComplete) return;

        ActiveQuest.ApplyProgress(report);

        if (ActiveQuest.IsComplete)
        {
            Quest finished = ActiveQuest;
            completed.Add(finished.Data);
            ActiveQuest = null;
            OnQuestCompleted?.Invoke(finished);

            if (MusicManager.Instance != null && SceneMusicConfigManager.Instance != null && SceneMusicConfigManager.Instance.DefaultTrack != null)
            {
                if (questCompletionSfx != null)
                    MusicManager.Instance.PlayWithStinger(questCompletionSfx, SceneMusicConfigManager.Instance.DefaultTrack);
                else
                    MusicManager.Instance.Play(SceneMusicConfigManager.Instance.DefaultTrack);
            }
        }
    }

    public bool IsCompleted(QuestData data) => completed.Contains(data);

    public void ConcludeQuest(QuestData data) => OnQuestConcluded?.Invoke(data);
}
