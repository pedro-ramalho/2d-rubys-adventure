using System;
using System.Collections.Generic;
using AdventureGame.Core.Quests;

namespace AdventureGame.Core
{
    [Serializable]
    public class Save
    {
        public int Version;
        public string SceneName;
        public int PlayerHealth;
        public List<QuestSaveData> Quests;
    }
}
