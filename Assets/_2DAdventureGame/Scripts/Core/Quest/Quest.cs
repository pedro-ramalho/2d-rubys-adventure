using UnityEngine;

public class Quest
{
    private int count;

    public QuestData Data { get; }

    public Quest(QuestData data) => Data = data;

    public bool IsComplete => count >= Data.objective.Count;

    public void ApplyProgress(QuestReport report)
    {
        if (IsComplete) return;

        if (Data.objective.Matches(report)) count++;
    }
}
