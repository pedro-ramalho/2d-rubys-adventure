public readonly struct QuestReport
{
    public readonly QuestObjectiveType Type;
    public readonly string Tag;

    public QuestReport(QuestObjectiveType type, string tag)
    {
        Type = type;
        Tag = tag;
    }
}
