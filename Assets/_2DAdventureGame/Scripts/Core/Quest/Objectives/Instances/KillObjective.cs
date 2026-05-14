using UnityEngine;

[CreateAssetMenu(fileName = "KillObjective", menuName = "Scriptable Objects/KillObjective")]
public class KillObjective : QuestObjective
{
    private int count;
    private string enemy;
    
    public override QuestObjectiveType Type => QuestObjectiveType.Kill;
    public override int Count => count;

    public override bool Matches(QuestReport report) => report.Type == Type && report.Tag == enemy;
}
