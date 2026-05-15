using UnityEngine;

[CreateAssetMenu(fileName = "FetchObjective", menuName = "Game/Quests/Objectives/Fetch Objective")]
public class FetchObjective : QuestObjective
{
    [SerializeField] private int count;
    [SerializeField] private string item;
    
    public override QuestObjectiveType Type => QuestObjectiveType.Fetch;
    public override int Count => count;

    public override bool Matches(QuestReport report) => report.Type == Type && report.Tag == item; 
}
