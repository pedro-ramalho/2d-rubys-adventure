using UnityEngine;

public enum QuestObjectiveType { Fetch, Kill }

[CreateAssetMenu(fileName = "QuestObjective", menuName = "Scriptable Objects/Quest Objective")]
public abstract class QuestObjective : ScriptableObject
{
    public abstract QuestObjectiveType Type { get; }
    public abstract int Count { get; }
    public abstract bool Matches(QuestReport report);
}
