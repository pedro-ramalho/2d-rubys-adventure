using System;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    public Quest ActiveQuest { get; private set; }
    private readonly HashSet<QuestData> completed = new();

    public event Action<Quest> OnQuestAccepted;
    public event Action<Quest> OnQuestCompleted;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void AcceptQuest(QuestData data)
    {
        Quest quest = new Quest(data);
        ActiveQuest = quest;
        OnQuestAccepted?.Invoke(quest);
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
        }
    }

    public bool IsCompleted(QuestData data) => completed.Contains(data);
}
