using AdventureGame.Core.Dialogue;
using UnityEngine;

namespace AdventureGame.Core.Quests
{
    [CreateAssetMenu(fileName = "QuestDialogue", menuName = "Game/Quest Dialogue")]
    public class QuestDialogue : ScriptableObject
    {
        public QuestDefinition Quest;

        public DialoguePhase Before;

        public DialoguePhase During;

        public DialoguePhase After;

        public DialoguePhase Pick(Quest quest)
        {
            QuestState state = quest != null ? quest.State : QuestState.Inactive;

            DialoguePhase chosen = state switch
            {
                QuestState.Complete => After,
                QuestState.Active => During,
                _ => Before,
            };

            return chosen != null && !chosen.IsEmpty ? chosen : null;
        }

        public bool IsEpilogue(DialoguePhase phase) => phase == After && Quest != null;
    }
}
