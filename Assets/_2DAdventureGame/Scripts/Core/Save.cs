using System;

[Serializable]
public class Save
{
    public int version;
    public string sceneName;
    public int playerHealth;
    public string activeQuestId;
    public int activeQuestCount;
    public string[] completedQuestIds;
}
