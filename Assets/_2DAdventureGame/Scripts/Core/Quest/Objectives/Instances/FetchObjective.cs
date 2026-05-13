using UnityEngine;

[CreateAssetMenu(fileName = "FetchObjective", menuName = "Scriptable Objects/FetchObjective")]
public class FetchObjective : QuestObjective
{
    public override QuestObjectiveType Type => QuestObjectiveType.Fetch;

    public string item;
    public int count;
}
