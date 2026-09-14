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
        [FormerlySerializedAs("lines")]
        public List<string> Lines;

        [FormerlySerializedAs("questToGrantAfter")]
        public QuestData QuestToGrantAfter;

        [FormerlySerializedAs("onExhausted")]
        public UnityEvent OnExhausted;

        public bool IsEmpty => Lines == null || Lines.Count == 0;
    }
}
