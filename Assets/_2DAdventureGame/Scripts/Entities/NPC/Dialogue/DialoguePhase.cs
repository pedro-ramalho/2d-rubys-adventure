using System;
using System.Collections.Generic;
using UnityEngine.Events;

[Serializable]
public class DialoguePhase
{
    public List<string> lines;
    public QuestData questToGrantAfter;
    public UnityEvent onExhausted;

    public bool IsEmpty => lines == null || lines.Count == 0;
}