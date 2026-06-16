using System;
using System.Collections.Generic;

[Serializable]
public class QuestSaveData
{
    public string questId;
    public QuestPhase phase;
    public List<string> consumedIds;
}
