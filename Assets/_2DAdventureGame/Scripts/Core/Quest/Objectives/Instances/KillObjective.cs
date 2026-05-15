using UnityEngine;

[CreateAssetMenu(fileName = "KillObjective", menuName = "Scriptable Objects/KillObjective")]
public class KillObjective : QuestObjective
{
    [SerializeField] private int count;
    [SerializeField] private string enemy;
    
    public override QuestObjectiveType Type => QuestObjectiveType.Kill;
    public override int Count => count;

    public override bool Matches(QuestReport report) => report.Type == Type && report.Tag == enemy;
}
