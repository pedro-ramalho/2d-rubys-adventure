using System.Collections.Generic;

public class CountedQuestController : QuestController
{
    private readonly HashSet<string> consumedIds = new();

    public int Count => consumedIds.Count;
    public int Target => data != null ? data.targetCount : 0;

    public bool IsConsumed(string worldId) =>
        !string.IsNullOrEmpty(worldId) && consumedIds.Contains(worldId);

    public bool CanReport(string worldId) =>
        Phase == QuestPhase.During &&
        !string.IsNullOrEmpty(worldId) &&
        !consumedIds.Contains(worldId);

    public bool TryReport(string worldId)
    {
        if (!CanReport(worldId)) 
            return false;
        
        consumedIds.Add(worldId);

        if (consumedIds.Count >= Target)
            MarkComplete();

        return true;
    }

    protected override List<string> CaptureConsumed() => new(consumedIds);

    protected override void RestoreData(QuestSaveData saved)
    {
        consumedIds.Clear();

        if (saved.consumedIds == null) 
            return;

        foreach (string id in saved.consumedIds)
            if (!string.IsNullOrEmpty(id))
                consumedIds.Add(id);
    }
}
