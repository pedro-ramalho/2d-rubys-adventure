using System;

public enum QuestObjectiveType { Fetch, Kill }

[Serializable]
public class QuestObjective
{
    public QuestObjectiveType type;
    public string targetTag;
    public int count;

    public bool Matches(QuestReport report) => report.Type == type && report.Tag == targetTag;
}
