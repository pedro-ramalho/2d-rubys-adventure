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

        UIHandler.Instance.DisplayDialogueWithLine(currentPhase.lines[lineIndex++], transform);

        if (lineIndex >= currentPhase.lines.Count)
        {
            if (currentPhase.questToGrantAfter != null)
                QuestManager.Instance.AcceptQuest(currentPhase.questToGrantAfter);

            if (currentDialogue.IsAfterPhase(currentPhase))
                QuestManager.Instance.NotifyEpilogueFinished(currentDialogue.quest);

            currentPhase.onExhausted?.Invoke();
            currentPhase = null;
            currentDialogue = null;
        }
    }
}
