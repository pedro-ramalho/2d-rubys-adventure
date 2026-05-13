using UnityEngine;

[CreateAssetMenu(fileName = "QuestObjective", menuName = "Game/Quest Objective/")]
public abstract class QuestObjective : ScriptableObject
{
    public abstract bool Matches(QuestReport report);
}
