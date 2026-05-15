using System;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    [SerializeField] private List<DialogueEntry> entries;
    private DialogueEntry currentEntry;
    private int lineIndex;

    public GameObject dialogueBubble;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dialogueBubble.SetActive(false);
        lineIndex = 0;
    }

    public void Talk()
    {
        if (currentEntry == null)
        {
            foreach (DialogueEntry entry in entries)
                if (entry.Matches(QuestManager.Instance))
                {
                    currentEntry = entry;
                    break;
                }

            if (currentEntry == null) return;
            lineIndex = 0;
        }

        UIHandler.Instance.DisplayDialogueWithLine(currentEntry.lines[lineIndex++]);

        if (lineIndex >= currentEntry.lines.Count)
        {
            if (currentEntry.questToGrantAfter != null)
                QuestManager.Instance.AcceptQuest(currentEntry.questToGrantAfter);

            currentEntry.onExhausted?.Invoke();
            currentEntry = null;
        }
    }
}
