using UnityEngine;

public class QuestReporter
{
    [SerializeField] private QuestObjectiveType type;
    [SerializeField] private string tag;

    public void Report() => QuestManager.Instance?.Report(new QuestReport(type, tag));
}
