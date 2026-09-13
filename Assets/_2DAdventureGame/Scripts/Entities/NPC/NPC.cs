using System.Collections.Generic;
using AdventureGame.Core.Dialogue;
using AdventureGame.Core.Quest;
using AdventureGame.UI;
using UnityEngine;

namespace AdventureGame.Entities.NPC
{
    public class NPC : MonoBehaviour
    {
        [SerializeField] private List<QuestDialogue> dialogues;

        private DialoguePhase currentPhase;
        private QuestDialogue currentDialogue;
        private int lineIndex;

        public void Talk()
        {
            if (currentPhase == null)
            {
                foreach (QuestDialogue dialogue in dialogues)
                {
                    QuestController controller = QuestManager.Instance != null ? QuestManager.Instance.Get(dialogue.quest) : null;
                    DialoguePhase phase = dialogue.Pick(controller);
                    if (phase != null)
                    {
                        currentDialogue = dialogue;
                        currentPhase = phase;

                        break;
                    }
                }

                if (currentPhase == null) 
                    return;
            
                lineIndex = 0;

                if (currentDialogue.IsAfterPhase(currentPhase))
                {
                    QuestController controller = QuestManager.Instance != null ? QuestManager.Instance.Get(currentDialogue.quest) : null;
                    if (controller != null) 
                        controller.Conclude();
                }
            }

            DialoguePresenter.Instance.DisplayDialogueWithLine(currentPhase.lines[lineIndex++], transform);

            if (lineIndex >= currentPhase.lines.Count)
            {
                if (currentPhase.questToGrantAfter != null)
                {
                    QuestController controller = QuestManager.Instance != null ? QuestManager.Instance.Get(currentPhase.questToGrantAfter) : null;
                    if (controller != null) 
                        controller.Accept();
                }

                if (currentDialogue.IsAfterPhase(currentPhase))
                {
                    QuestController controller = QuestManager.Instance != null ? QuestManager.Instance.Get(currentDialogue.quest) : null;
                    if (controller != null) 
                        controller.EpilogueFinished();
                }

                currentPhase.onExhausted?.Invoke();
                currentPhase = null;
                currentDialogue = null;
            }
        }
    }
}
