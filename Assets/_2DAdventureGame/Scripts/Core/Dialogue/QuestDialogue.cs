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

    public DialoguePhase Pick(QuestManager manager)
    {
        DialoguePhase phase =
            manager.IsCompleted(quest)         ? after  :
            manager.ActiveQuest?.Data == quest ? during :
                                                 before;

        return phase != null && !phase.IsEmpty ? phase : null;
    }

    public bool IsAfterPhase(DialoguePhase phase) => phase == after && quest != null;
}