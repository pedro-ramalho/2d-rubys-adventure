using System;

namespace AdventureGame.Core.Quests
{
    [Serializable]
    public class QuestSaveData
    {
        public string QuestId;
        public QuestState State;
    }
}
