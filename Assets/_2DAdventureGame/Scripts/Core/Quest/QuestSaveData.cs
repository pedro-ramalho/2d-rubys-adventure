using System;
using System.Collections.Generic;

namespace AdventureGame.Core.Quests
{
    [Serializable]
    public class QuestSaveData
    {
        public string QuestId;
        public QuestState State;
        public List<string> ConsumedIds;
    }
}
