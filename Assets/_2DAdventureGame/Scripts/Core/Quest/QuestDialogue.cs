using UnityEngine;

[CreateAssetMenu(fileName = "QuestDialogue", menuName = "Game/Quest Dialogue")]
public class QuestDialogue : ScriptableObject
{
    public QuestData quest;

    public DialoguePhase before;

    public DialoguePhase during;

    public DialoguePhase after;

    public DialoguePhase Pick(QuestController controller)
    {
        QuestPhase phase = controller != null ? controller.Phase : QuestPhase.Before;

        DialoguePhase chosen = phase switch
        {
            QuestPhase.After  => after,
            QuestPhase.During => during,
            _                 => before
        };

        return chosen != null && !chosen.IsEmpty ? chosen : null;
    }

    public bool IsAfterPhase(DialoguePhase phase) => phase == after && quest != null;
}
