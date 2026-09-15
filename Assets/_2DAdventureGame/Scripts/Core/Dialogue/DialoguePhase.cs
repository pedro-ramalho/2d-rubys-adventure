using System;
using System.Collections.Generic;
using AdventureGame.Core.Quests;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace AdventureGame.Core.Dialogue
{
    [Serializable]
    public class DialoguePhase
    {
        public List<string> Lines;

        public QuestData QuestToGrantAfter;

        public UnityEvent OnExhausted;

        public bool IsEmpty => Lines == null || Lines.Count == 0;
    }
}
