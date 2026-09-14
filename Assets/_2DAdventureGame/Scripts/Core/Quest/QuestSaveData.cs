using System;
using System.Collections.Generic;

namespace AdventureGame.Core.Quests
{
    [Serializable]
    public class QuestSaveData
    {
        public string QuestId;
        public QuestPhase Phase;
        public List<string> ConsumedIds;
    }
}
