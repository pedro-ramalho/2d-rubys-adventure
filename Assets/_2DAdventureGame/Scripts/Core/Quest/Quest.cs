using UnityEngine;

public class Quest
{
    public QuestData Data { get; }
    public int Count { get; private set; }

    public Quest(QuestData data) => Data = data;

    public bool IsComplete => Count >= Data.objective.Count;

    public void ApplyProgress(QuestReport report)
    {
        if (IsComplete) return;

        if (Data.objective.Matches(report)) Count++;
    }
}
