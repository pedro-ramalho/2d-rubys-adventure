using AdventureGame.Core.Dialogue;
using UnityEngine;

namespace AdventureGame.Core.Quests
{
    [CreateAssetMenu(fileName = "QuestDialogue", menuName = "Game/Quest Dialogue")]
    public class QuestDialogue : ScriptableObject
    {
        public QuestData Quest;

        public DialoguePhase Before;

        public DialoguePhase During;

        public DialoguePhase After;

        public DialoguePhase Pick(Quest quest)
        {
            QuestPhase phase = quest != null ? quest.Phase : QuestPhase.Before;

            DialoguePhase chosen = phase switch
            {
                QuestPhase.After => After,
                QuestPhase.During => During,
                _ => Before,
            };

            return chosen != null && !chosen.IsEmpty ? chosen : null;
        }

        public bool IsEpilogue(DialoguePhase phase) => phase == After && Quest != null;
    }
}
