using AdventureGame.Core.Dialogue;
using UnityEngine;
using UnityEngine.Serialization;

namespace AdventureGame.Core.Quests
{
    [CreateAssetMenu(fileName = "QuestDialogue", menuName = "Game/Quest Dialogue")]
    public class QuestDialogue : ScriptableObject
    {
        public QuestDefinition Quest;

        [FormerlySerializedAs("Before")]
        public DialoguePhase Inactive;

        [FormerlySerializedAs("During")]
        public DialoguePhase Active;

        [FormerlySerializedAs("After")]
        public DialoguePhase Complete;

        public DialoguePhase Concluded;

        public DialoguePhase Pick(Quest quest)
        {
            QuestState state = quest != null ? quest.State : QuestState.Inactive;

            DialoguePhase chosen = state switch
            {
                QuestState.Concluded => Concluded,
                QuestState.Complete => Complete,
                QuestState.Active => Active,
                _ => Inactive,
            };

            return chosen != null && !chosen.IsEmpty ? chosen : null;
        }

        public bool IsEpilogue(DialoguePhase phase) => phase == Complete && Quest != null;
    }
}
