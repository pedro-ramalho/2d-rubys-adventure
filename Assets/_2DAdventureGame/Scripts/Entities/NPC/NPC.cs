using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    [SerializeField] private List<QuestDialogue> dialogues;
    [SerializeField] private GameObject dialogueBubble;

    private DialoguePhase currentPhase;
    private int lineIndex;

    void Start() => dialogueBubble.SetActive(false);

    public void SetBubbleVisible(bool visible) => dialogueBubble.SetActive(visible);

    public void Talk()
    {
        if (currentPhase == null)
        {
            foreach (QuestDialogue dialogue in dialogues)
            {
                DialoguePhase phase = dialogue.Pick(QuestManager.Instance);
                if (phase != null)
                {
                    currentPhase = phase;
                    break;
                }
            }

            if (currentPhase == null) return;
            lineIndex = 0;
        }

        UIHandler.Instance.DisplayDialogueWithLine(currentPhase.lines[lineIndex++]);

        if (lineIndex >= currentPhase.lines.Count)
        {
            if (currentPhase.questToGrantAfter != null)
                QuestManager.Instance.AcceptQuest(currentPhase.questToGrantAfter);

            currentPhase.onExhausted?.Invoke();
            currentPhase = null;
        }
    }
}
