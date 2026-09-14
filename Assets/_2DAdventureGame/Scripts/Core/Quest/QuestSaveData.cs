using System;
using System.Collections.Generic;

namespace AdventureGame.Core.Quest
{
    [Serializable]
    public class QuestSaveData
    {
        public string QuestId;
        public QuestPhase Phase;
        public List<string> ConsumedIds;
    }
}
