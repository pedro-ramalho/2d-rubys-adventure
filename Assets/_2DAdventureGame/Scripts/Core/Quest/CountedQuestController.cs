using System.Collections.Generic;

public class CountedQuestController : QuestController
{
    private readonly HashSet<string> consumedIds = new();

    public int Count => consumedIds.Count;
    public int Target => data != null ? data.objective.count : 0;

    public bool IsConsumed(string worldId) =>
        !string.IsNullOrEmpty(worldId) && consumedIds.Contains(worldId);

    public bool CanReport(string worldId) =>
        Phase == QuestPhase.During &&
        !string.IsNullOrEmpty(worldId) &&
        !consumedIds.Contains(worldId);

    public bool TryReport(string worldId)
    {
        if (!CanReport(worldId)) return false;
        consumedIds.Add(worldId);

        if (consumedIds.Count >= Target)
            MarkComplete();

        return true;
    }

    public override QuestSaveData Capture() => new QuestSaveData
    {
        questId = data.id,
        phase = Phase,
        consumedIds = new List<string>(consumedIds)
    };

    public override void Restore(QuestSaveData saved)
    {
        consumedIds.Clear();
        if (saved.consumedIds != null)
            foreach (string id in saved.consumedIds)
                if (!string.IsNullOrEmpty(id))
                    consumedIds.Add(id);

        SetPhase(saved.phase);

        if (saved.phase != QuestPhase.Before)
            ApplyUnlock();
    }
}
