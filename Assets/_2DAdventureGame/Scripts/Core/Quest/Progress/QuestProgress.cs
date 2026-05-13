using System.Collections.Generic;
using UnityEngine;

public class QuestProgress
{
    private Quest quest;
    private Dictionary<QuestObjective, int> counts;

    public void HandleReport(QuestReport report)
    {
        
    }

    public bool Complete()
    {
        return false;
    }
}
