using UnityEngine;

[CreateAssetMenu(fileName = "FetchObjective", menuName = "Scriptable Objects/Kill Objective")]
public class KillObjective : QuestObjective
{
    public string enemy;
    public int count;

    public override bool Matches(QuestReport report) => report.type == ReportType.Kill && report.tag == enemy;
}
