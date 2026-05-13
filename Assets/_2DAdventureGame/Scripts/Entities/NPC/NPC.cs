using System;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public GameObject dialogueBubble;
    public List<string> dialogueLines = new List<string>();
    private int lineIndex;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dialogueBubble.SetActive(false);
        lineIndex = 0;
    }

    public void Talk()
    {
        UIHandler.Instance.DisplayDialogueWithLine(dialogueLines[lineIndex++]);
        if (lineIndex >= dialogueLines.Count) lineIndex = 0;
    }
}
