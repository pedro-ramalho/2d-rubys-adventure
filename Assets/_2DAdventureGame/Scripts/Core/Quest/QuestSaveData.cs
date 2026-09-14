using System;
using System.Collections.Generic;

namespace AdventureGame.Core.Quest
{
    [Serializable]
    public class QuestSaveData
    {
        public string questId;
        public QuestPhase phase;
        public List<string> consumedIds;
    }
}
