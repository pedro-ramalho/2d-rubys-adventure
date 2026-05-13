using UnityEngine;

[CreateAssetMenu(fileName = "FetchObjective", menuName = "Scriptable Objects/FetchObjective")]
public class FetchObjective : QuestObjective
{
    public string item;
    public int count;

    public override bool Matches(QuestReport report) => report.type == ReportType.Fetch && report.tag == item;
}
