using UnityEngine;

public class QuestReporter : MonoBehaviour
{
    [SerializeField] private QuestObjectiveType type;
    [SerializeField] private string reportTag;

    public void Report() => QuestManager.Instance?.SubmitReport(new QuestReport(type, reportTag));
}
