using System;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    [SerializeField] private List<DialogueEntry> entries;
    private DialogueEntry currentEntry;
    private int lineIndex;

    public GameObject dialogueBubble;
    public List<string> dialogueLines = new List<string>();


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
                    currentEntry = entry;

            lineIndex = 0;

            return;
        }

        if (lineIndex >= currentEntry.Lines.Count)
        {
            if (currentEntry.QuestToGrantAfter != null)
                QuestManager.Instance.AcceptQuest(new Quest(currentEntry.QuestToGrantAfter));
            
            currentEntry.OnExhausted?.Invoke();
            currentEntry = null;

            return;
        }

        UIHandler.Instance.DisplayDialogueWithLine(currentEntry.Lines[lineIndex]);
    }
}
