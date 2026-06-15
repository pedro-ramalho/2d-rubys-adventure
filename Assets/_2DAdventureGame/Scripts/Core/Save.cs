using System;
using System.Collections.Generic;

[Serializable]
public class Save
{
    public int version;
    public string sceneName;
    public int playerHealth;
    public string activeQuestId;
    public int activeQuestCount;
    public List<string> completedQuestIds;
}
