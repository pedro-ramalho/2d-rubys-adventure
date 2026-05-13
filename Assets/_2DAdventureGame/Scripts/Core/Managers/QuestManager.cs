using System;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    private Dictionary<Quest, QuestProgress> active;
    private HashSet<Quest> completed;

    public event Action<Quest> OnQuestAccepted;
    public event Action<Quest> OnQuestCompleted;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AcceptQuest(Quest quest)
    {
        
    }

    public void Report(QuestReport report)
    {
        
    }
}
