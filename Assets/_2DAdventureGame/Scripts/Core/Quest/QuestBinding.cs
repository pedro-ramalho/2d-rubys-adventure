using UnityEngine;

public class QuestBinding : MonoBehaviour
{
    [SerializeField] private QuestData boundQuest;
    [SerializeField] private bool deactivateOnComplete = true;

    void Start()
    {
        QuestManager.Instance.OnQuestAccepted += HandleAccepted;
        QuestManager.Instance.OnQuestCompleted += HandleCompleted;
    }

    void OnDestroy()
    {
        if (QuestManager.Instance == null) return;
        
        QuestManager.Instance.OnQuestAccepted -= HandleAccepted;
        QuestManager.Instance.OnQuestCompleted -= HandleCompleted;
    }

    void HandleAccepted(Quest quest)
    {
        if (quest.Data.id == boundQuest.id) 
            gameObject.SetActive(true);
    }

    void HandleCompleted(Quest quest)
    {
        if (quest.Data == boundQuest && deactivateOnComplete)
            gameObject.SetActive(false);
    }
}
