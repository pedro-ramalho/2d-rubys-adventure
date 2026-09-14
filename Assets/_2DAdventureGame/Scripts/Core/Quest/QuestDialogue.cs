using AdventureGame.Core.Dialogue;
using UnityEngine;

namespace AdventureGame.Core.Quest
{
    [CreateAssetMenu(fileName = "QuestDialogue", menuName = "Game/Quest Dialogue")]
    public class QuestDialogue : ScriptableObject
    {
        public QuestData Quest;

        public DialoguePhase Before;

        public DialoguePhase During;

        public DialoguePhase After;

        public DialoguePhase Pick(QuestController controller)
        {
            QuestPhase phase = controller != null ? controller.Phase : QuestPhase.Before;

            DialoguePhase chosen = phase switch
            {
                QuestPhase.After  => After,
                QuestPhase.During => During,
                _                 => Before
            };

            return chosen != null && !chosen.IsEmpty ? chosen : null;
        }

        public bool IsAfterPhase(DialoguePhase phase) => phase == After && Quest != null;
    }
}
