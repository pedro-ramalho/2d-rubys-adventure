using System.Collections.Generic;
using UnityEngine;

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
                DialoguePhase phase = dialogue.Pick(QuestManager.Instance);
                if (phase != null)
                {
                    currentDialogue = dialogue;
                    currentPhase = phase;
                    break;
                }
            }

            if (currentPhase == null) return;
            lineIndex = 0;

            if (currentDialogue.IsAfterPhase(currentPhase))
                QuestManager.Instance.ConcludeQuest(currentDialogue.quest);
        }

        UIHandler.Instance.DisplayDialogueWithLine(currentPhase.lines[lineIndex++]);

        if (lineIndex >= currentPhase.lines.Count)
        {
            if (currentPhase.questToGrantAfter != null)
                QuestManager.Instance.AcceptQuest(currentPhase.questToGrantAfter);

            if (currentPhase == currentDialogue.after && currentDialogue.quest != null)
                QuestManager.Instance.RaiseQuestEpilogueFinished(currentDialogue.quest);

            currentPhase.onExhausted?.Invoke();
            currentPhase = null;
            currentDialogue = null;
        }
    }
}
