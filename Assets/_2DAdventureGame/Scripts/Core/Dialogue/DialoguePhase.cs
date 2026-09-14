using System;
using System.Collections.Generic;
using AdventureGame.Core.Quest;
using UnityEngine.Events;

namespace AdventureGame.Core.Dialogue
{
    [Serializable]
    public class DialoguePhase
    {
        public List<string> lines;
        public QuestData questToGrantAfter;
        public UnityEvent onExhausted;

        public bool IsEmpty => lines == null || lines.Count == 0;
    }
}