using UnityEngine;

public class Quest
{
    public QuestData Data { get; }
    public int Count { get; private set; }

    public Quest(QuestData data, int initialCount = 0)
    {
        Data = data;
        Count = initialCount;
    }

    public bool IsComplete => Count >= Data.objective.count;

    public void ApplyProgress(QuestReport report)
    {
        if (IsComplete) return;

        if (Data.objective.Matches(report)) Count++;
    }
}
