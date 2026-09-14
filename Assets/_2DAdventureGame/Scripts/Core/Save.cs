using System;
using System.Collections.Generic;
using AdventureGame.Core.Quest;

namespace AdventureGame.Core
{
    [Serializable]
    public class Save
    {
        public int version;
        public string sceneName;
        public int playerHealth;
        public List<QuestSaveData> quests;
    }
}
