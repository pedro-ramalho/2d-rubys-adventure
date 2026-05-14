using UnityEngine;

[CreateAssetMenu(fileName = "FetchObjective", menuName = "Scriptable Objects/FetchObjective")]
public class FetchObjective : QuestObjective
{
    private int count;
    private string item;
    
    public override QuestObjectiveType Type => QuestObjectiveType.Fetch;
    public override int Count => count;

    public override bool Matches(QuestReport report) => report.Type == Type && report.Tag == item; 
}
