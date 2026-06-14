public readonly struct QuestReport
{
    public readonly QuestObjectiveType Type;
    public readonly string Tag;
    public readonly string WorldId;

    public QuestReport(QuestObjectiveType type, string tag, string worldId = null)
    {
        Type = type;
        Tag = tag;
        WorldId = worldId;
    }
}
