using UnityEngine;

[CreateAssetMenu(fileName = "QuestDialogue", menuName = "Game/Quest Dialogue")]
public class QuestDialogue : ScriptableObject
{
    [Tooltip("The quest associated with this dialogue.")]
    public QuestData quest;

    [Tooltip("Dialogue lines that will be displayed before the associated quest is accepted by the Player.")]
    public DialoguePhase before;

    [Tooltip("Dialogue lines that will be displayed whilst the associated quest is active.")]
    public DialoguePhase during;

    [Tooltip("Dialogue lines that will be displayed once the associated quest is completed.")]
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
