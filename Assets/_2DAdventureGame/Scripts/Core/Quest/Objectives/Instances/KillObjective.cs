using UnityEngine;

[CreateAssetMenu(fileName = "KillObjective", menuName = "Scriptable Objects/KillObjective")]
public class KillObjective : QuestObjective
{
    public override QuestObjectiveType Type => QuestObjectiveType.Kill;

    public string enemy;
    public int count;
}
