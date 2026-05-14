using UnityEngine;

public enum QuestObjectiveType { Fetch, Kill }

public abstract class QuestObjective : ScriptableObject
{
    public abstract QuestObjectiveType Type { get; }
    public abstract int Count { get; }
    public abstract bool Matches(QuestReport report);
}
